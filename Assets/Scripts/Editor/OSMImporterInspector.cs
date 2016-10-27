using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(OSMImporter))]
public class OSMImporterInspector : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if(GUILayout.Button("Regenerate")) {
			OSMImporter importer = (OSMImporter)target;
			importer.Import ();
		}
	}
}
