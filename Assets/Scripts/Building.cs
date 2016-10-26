using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OSM;

public class Building : MonoBehaviour {

	public Dictionary<string, Material> materials;

	private Way way;

	public void AddWay(Way w) {
		way = w;
		UpdateMeshes ();
		UpdateMaterial ();
	}

	private void UpdateMeshes () {
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		// TODO: MeshCollider meshCollider = GetComponent<MeshCollider> ();

		meshFilter.mesh = GeometryHelper.Extrude(way.BuildVertices().ToArray(), 
												 way.GetHeight());
	}

	private void UpdateMaterial() {
		string materialTag = way.GetTag ("building:material");

		// TODO: handle case where there's no material for the materialTag

		Material floorMaterial = Resources.Load ("Materials/floor") as Material;
		Material roofMaterial = Resources.Load ("Materials/roof") as Material;
		Material wallMaterial = Resources.Load ("Materials/" + materialTag) as Material;

		if (wallMaterial == null) {
			wallMaterial = Resources.Load ("Materials/default") as Material;
		}

		GetComponent<Renderer>().materials = new Material[]{  
			floorMaterial,       
			roofMaterial,
			wallMaterial
		};
	}
}
