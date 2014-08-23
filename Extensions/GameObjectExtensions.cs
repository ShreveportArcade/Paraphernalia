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
using System.Collections;
using System.Collections.Generic;

namespace Paraphernalia.Extensions {
public static class GameObjectExtensions {

	private static void _Destroy (UnityEngine.Object obj) {
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
    		GameObject.Destroy(obj);
    	#if UNITY_EDITOR
    	}
    	else {
    		GameObject.DestroyImmediate(obj);
    	}
    	#endif
	}

    public static void DestroyChildren (this Transform t) {
        for (int i = t.childCount - 1; i >= 0; i--) {
            GameObject child = t.GetChild(i).gameObject;
            _Destroy(child);
        }
    }

    public static T[] GetChildComponents<T> (this Transform t) where T : Component {
        List<T> children = new List<T>();
        for (int i = 0; i < t.childCount; i++) {
            GameObject child = t.GetChild(i).gameObject;
            T component = child.GetComponent<T>();
            if (component != null) children.Add(component);
        }
        return children.ToArray();
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
        T component = go.GetComponent<T>();
        if (component == null) component = go.AddComponent<T>();
        return component;
    }

    public static void DestroyComponent<T>(this GameObject go) where T : Component {
    	T component = go.GetComponent<T>();
    	if (component != null) _Destroy(component);
    }

    public static void DestroyComponents<T>(this GameObject go) where T : Component {
    	T[] components = go.GetComponents<T>();
        for (int i = 0; i < components.Length; i++) {
            T component = components[i];
    		_Destroy(component);
        }
    }
}
}
