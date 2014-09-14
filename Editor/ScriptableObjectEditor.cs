using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObject))]
class ScriptableObjectEditor : Editor {
	
	static bool foldout = false;

	public override void OnInspectorGUI () {
		if (EditorGUILayout.Foldout(foldout, "Show Script")) base.OnInspectorGUI();
	}
}