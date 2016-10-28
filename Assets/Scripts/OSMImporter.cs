using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OSM;

public class OSMImporter : MonoBehaviour {

	OSMData osmData = new OSMData();

	public GameObject buildingPrefab;
	public double north = 36.003265;
	public double south = 35.987988;
	public double east = -78.891792;
	public double west = -78.914795;


	public void Import() {
		// Get OSM Data
//		MapboxDataGetter.GetOSMData(north, south, east, west);

		// Add it to our dataset
		osmData.ImportFromXML("Assets/atc.osm");

		// Delete existing children
		foreach (Transform child in transform) {
			DestroyImmediate(child.gameObject);
		}

		// Draw the scene
		DrawBuildings ();
	}
		
	private void DrawBuildings() {
		foreach (Way way in osmData.GetWays().Values) {
			if (!way.HasTag("building")) {
				continue;
			}

			way.BuildPosition (osmData.GetBounds());

			GameObject buildingGO = (GameObject)Instantiate(buildingPrefab, way.GetPosition(), Quaternion.identity);
//			buildingGO.transform.SetParent (this.gameObject.transform);
			buildingGO.transform.parent = this.transform;
			buildingGO.name = way.id.ToString();
			Building b = buildingGO.GetComponent<Building>();
			b.AddWay(way);
		}
	}
}
