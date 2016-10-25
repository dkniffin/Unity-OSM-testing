using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OSM;

public class Building : MonoBehaviour {
	private Way way;

	public void AddWay(Way w) {
		way = w;
		UpdateMeshes ();
		UpdateMaterial ();
	}

	private void UpdateMeshes () {
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		// TODO: MeshCollider meshCollider = GetComponent<MeshCollider> ();

		meshFilter.mesh = GeometryHelper.Extrude(way.BuildVertices().ToArray(), way.GetHeight());
	}

	private void UpdateMaterial() {
		MeshRenderer rend = GetComponent<MeshRenderer> ();
//		rend.material.SetColor ("_EmissionColor", way.GetMaterialColor ());
	}
}
