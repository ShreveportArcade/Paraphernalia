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
using System.Collections.Generic;
using System.IO;
using Paraphernalia.Utils;
using Paraphernalia.Extensions;

class CubemapTools : Editor {

	static Cubemap[] GetCubemaps () {
		string[] assetGUIDs = Selection.assetGUIDs;
		List<Cubemap> cubemaps = new List<Cubemap>();
		foreach (string assetGUID in assetGUIDs) {
			string path = AssetDatabase.GUIDToAssetPath(assetGUID);
			Cubemap cubemap = AssetDatabase.LoadAssetAtPath(path, typeof(Cubemap)) as Cubemap;
			if (cubemap != null) cubemaps.Add(cubemap);
		}
		return cubemaps.ToArray();		
	}
	static bool ValidateExportCubemap () {
		Cubemap[] cubemaps = GetCubemaps();
		return cubemaps.Length > 0;
	}

	static void ExportCubemap(CubeMappingType exportType) {
		Cubemap[] cubemaps = GetCubemaps();
		string folder = "";
		string filename = null;
		if (cubemaps.Length == 0) return;
		else if (cubemaps.Length == 1) {
			string path = EditorUtility.SaveFilePanel("Save Cubemap as PNG", "", cubemaps[0].name + ".png", "png");
			folder = Path.GetDirectoryName(path);
			filename = Path.GetFileName(path);
		}
		else {
			folder = EditorUtility.SaveFolderPanel("Save Cubemaps to Folder", "", "");
		}
		foreach(Cubemap cubemap in cubemaps) {
			string assetPath = AssetDatabase.GetAssetPath(cubemap);
	 
	        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
	        importer.maxTextureSize = 2048;
	        importer.isReadable = true;
	        importer.textureCompression = TextureImporterCompression.Uncompressed;
	           
	        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
			
			string path = null;
			if (string.IsNullOrEmpty(filename)) {
				path = Path.Combine(folder, cubemap.name + ".png");
			}
			else {
				path = Path.Combine(folder, filename);
			}
			cubemap.SaveToPNG(path, exportType);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
	}

	[MenuItem ("Assets/Cubemap/Export Spherical")]
	static void ExportSpherical () {
		ExportCubemap(CubeMappingType.Spherical);
	}

	[MenuItem ("Assets/Cubemap/Export Spherical", true)]
	static bool ValidateExportSpherical () {
		return ValidateExportCubemap();
	}

	[MenuItem ("Assets/Cubemap/Export Cylindrical")]
	static void ExportCylindrical () {
		ExportCubemap(CubeMappingType.Cylindrical);
	}

	[MenuItem ("Assets/Cubemap/Export Cylindrical", true)]
	static bool ValidateExportCylindrical () {
		return ValidateExportCubemap();
	}

	[MenuItem ("Assets/Cubemap/Export Faces4x3")]
	static void ExportFaces4x3 () {
		ExportCubemap(CubeMappingType.Faces4x3);
	}

	[MenuItem ("Assets/Cubemap/Export Faces4x3", true)]
	static bool ValidateExportFaces4x3 () {
		return ValidateExportCubemap();
	}

	[MenuItem ("Assets/Cubemap/Export Faces3x4")]
	static void ExportFaces3x4 () {
		ExportCubemap(CubeMappingType.Faces3x4);
	}

	[MenuItem ("Assets/Cubemap/Export Faces3x4", true)]
	static bool ValidateExportFaces3x4 () {
		return ValidateExportCubemap();
	}

	[MenuItem ("Assets/Cubemap/Export Faces6x1")]
	static void ExportFaces6x1 () {
		ExportCubemap(CubeMappingType.Faces6x1);
	}

	[MenuItem ("Assets/Cubemap/Export Faces6x1", true)]
	static bool ValidateExportFaces6x1 () {
		return ValidateExportCubemap();
	}

	[MenuItem ("Assets/Cubemap/Export Faces1x6")]
	static void ExportFaces1x6 () {
		ExportCubemap(CubeMappingType.Faces1x6);
	}

	[MenuItem ("Assets/Cubemap/Export Faces1x6", true)]
	static bool ValidateExportFaces1x6 () {
		return ValidateExportCubemap();
	}
}