using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoScript))]
class MonoScriptEditor : Editor {

	public override void OnInspectorGUI () {
		if (target == null) return;
		MonoScript script = target as MonoScript;
		System.Type t = script.GetClass();
		if (!t.IsSubclassOf(typeof(Editor))) {
			if (t.IsSubclassOf(typeof(ScriptableObject)) && GUILayout.Button("Create Asset"))
				ScriptableObjectUtility.CreateAsset(t);
		}
	}
}