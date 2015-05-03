// based on BlenderCameraControls.cs by Marc Kusters (Nighteyes)
// which is based on BlenderCamControl.cs by Sarper Soher
// made into a static class with menu items by Nolan Baker

using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class SceneViewController {

    static UnityEditor.SceneView sceneView {
    	get { return UnityEditor.SceneView.lastActiveSceneView; }
    }

    static Vector3 pivot {
    	get { return sceneView.pivot; }
    }

	[MenuItem("View/Look Right &1")]
    static void LookRight() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.right));
	}

	[MenuItem("View/Look Left &#1")]
    static void LookLeft() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.right));
	}
 
 	[MenuItem("View/Look Forward &2")]
    static void LookForward() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.forward));
	}

	[MenuItem("View/Look Back &#2")]
    static void LookBack() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.forward));
	}

	[MenuItem("View/Look Down &3")]
    static void LookDown() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.up));
	}

	[MenuItem("View/Look Up &#3")]
    static void LookUp() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.up));
	}

	[MenuItem("View/Perspective &5")]
    static void SetPerspective() {
		sceneView.orthographic = false;
    }

    [MenuItem("View/Orthographic &#5")]
    static void SetOrthographic() {
		sceneView.orthographic = true;
    }
}