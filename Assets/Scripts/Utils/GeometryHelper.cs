using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GeometryHelper
{

	public static Mesh Extrude(Vector2 [] poly, int height)
	{
		// convert polygon to triangles
		Triangulator triangulator = new Triangulator(poly);
		int[] tris = triangulator.Triangulate();
		Mesh m = new Mesh();
		Vector3[] vertices = new Vector3[poly.Length*2];

		for(int i=0;i<poly.Length;i++)
		{
			vertices[i].x = poly[i].x;
			vertices[i].y = 0; // bottom vertex
			vertices[i].z = poly[i].y;
			vertices[i+poly.Length].x = poly[i].x;
			vertices[i+poly.Length].y = height; // top vertex 
			vertices[i+poly.Length].z = poly[i].y; 
		}
		int[] triangles = new int[tris.Length*2+poly.Length*6];
		int count_tris = 0;
		// bottom vertices
		for(int i=0;i<tris.Length;i+=3)
		{
			triangles[i] = tris[i];
			triangles[i+1] = tris[i+2];
			triangles[i+2] = tris[i+1];
		} 
		count_tris+=tris.Length;
		// top vertices
		for(int i=0;i<tris.Length;i+=3)
		{
			triangles[count_tris+i] = tris[i+2]+poly.Length;
			triangles[count_tris+i+1] = tris[i]+poly.Length;
			triangles[count_tris+i+2] = tris[i+1]+poly.Length;
		} 
		count_tris+=tris.Length;
		for(int i=0;i<poly.Length;i++)
		{
			// triangles around the perimeter of the object
			int n = (i+1)%poly.Length;
			triangles[count_tris] = i;
			triangles[count_tris+1] = i + poly.Length;
			triangles[count_tris+2] = n;
			triangles[count_tris+3] = n;
			triangles[count_tris+4] = i + poly.Length;
			triangles[count_tris+5] = n + poly.Length;
			count_tris += 6;
		}
		m.vertices = vertices;
		m.triangles = triangles;
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

