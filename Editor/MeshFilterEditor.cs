/*
Copyright (C) 2016 Nolan Baker

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

[CanEditMultipleObjects]
[CustomEditor(typeof(MeshFilter))]
class MeshFilterEditor : Editor {

	private int triCount = 0;
    private int vertCount = 0;
	private int targetCount = -1;

	public override void OnInspectorGUI () {
		
		if (targets == null || targets.Length < 2) {
			MeshFilter filter = target as MeshFilter;
			if (filter != null && filter.sharedMesh != null) {
				triCount = filter.sharedMesh.triangles.Length / 3;
                vertCount = filter.sharedMesh.vertices.Length;
			}
		}
		else if (targetCount != targets.Length) {
			targetCount = targets.Length;
			for (int i = 0; i < targetCount; i++) {
				MeshFilter filter = targets[i] as MeshFilter;
				if (filter != null && filter.sharedMesh != null) {
					triCount += filter.sharedMesh.triangles.Length / 3;
                    vertCount += filter.sharedMesh.vertices.Length;
				}
			}
		}

		EditorGUILayout.LabelField("Triangles: " + triCount);
		EditorGUILayout.LabelField("Vertices: " + vertCount);
		base.OnInspectorGUI();
	}
}