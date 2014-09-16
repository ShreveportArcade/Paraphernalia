using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoScript))]
class MonoScriptEditor : Editor {

	public override void OnInspectorGUI () {
		if (target == null || !(target is MonoScript)) return;
		MonoScript script = target as MonoScript;
		System.Type t = script.GetClass();
		if (t == null) return;
		if (!(t.IsSubclassOf(typeof(Editor)) || t.IsSubclassOf(typeof(EditorWindow)))) {
			if (t.IsSubclassOf(typeof(ScriptableObject)) && GUILayout.Button("Create Asset"))
				ScriptableObjectUtility.CreateAsset(t);
		}
	}
}