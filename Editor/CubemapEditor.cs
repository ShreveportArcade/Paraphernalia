/*
Copyright (C) 2015 Nolan Baker

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

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