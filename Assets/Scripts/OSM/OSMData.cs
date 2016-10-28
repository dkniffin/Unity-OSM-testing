using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OSM;

namespace OSM {
	public class OSMData {

		private string _xmlContent;
		private XmlReader _reader;
		private Element _currentElement;

		private Dictionary<long, Node> nodes = new Dictionary<long, Node> ();
		private Dictionary<long, Way> ways = new Dictionary<long, Way>();
		private LatLonBounds bounds = new LatLonBounds ();

		public void Import() {

		}

		public void ImportFromXML(string osmfilename) {
			_xmlContent = File.ReadAllText (osmfilename);
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


		public Dictionary<long, Node> GetNodes() {
			return nodes;
		}

		public Node GetNodeById(long id) {
			return nodes [id];
		}

		public Dictionary<long, Way> GetWays() {
			return ways;
		}

		public Way GetWay(long id) {
			return ways [id];
		}

		public LatLonBounds GetBounds() {
			return bounds;
		}

		private void parseNode() {
			Node node = new Node ();
			node.id = long.Parse(_reader.GetAttribute("id"));
			node.latitude = float.Parse(_reader.GetAttribute("lat"));
			node.longitude = float.Parse(_reader.GetAttribute("lon"));
			_currentElement = node;
			AddNode(node);
		}

		private void ParseWay() {
			Way way = new Way ();
			way.id = long.Parse(_reader.GetAttribute("id"));
			_currentElement = way;
			AddWay(way);
		}

		private void ParseNd() {
			long node_id = long.Parse(_reader.GetAttribute("ref"));
			Node node = GetNodeById(node_id);
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

			SetBounds (n, e, s, w);
		}


		private void AddNode(Node node) {
			nodes[node.id] = node;
		}

		private void AddWay(Way way) {
			ways[way.id] = way;
		}

		private void SetBounds(double n, double e, double s, double w) {
			bounds.n = n;
			bounds.e = e;
			bounds.s = s;
			bounds.w = w;
		}
	}
}
