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

	[MenuItem("View/Look Right _1")]
    static void LookRight() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.right));
	}
 
 	[MenuItem("View/Look Forward _2")]
    static void LookForward() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.forward));
	}

	[MenuItem("View/Look Down _3")]
    static void LookDown() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.up));
	}

	[MenuItem("View/Look Left _4")]
    static void LookLeft() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.right));
	}

	[MenuItem("View/Look Back _5")]
    static void LookBack() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.forward));
	}

	[MenuItem("View/Look Up _6")]
    static void LookUp() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.up));
	}

	[MenuItem("View/Toggle Perspective/Ortho _7")]
    static void TogglePerspectiveOrtho() {
		sceneView.orthographic = !sceneView.orthographic;
    }
}