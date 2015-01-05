using UnityEditor;
using UnityEngine;
using Paraphernalia.Utils;
using Paraphernalia.Extensions;

[CustomEditor(typeof(Cubemap))]
class CubemapEditor : Editor {

	public static CubeMappingType exportType = CubeMappingType.Faces4x3;

	public override void OnInspectorGUI () {
		base.OnInspectorGUI();

		if (target == null || !(target is Cubemap)) return;
		Cubemap cubemap = target as Cubemap;

		string assetPath = AssetDatabase.GetAssetPath(target);
 
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureType = TextureImporterType.Advanced;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.textureFormat = TextureImporterFormat.PVRTC_RGB4;
           
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
		
		exportType = (CubeMappingType)EditorGUILayout.EnumPopup("Export Type", exportType);
		if (GUILayout.Button("Export")) {
			string path = EditorUtility.SaveFilePanel("Save Cubemap as PNG", "", cubemap.name + ".png", "png");
			cubemap.SaveToPNG(path, exportType);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
	}
}