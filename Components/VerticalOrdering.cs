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

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class VerticalOrdering : MonoBehaviour {

    public Vector3 axis = Vector3.down;
    public float orderMultiplier = 100;
    public float orderOffset = 0;
    public int order {
        get {
            return Mathf.RoundToInt(Vector3.Dot(transform.position * orderMultiplier, axis) + orderOffset);
        }
    }
    public bool isStatic = true;

    #if UNITY_EDITOR
    void Update () {
        if (!Application.isPlaying) UpdateSortOrder();
    }
    #endif

    void OnEnable () {
        UpdateSortOrder();
        if (!isStatic) StartCoroutine("SortCoroutine");
    }

    IEnumerator SortCoroutine () {
        while (enabled && !isStatic) {
            UpdateSortOrder();
            yield return new WaitForEndOfFrame();
        }
    }

    [ContextMenu("Update Sort Order")]
    protected virtual void UpdateSortOrder () {
    }
}
