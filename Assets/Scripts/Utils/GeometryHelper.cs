using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GeometryHelper
{

	// https://forum.unity3d.com/threads/trying-extrude-a-2d-polygon-to-create-a-mesh.102629/
	public static Mesh Extrude(Vector2 [] poly, int height, float textureWidth = 1, float textureHeight = 1)
	{
		/* ============ Vertices ============ */
		// convert polygon to triangles
		Triangulator triangulator = new Triangulator(poly);
		int[] tris = triangulator.Triangulate();
		Mesh m = new Mesh();

		/* It's very counter-intuitive, but: we need a set of vertices for each subMesh. This will probably result in 
		 * duplicate vertices, but that's necessary for mapping UVs.
		 * 
		 * For example, for a cube, we need 24 (6*4) vertices, not 8, because each face needs it's own set of vertices
		 */
		int bottomVertCount = poly.Length;
		int topVertCount = poly.Length;
		int sideVertCount = poly.Length * 2;
		int topAndBottomVertCount = bottomVertCount + topVertCount;
		int totalVertCount = topAndBottomVertCount + sideVertCount;
		Vector3[] vertices = new Vector3[totalVertCount];


		/* The vertices array is built like this:
		 * [bottomVertices...topVertices...sideVerticesBottom...sideVerticesTop]
		 */
		for(int i=0;i<poly.Length;i++)
		{
			// Bottom vertices
			vertices[i].x = poly[i].x;
			vertices[i].y = 0;
			vertices[i].z = poly[i].y;
			// Top vertices
			vertices[bottomVertCount+i].x = poly[i].x;
			vertices[bottomVertCount+i].y = height;
			vertices[bottomVertCount+i].z = poly[i].y;

			// Side vertices, bottom
			int sideBottomVertIdx = topAndBottomVertCount + i;
			vertices[sideBottomVertIdx].x = poly[i].x;
			vertices[sideBottomVertIdx].y = 0;
			vertices[sideBottomVertIdx].z = poly[i].y;
			// Side vertices, top
			int sideTopVertIdx = sideBottomVertIdx + bottomVertCount;
			vertices[sideTopVertIdx].x = poly[i].x;
			vertices[sideTopVertIdx].y = height;
			vertices[sideTopVertIdx].z = poly[i].y;
		}
		m.vertices = vertices;


		/* ============ Triangles ============ */
		m.subMeshCount = 3;

		int[] bottomTriangles = new int[tris.Length];
		for(int i=0;i<tris.Length;i+=3)
		{
			bottomTriangles[i] = tris[i];
			bottomTriangles[i+1] = tris[i+2];
			bottomTriangles[i+2] = tris[i+1];
		} 

		int[] topTriangles = new int[tris.Length];
		for(int i=0;i<tris.Length;i+=3)
		{
			topTriangles[i] = tris[i+2] + bottomVertCount;
			topTriangles[i+1] = tris[i] + bottomVertCount;
			topTriangles[i+2] = tris[i+1] + bottomVertCount;
		} 

		int countTris = 0;
		int[] sideTriangles = new int[poly.Length * 6];
		for(int i=0;i<poly.Length;i++)
		{
			// triangles around the perimeter of the object
			int n = (i+1)%bottomVertCount;
			sideTriangles[countTris]   = topAndBottomVertCount + i;
			sideTriangles[countTris+1] = topAndBottomVertCount + bottomVertCount + i;
			sideTriangles[countTris+2] = topAndBottomVertCount + n;
			sideTriangles[countTris+3] = topAndBottomVertCount + n;
			sideTriangles[countTris+4] = topAndBottomVertCount + bottomVertCount + i;
			sideTriangles[countTris+5] = topAndBottomVertCount + bottomVertCount + n;
			countTris += 6;
		}
		m.SetTriangles (bottomTriangles, 0);
		m.SetTriangles (topTriangles, 1);
		m.SetTriangles (sideTriangles, 2);

		/* ============ UV ============ */
		Vector2[] uv = new Vector2[vertices.Length];
		// TODO: Top/Bottom UVs?

		// Since the height is the same for the entire geometry, calculate this once here.
		float distanceBetweenTopAndBottom = Vector3.Distance (
			                                    vertices [topAndBottomVertCount], 
			                                    vertices [topAndBottomVertCount + bottomVertCount]);

		float currentUVX = 0;
		// Create side UVs, by looping through each side-bottom vertex, 
		// and assigning UVs for it and the corresponding side-top vertex
		for (int i = 0; i < bottomVertCount; i++) {
			// Calculate UV in the X direction
			int bottomVertexIdx = topAndBottomVertCount + i;
			Vector3 thisVertex = vertices [bottomVertexIdx];
			int prevVertexIdx = (bottomVertexIdx > 0) ? bottomVertexIdx - 1 : bottomVertCount;
			Vector3 prevVertex = vertices [prevVertexIdx];

			float distanceToPrevVertex = Vector3.Distance (thisVertex, prevVertex);

			int repeatTimesX = (int)Math.Floor(distanceToPrevVertex / textureWidth);
			currentUVX += repeatTimesX;

			float uvX = currentUVX;

			// Calculate UV in the Y direction
			int topVertexIdx = topAndBottomVertCount + bottomVertCount + i;
			int repeatTimesY = (int)Math.Floor (distanceBetweenTopAndBottom / textureHeight);
			float uvY = repeatTimesY;

			// Set the UVs
			uv[bottomVertexIdx] = new Vector2(uvX, 0); // Bottom
			uv[topVertexIdx] = new Vector2(uvX, uvY); // Top
		}
			
		m.uv = uv;

		m.RecalculateNormals();
		m.RecalculateBounds();
		m.Optimize();
		return m;
	}

	public static bool CheckIfClockwise(List<Vector2> polygon) {
		// http://stackoverflow.com/a/1165943/1202488
		float total = 0;
		int last_i = polygon.Count - 1;
		for (var i = 0; i <= last_i; i++) {
			Vector2 vertex = polygon [i];
			int next_i = (i == polygon.Count - 1) ? 0 : i + 1;
			Vector2 next_vertex = polygon [next_i];
			total += (next_vertex.x - vertex.x) * (next_vertex.y + vertex.y);
		}
		return total >= 1;
	}

	public static List<Vector2> NormalizeToClockwise(List<Vector2> polygon) {
		if (CheckIfClockwise (polygon)) {
			polygon.Reverse ();
		}
		return polygon;
	}
}

