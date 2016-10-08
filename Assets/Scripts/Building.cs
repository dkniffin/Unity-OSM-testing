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
		// TODO: MeshCollider meshCollider = GetComponent<MeshCollider> ();

		meshFilter.mesh = GeometryHelper.Extrude(way.BuildVertices().ToArray(), way.GetHeight());
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
