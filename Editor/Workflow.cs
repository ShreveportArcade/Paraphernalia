// A collection of workflow scripts from http://wiki.unity3d.com/index.php/Scripts/Editor
// Shortcut keys, Undo functionality, and RectTransform compatibility added by Nolan Baker
	
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class Workflow : Editor {
 
	// From: http://wiki.unity3d.com/index.php/AddChild
	// Original Author: Neil Carter (NCarter)
	[MenuItem ("GameObject/Add Child &c")]
	static void AddChild() {
		Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
		Undo.RecordObjects(transforms, "adds children to selected transforms");
		foreach(Transform transform in transforms) {
			GameObject newChild = new GameObject("NewChild");
			Undo.RegisterCreatedObjectUndo(newChild, "create child");
			newChild.transform.parent = transform;
			newChild.transform.localPosition = Vector3.zero;
			if (transform is RectTransform) {
				RectTransform r = newChild.AddComponent<RectTransform>();
				r.anchorMin = Vector2.zero;
				r.anchorMax = Vector2.one;
			}
		}
	}

	// From: http://wiki.unity3d.com/index.php/AddParent
	// Original Author: Neil Carter (NCarter)
	static void AddParent(Vector3 center) {
		Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
 		if (transforms.Length == 0) return;
		
		GameObject newParent = new GameObject("NewParent");
		Undo.RegisterCreatedObjectUndo(newParent, "create parent");
		Transform newParentTransform = newParent.transform;
		newParentTransform.position = center;
		Undo.SetTransformParent(newParentTransform, transforms[0].parent, "sets parent of created obj");
 
 		int rects = 0;
		foreach (Transform t in transforms) { 
			if (t is RectTransform) rects++;
		}

		if (transforms.Length == rects) {
			RectTransform r = newParent.AddComponent<RectTransform>();
			r.localScale = Vector3.one;
			r.anchorMin = Vector2.zero;
			r.anchorMax = Vector2.one;
			r.offsetMin = Vector2.zero;
			r.offsetMax = Vector2.zero;
			r.localPosition = Vector3.zero;
			newParentTransform = r as Transform;
		}

		foreach (Transform t in transforms) { 
			Undo.SetTransformParent(t, newParentTransform, "sets parent of selected");
		}

	}

	[MenuItem ("GameObject/Add Parent at Origin %g")]
	static void AddParentAtOrigin () {
		AddParent(Vector3.zero);
	}

	[MenuItem ("GameObject/Add Parent at Center &g")]
	static void AddParentAtCenter () {
		Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
		Vector3 center = System.Array.ConvertAll(transforms, t => t.position).Average();
 		AddParent(center);
	}
 
	// From: http://wiki.unity3d.com/index.php/InvertSelection
	// Original Author: Mift (mift)
	[MenuItem ("Edit/Invert Selection &i")]
	static void InvertSelection() { 
		List< GameObject > oldSelection = new List< GameObject >();
		List< GameObject > newSelection = new List< GameObject >();
 
		foreach(GameObject obj in Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab)) {
			oldSelection.Add(obj);
		}
 
		foreach(GameObject obj in FindObjectsOfType(typeof(GameObject))) {
			if (!oldSelection.Contains(obj)) {
				newSelection.Add(obj);
			}
		}
 
		Undo.RecordObjects(Selection.objects, "inverts selection");
		Selection.objects = newSelection.ToArray();
	}

	[MenuItem ("Edit/Select Children %&c")]
	static void SelectChildren() { 
		List< GameObject > oldSelection = new List< GameObject >();
		List< GameObject > newSelection = new List< GameObject >();
 
		foreach (GameObject obj in Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab)) {
			oldSelection.Add(obj);
		}
 
		foreach (GameObject obj in oldSelection) {
			foreach (Transform t in obj.transform) {
				newSelection.Add(t.gameObject);
			}
		}
 
		Undo.RecordObjects(Selection.objects, "selects children");
		Selection.objects = newSelection.ToArray();
	}

	// From: http://wiki.unity3d.com/index.php/MoveToOrigin
	// Original Author: Matthew Miner (matthew@matthewminer.com)
	[MenuItem ("GameObject/Move To Origin &m")]
	static void MenuMoveToOrigin () {
		foreach (Transform t in Selection.transforms) {
			Undo.RecordObject(t, "Move " + t.name);
			t.position = Vector3.zero;
			Debug.Log("Moving " + t.name + " to origin");
		}
	}
}