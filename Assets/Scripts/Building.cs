using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OSM;

public class Building : MonoBehaviour {
	private Way way;

	public void AddWay(Way w) {
		way = w;
		UpdateMeshes ();
//		UpdateMaterial ();
	}

	private void UpdateMeshes () {
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
//		MeshCollider meshCollider = GetComponent<MeshCollider> ();

		Vector2[] way_vertices = way.BuildVertices ().ToArray();
		Vector3[] vertices = new Vector3[way_vertices.Length];
		int[] triangles;
//		Vector2[] uv;
		Vector3[] normals = new Vector3[vertices.Length];


		for (int i = 0; i < way_vertices.Length; i++) {
			Vector2 vertex = way_vertices [i];
			// Set up 3d vertex from 2d vertex
			vertices [i] = new Vector3 (vertex.x, 0, vertex.y);

			// The normal is always the same, since we're in 2d
			normals [i] = Vector3.up;
		}

//		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(way_vertices);
		triangles = tr.Triangulate();


		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
//		mesh.uv = uv;
		mesh.normals = normals;

		meshFilter.mesh = mesh;
//		# TODO: meshCollider
	}

	private void UpdateMaterial() {
		Dictionary<string, Material> materialMap = new Dictionary<string, Material>{
			{ "police", (Material)  Resources.Load ("Materials/Building/PoliceStation") },
			{ "fire_department", (Material) Resources.Load ("Materials/Building/FireStation") },
			{ "medical", (Material) Resources.Load ("Materials/Building/medical") },
			{ "unknown", (Material) Resources.Load ("Materials/Building/Default") }
		};
		MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
		meshRenderer.material = materialMap [way.BuildingType ()];
	}
}
