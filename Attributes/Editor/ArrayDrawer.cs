// from http://forum.unity3d.com/threads/240895-Generic-Editor-Array-PropertyAttribute-Tools
// Author: cfloutier 

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(ArrayAttribute))]
public class ArrayDrawer : PropertyDrawer
{
	const float widthBt = 35;

	void addArrayTools(Rect position, SerializedProperty property)
	{
		string path = property.propertyPath;
		int arrayInd = path.LastIndexOf(".Array");
		bool bIsArray = arrayInd >= 0;

		if (bIsArray)
		{
			SerializedObject so = property.serializedObject;
			string arrayPath = path.Substring(0, arrayInd);
			SerializedProperty arrayProp = so.FindProperty(arrayPath);

			//Next we need to grab the index from the path string
			int indStart = path.IndexOf("[") + 1;
			int indEnd = path.IndexOf("]");

			string indString = path.Substring(indStart, indEnd - indStart);

			int myIndex = int.Parse(indString);
			Rect rcButton = position;
			rcButton.height = EditorGUIUtility.singleLineHeight;
			rcButton.x = position.xMax - widthBt * 4;
			rcButton.width = widthBt;

			bool lastEnabled = GUI.enabled;

			if (myIndex == 0)
				GUI.enabled = false;

			if (GUI.Button(rcButton, "^"))
			{
				arrayProp.MoveArrayElement(myIndex, myIndex - 1);
				so.ApplyModifiedProperties();
				
			}

			rcButton.x += widthBt;
			GUI.enabled = lastEnabled;
			if (myIndex >= arrayProp.arraySize - 1)
				GUI.enabled = false;

			if (GUI.Button(rcButton, "v"))
			{
				arrayProp.MoveArrayElement(myIndex, myIndex + 1);
				so.ApplyModifiedProperties();
			}

			GUI.enabled = lastEnabled;

			rcButton.x += widthBt;
			if (GUI.Button(rcButton, "-"))
			{
				arrayProp.DeleteArrayElementAtIndex(myIndex);
				so.ApplyModifiedProperties();
			}

			rcButton.x += widthBt;
			if (GUI.Button(rcButton, "+"))
			{
				arrayProp.InsertArrayElementAtIndex(myIndex);
				so.ApplyModifiedProperties();
			}
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		addArrayTools(position, property);
		Rect rc = position;
		if (!property.isExpanded)
			rc.width -= widthBt * 4;

		EditorGUI.PropertyField(rc, property, label, true);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property);
	}
}