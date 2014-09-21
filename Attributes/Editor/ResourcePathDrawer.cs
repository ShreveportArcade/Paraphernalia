/*
Copyright (C) 2014 Nolan Baker

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

using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
public class ResourcePathDrawer : PropertyDrawer {

	public override void OnGUI (Rect rect, SerializedProperty property, GUIContent label) {
		if (property.propertyType == SerializedPropertyType.String) {

			Type objType = ((ResourcePathAttribute)attribute).resourceType;
			UnityEngine.Object obj = null;
			if (property.stringValue != "") {
				obj = Resources.Load(property.stringValue, objType);
			}

			obj = EditorGUI.ObjectField(rect, label, obj, objType, false);
			string assetPath = AssetDatabase.GetAssetPath(obj);
			if (assetPath != "") {
				string[] split = assetPath.Split(new string[]{"Resources/"}, StringSplitOptions.RemoveEmptyEntries);
				string ext = System.IO.Path.GetExtension(assetPath);
				property.stringValue = split[split.Length-1].Replace(ext, "");
			}
		}
	}
}