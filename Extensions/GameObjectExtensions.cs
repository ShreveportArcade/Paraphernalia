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
using Paraphernalia.Utils;

#if UNITY_EDITOR
using System.Reflection;
#endif

namespace Paraphernalia.Extensions {
public static class GameObjectExtensions {

    public static UnityEngine.Object Instantiate (this UnityEngine.Object obj) {
        #if UNITY_EDITOR
        if (Application.isPlaying) {
        #endif
            return GameObject.Instantiate(obj);
        #if UNITY_EDITOR
        }
        else {
            System.Type prefabUtility = System.Type.GetType("UnityEditor.PrefabUtility, UnityEditor", false, false);
            System.Type[] paramTypes = new System.Type[]{ typeof(UnityEngine.Object) };
            System.Reflection.MethodInfo method = prefabUtility.GetMethod(
                "InstantiatePrefab", 
                BindingFlags.Public | BindingFlags.Static,
                null,
                paramTypes,
                null
            );
            return method.Invoke(null, new UnityEngine.Object[]{obj}) as UnityEngine.Object;
        }
        #endif
    }

    public static bool GetStatic(this GameObject go) {
        #if UNITY_EDITOR
        System.Reflection.PropertyInfo property = go.GetType().GetProperty("isStatic", BindingFlags.Instance | BindingFlags.Public);
        return (bool)property.GetValue(go, null);
        #else
        return false;
        #endif
    }

    public static void SetStatic(this GameObject go, bool isStatic) {
        #if UNITY_EDITOR
        System.Reflection.PropertyInfo property = go.GetType().GetProperty("isStatic", BindingFlags.Instance | BindingFlags.Public);
        property.SetValue(go, isStatic, null);
        #endif
    }

    public static bool HasActiveChildren<T> (this Transform t) where T : Component {
        for (int i = 0; i < t.childCount; i++) {
            GameObject child = t.GetChild(i).gameObject;
            if (child.activeSelf && child.GetComponent<T>()) return true;
        }
        return false;
    }

    public static List<T> GetActiveChildren<T> (this Transform t) where T : Component {
        List<T> activeChildren = new List<T>();
        for (int i = 0; i < t.childCount; i++) {
            GameObject child = t.GetChild(i).gameObject;
            T component = child.GetComponent<T>();
            if (child.activeSelf && component != null) activeChildren.Add(component);
        }
        return activeChildren;
    }

    public static void DestroyChildren (this Transform t) {
        for (int i = t.childCount - 1; i >= 0; i--) {
            GameObject child = t.GetChild(i).gameObject;
            GameObjectUtils.Destroy(child);
        }
    }

    public static void DestroyChildrenAfter (this Transform t, int index) {
        for (int i = t.childCount - 1; i >= index; i--) {
            GameObject child = t.GetChild(i).gameObject;
            GameObjectUtils.Destroy(child);
        }
    }

    public static T GetAncestorComponent<T> (this GameObject g) where T : Component {
        Transform parent = g.transform.parent;
        while (parent != null) {
            T component = parent.gameObject.GetComponent<T>();
            if (component != null) return component;
            parent = parent.parent;
        }
        return null;
    }

    public static T[] GetChildComponents<T> (this GameObject g) where T : Component {
        return g.transform.GetChildComponents<T>();
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


    public static T GetOrAddComponent<T> (this GameObject go) where T : Component {
        T component = go.GetComponent<T>();
        if (component == null) component = go.AddComponent<T>();
        return component;
    }

    public static void DestroyComponent<T> (this GameObject go) where T : Component {
    	T component = go.GetComponent<T>();
    	if (component != null) GameObjectUtils.Destroy(component);
    }

    public static void DestroyComponents<T> (this GameObject go) where T : Component {
    	T[] components = go.GetComponents<T>();
        for (int i = 0; i < components.Length; i++) {
            T component = components[i];
    		GameObjectUtils.Destroy(component);
        }
    }

    public static Bounds RendererBounds (this GameObject go, bool ignoreParticles = true) {
        List<Renderer> renderers = new List<Renderer>();
        go.GetComponentsInChildren<Renderer>(renderers);

        if (ignoreParticles) {
            renderers.RemoveAll((r) => r is ParticleSystemRenderer);
        }

        if (renderers.Count == 0) {
            return new Bounds(go.transform.position, Vector3.one * 0.01f);
        }
        Bounds b = renderers[0].bounds;
        for (int i = renderers.Count-1; i > 0; i--) {
            b.Encapsulate(renderers[i].bounds);
        }

        return b;
    }

    public static bool IsVisible (this GameObject go) {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i].isVisible) return true;
        }
        return false;
    }

    public static bool InLayerMask (this GameObject go, LayerMask mask) {
        int layer = 1 << go.layer;
        return ((mask & layer) == layer);
    }
}
}
