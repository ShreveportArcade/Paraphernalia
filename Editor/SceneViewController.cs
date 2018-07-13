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
#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _1")]
#else
	[MenuItem("View/Look Right _&1")]
#endif
    static void LookRight() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.right));
	}

#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _2")]
#else
 	[MenuItem("View/Look Forward _&2")]
#endif 
    static void LookForward() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.forward));
	}

#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _3")]
#else
	[MenuItem("View/Look Down _&3")]
#endif
    static void LookDown() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.up));
	}

#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _4")]
#else
	[MenuItem("View/Look Left _&4")]
#endif
    static void LookLeft() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.right));
	}

#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _5")]
#else
	[MenuItem("View/Look Back _&5")]
#endif
    static void LookBack() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(-Vector3.forward));
	}

#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _6")]
#else
	[MenuItem("View/Look Up _&6")]
#endif
    static void LookUp() {
		sceneView.LookAt(pivot, Quaternion.LookRotation(Vector3.up));
	}

#if UNITY_EDITOR_OSX
	[MenuItem("View/Look Right _7")]
#else
	[MenuItem("View/Toggle Perspective/Ortho _&7")]
#endif
    static void TogglePerspectiveOrtho() {
		sceneView.orthographic = !sceneView.orthographic;
    }
}