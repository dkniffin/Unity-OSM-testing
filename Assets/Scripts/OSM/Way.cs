using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OSM {
	public class Way : Element
	{
		private List<Node> nodes;

		private double boundingBoxN = -90.0;
		private double boundingBoxE = -180.0;
		private double boundingBoxS = 90.0;
		private double boundingBoxW = 180.0;

		private double centerLat = 1000.0;
		private double centerLon = 1000.0;

		private Vector3 position;

		public Way () {
			nodes = new List<Node> ();
		}

		public void AddNode(Node node) {
			nodes.Add (node);

			if (node.latitude > boundingBoxN) {
				boundingBoxN = node.latitude;
			}
			if (node.longitude > boundingBoxE) {
				boundingBoxE = node.longitude;
			}
			if (node.latitude < boundingBoxS) {
				boundingBoxS = node.latitude;
			}
			if (node.longitude < boundingBoxW) {
				boundingBoxW = node.longitude;
			}
		}

		public List<Node> getNodes() {
			return nodes;
		}

		// This method takes in the map bounds, and scales the current way so that origin is in the southwest corner of the bounds
		//
		public List<Vector2> BuildVertices() {
			List<Vector2> vertices = new List<Vector2> ();

			foreach(Node node in nodes) {
				var x = (float)DistanceBetween(CenterLat(), CenterLon(), CenterLat(), node.longitude);
				if (CenterLon () > node.longitude) {
					x = 0 - x;
				}

				var y = (float)DistanceBetween(CenterLat(), CenterLon(), node.latitude, CenterLon());
				if (CenterLat () > node.latitude) {
					y = 0 - y;
				}

				vertices.Add(new Vector2(x, y));
			}
			// In OSM, the first node and the last node are the same. In Unity, we don't need that duplication
			vertices.RemoveAt (vertices.Count - 1);
			return GeometryHelper.NormalizeToClockwise (vertices);
		}

		public void BuildPosition(LatLonBounds bounds) {
			var x = (float)DistanceBetween (bounds.s, bounds.w, bounds.s, CenterLon ());
			var y = (float)DistanceBetween (bounds.s, bounds.w, CenterLat (), bounds.w);
			position = new Vector3 (x, 0, y);
		}

		public Vector3 GetPosition() {
			return position;
		}

		public int GetHeight() {
			/* Return the height of the building, based on what OSM tags are available
			   In descending order of preference:
			   - height=* tag
			   - levels * levelheight calculation
			    - levels based on:
			     - levels=* tag
			     - building=* tags (building type)
			     - options.levels
			    - levelheight based on:
			     - options.levelHeight
			*/
			int height; // In meters
			string tagHeight = GetTag("height");
			if (tagHeight != "") {
				// If the height tag is defined, use it
				// TODO: Check for various values like ft, etc (right now we assume meters)
				height = int.Parse(tagHeight);
			} else {
				// Otherwise use levels for calculation
				height = GetNumLevels() * 3; // 3m per floor
			}
			return height;
		}

		private int GetNumLevels() {
			string tagBuildingLevels = GetTag("building:levels");

			if (tagBuildingLevels != "") {
				return int.Parse(tagBuildingLevels);
			} else {
				// If we don't directly have level info, infer from building type
				string tagBuilding = GetTag ("building");
				switch (tagBuilding) {
				case "house":
				case "garage":
				case "roof": // TODO: Handle this separately
				case "hut":
					return 1;
				case "school":
					return UnityEngine.Random.Range(1, 2);
				case "apartments":
				case "office":
					return UnityEngine.Random.Range(1, 5);
				case "hospital":
					return UnityEngine.Random.Range(3, 5);
				case "hotel":
					return UnityEngine.Random.Range(10, 20);
				default:
					return UnityEngine.Random.Range(1, 3);
				}
			}
		}

		public string BuildingType() {
			if (HasTag ("amenity", "police")) {
				return "police";
			} else if (HasTag ("amenity", "fire_station")) {
				return "fire_department";
			} else if (HasTag ("amenity", new[]{"hospital", "doctors", "dentist", "clinic", "pharmacy", "veterinary"})) {
				return "medical";
			} else {
				return "unknown";
			}
		}

		public Dictionary<string, double> GetBoundingBox() {
			return new Dictionary<string, double>{
				{ "n", boundingBoxN },
				{ "e", boundingBoxE },
				{ "s", boundingBoxS },
				{ "w", boundingBoxW }
			};
		}

		private double CenterLat() {
			if (centerLat == 1000.0) {
				centerLat = (boundingBoxN + boundingBoxS) / 2;
			}
			return centerLat;
		}

		private double CenterLon() {
			if (centerLon == 1000.0) {
				centerLon = (boundingBoxW + boundingBoxE) / 2;
			}
			return centerLon;
		}

		private double DistanceBetween(double lat1, double lon1, double lat2, double lon2) {
			var earthRadius = 6378137; // earth radius in meters
			var d2r = Math.PI / 180;
			var dLat = (lat2 - lat1) * d2r;
			var dLon = (lon2 - lon1) * d2r;
			var lat1rad = lat1 * d2r;
			var lat2rad = lat2 * d2r;
			var sin1 = Math.Sin (dLat / 2);
			var sin2 = Math.Sin (dLon / 2);

			var a = sin1 * sin1 + sin2 * sin2 * Math.Cos(lat1rad) * Math.Cos(lat2rad);

			return earthRadius * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
		}

		public Color GetMaterialColor() {
			switch(GetTag("material")) {
			case "brick":
			case "bricks":
				return new Color(204,119,85);
			case "bronze":
				return new Color(255,238,204);
			case "canvas":
			case "sheet":
			case "sheets":
			case "tent":
				return new Color(255,248,240);
			case "concrete":
				return new Color(153,153,153);
			case "copper":
				return new Color(160,224,208);
			case "glass":
			case "glas":
			case "glassfront":
				return new Color(232,248,248);
			case "gold":
				return new Color(255,204,0);
			case "plants":
			case "grass":
			case "thatch":
				return new Color(0,153,51);
			case "metal":
			case "steel":
				return new Color(170,170,170);
			case "panel":
			case "panels":
				return new Color(255,248,240);
			case "plaster":
			case "plastered":
				return new Color(153,153,153);
			case "roof_tiles":
			case "tile":
			case "tiles":
			case "rooftiles":
				return new Color(240,128,96);
			case "silver":
				return new Color(204,204,204);
			case "slate":
			case "slates":
				return new Color(102,102,102);
			case "stone":
			case "block":
			case "masonry":
			case "granite":
			case "paving_stones":
			case "sandstone":
				return new Color(153,102,102);
			case "tar_paper":
			case "asphalt":
			case "bitumen":
			case "roofingfelt":
			case "shingle":
			case "shingles":
			case "tar":
				return new Color(51,51,51);
			case "wood":
				return new Color(222,184,135);
			default:
				return new Color(220, 210, 200);
			}
		}
	}
}
