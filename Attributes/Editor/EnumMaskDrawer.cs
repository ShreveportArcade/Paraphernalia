// based on http://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer
// simplified by Nolan Baker

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
public class EnumMaskDrawer : PropertyDrawer {

	public override void OnGUI (Rect rect, SerializedProperty property, GUIContent label) {
		if (property.propertyType == SerializedPropertyType.Enum) {
			property.intValue = EditorGUI.MaskField(rect, label, property.intValue, property.enumNames);
		}
	}
}