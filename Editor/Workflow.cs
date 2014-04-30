// A collection of workflow scripts from http://wiki.unity3d.com/index.php/Scripts/Editor
// Shortcut keys and Undo functionality added by Nolan Baker
// License: Creative Commons Attribution Share Alike
    
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Workflow : Editor {
 
 	// From: http://wiki.unity3d.com/index.php/AddChild
 	// Author: Neil Carter (NCarter)
 	[MenuItem ("GameObject/Add Child &c")]
    static void MenuAddChild() {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
		Undo.RecordObjects(transforms, "adds children to selected transforms");
        foreach(Transform transform in transforms) {
            GameObject newChild = new GameObject("_null");
            Undo.RegisterCreatedObjectUndo(newChild, "Create child");
            newChild.transform.parent = transform;
			newChild.transform.localPosition = Vector3.zero;
        }
    }

    // From: http://wiki.unity3d.com/index.php/AddParent
    // Author: Neil Carter (NCarter)
    [MenuItem ("GameObject/Add Parent %g")]
    static void MenuInsertParent() {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
 
        GameObject newParent = new GameObject("_Parent");
        Undo.RegisterCreatedObjectUndo(newParent, "Create child");
        Transform newParentTransform = newParent.transform;
 
        Undo.RecordObjects(transforms, "adds parent to selected transforms");
        if (transforms.Length == 1) {
            Transform originalParent = transforms[0].parent;
            transforms[0].parent = newParentTransform;
            if (originalParent) {
                newParentTransform.parent = originalParent;
            }
        }
        else {
            foreach (Transform transform in transforms) {
                transform.parent = newParentTransform;
            }
        }
    }
 
    // From: http://wiki.unity3d.com/index.php/InvertSelection
    // Author: Mift (mift)
    [MenuItem ("Edit/Invert Selection")]
    static void static_InvertSelection() { 
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

    // From: http://wiki.unity3d.com/index.php/MoveToOrigin
    // Author: Matthew Miner (matthew@matthewminer.com)
    [MenuItem ("GameObject/Move To Origin")]
	static void MenuMoveToOrigin () {
		foreach (Transform t in Selection.transforms) {
			Undo.RecordObject(t, "Move " + t.name);
			t.position = Vector3.zero;
			Debug.Log("Moving " + t.name + " to origin");
		}
    }
}