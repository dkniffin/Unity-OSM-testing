using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OSM;

public class OSMImporter : MonoBehaviour {

	OSMData osmData = new OSMData();

	public GameObject buildingPrefab;
	private string _xmlContent;
	private XmlReader _reader;
	private Element _currentElement;

	public void Import() {
		// Import OSM Data
		ImportOSMData ();

		// Delete existing children
		foreach (Transform child in transform) {
			DestroyImmediate(child.gameObject);
		}

		// Draw the scene
		DrawBuildings ();
	}

	private void ImportOSMData() {
		_xmlContent = File.ReadAllText ("Assets/atc.osm");
		_reader = XmlReader.Create (new StringReader (_xmlContent));

		while (_reader.Read ()) {
			if (_reader.NodeType == XmlNodeType.Element) {
				switch (_reader.Name) {
				case "node":
					parseNode ();
					break;
				case "tag":
					ParseTag ();
					break;
				case "nd":
					ParseNd ();
					break;
				case "way":
					ParseWay ();
					break;
			    /*
				case "relation":
					ParseRelation ();
					break;
				case "member":
					ParseMember ();
					break;
			    */
				case "bounds":
					ParseBounds ();
					break;
				}
			}
		}

	}

	private void parseNode() {
		Node node = new Node ();
		node.id = long.Parse(_reader.GetAttribute("id"));
		node.latitude = float.Parse(_reader.GetAttribute("lat"));
		node.longitude = float.Parse(_reader.GetAttribute("lon"));
		_currentElement = node;
		osmData.AddNode(node);
	}

	private void ParseWay() {
		Way way = new Way ();
		way.id = long.Parse(_reader.GetAttribute("id"));
		_currentElement = way;
		osmData.AddWay(way);
	}

	private void ParseNd() {
		long node_id = long.Parse(_reader.GetAttribute("ref"));
		Node node = osmData.GetNodeById(node_id);
		(_currentElement as Way).AddNode(node);
	}

	private void ParseTag() {
		var key = _reader.GetAttribute("k");
		var value = _reader.GetAttribute("v");
		_currentElement.tags[key] = value;
	}

	private void ParseBounds() {
		var n = double.Parse(_reader.GetAttribute("maxlat"));
		var e = double.Parse(_reader.GetAttribute("maxlon"));
		var s = double.Parse(_reader.GetAttribute("minlat"));
		var w = double.Parse(_reader.GetAttribute("minlon"));

		osmData.SetBounds (n, e, s, w);
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
