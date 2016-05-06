// from http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
// Author: Brandon Edmark

using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility {

	public static void CreateAsset<T> () where T : ScriptableObject {
		T asset = ScriptableObject.CreateInstance<T> ();
 
		string path = GetPath();
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
 
		AssetDatabase.CreateAsset(asset, assetPathAndName);
 
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	public static void CreateAsset (System.Type type) {
		ScriptableObject asset = ScriptableObject.CreateInstance(type);
 
		string path = GetPath();
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + type.ToString() + ".asset");
 
		AssetDatabase.CreateAsset (asset, assetPathAndName);
 
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	private static string GetPath () {
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
			path = "Assets";
		else if (Path.GetExtension(path) != "") 
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		return path;
	}
}