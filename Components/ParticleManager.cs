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
using System.Collections.Generic;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;

namespace Paraphernalia.Components {
public class ParticleManager : MonoBehaviour {

	public ParticleSystem[] particlePrefabs;
	[Range(0,50)] public int poolSize = 15;
	public static ParticleManager instance;

	private Dictionary<int, ParticleSystem> prefabs = new Dictionary<int, ParticleSystem>();
	private Dictionary<int, List<ParticleSystem>> pools = new Dictionary<int, List<ParticleSystem>>();
	private Dictionary<int, int> currentIndices = new Dictionary<int, int>();

	void Awake () {
		if (instance != null) {
			GameObjectUtils.Destroy(gameObject);
		}
		else {
			instance = this;
			CreateParticleSystemPool();
		}
	}

	void CreateParticleSystemPool () {
		for (int prefabIndex = 0; prefabIndex < particlePrefabs.Length; prefabIndex++) {
            string name = particlePrefabs[prefabIndex].name;
			int hashCode = name.GetHashCode();
			pools[hashCode] = new List<ParticleSystem>();
			currentIndices[hashCode] = 0;
			prefabs[hashCode] = particlePrefabs[prefabIndex];
			for (int poolIndex = 0; poolIndex < poolSize; poolIndex++) {
				ParticleSystem particleSystem = particlePrefabs[prefabIndex].Instantiate() as ParticleSystem;
				particleSystem.transform.parent = transform;
				pools[hashCode].Add(particleSystem);
			}
		}
	}

	void RefreshPool(string name) {
        int hashCode = name.GetHashCode();
		for (int poolIndex = pools[hashCode].Count; poolIndex < poolSize; poolIndex++) {
			ParticleSystem particleSystem = prefabs[hashCode].Instantiate() as ParticleSystem;
			particleSystem.transform.parent = transform;
			pools[hashCode].Add(particleSystem);
		}
	}

	public static ParticleSystem Play(string name, Transform t) {
		return Play(name, t.gameObject.RendererBounds().center, Vector3.up, 1, null, t);
	}

	public static ParticleSystem Play(string name, Transform t, Color color) {
		return Play(name, t.gameObject.RendererBounds().center, Vector3.up, 1, color, t);
	}

	public static ParticleSystem Play(string name, Vector3 position, Color? color = null) {
		return Play(name, position, Vector3.up, color);
	}

	public static ParticleSystem Play(string name, Vector3 position, Vector3 normal, Color? color = null) {
		return Play(name, position, normal, 1, color);
	}

	public static ParticleSystem Play(string name, Vector3 position, Vector3 normal, float size, Color? color = null, Transform t = null) {
        int hashCode = name.GetHashCode();
        if (instance == null || !instance.currentIndices.ContainsKey(hashCode)) return null;

		List<ParticleSystem> pool = instance.pools[hashCode];
		pool.RemoveAll((i) => i == null);
		int index = instance.currentIndices[hashCode];
		if (index >= pool.Count) instance.RefreshPool(name);
		
		ParticleSystem particleSystem = pool[index];
		if (t != null) particleSystem.transform.parent = t;
		particleSystem.transform.position = position;
		particleSystem.transform.localScale = Vector3.one * size;
		particleSystem.transform.up = normal;
		ParticleSystem prefab = instance.prefabs[hashCode];
		ParticleSystem.MainModule main = particleSystem.main;
		if (color != null) main.startColor = color.Value;
		particleSystem.Play();
		index = (index + 1) % instance.poolSize;
		instance.currentIndices[hashCode] = index;
		return particleSystem;
	}

    public static void StopAll() {
        foreach (List<ParticleSystem> pool in instance.pools.Values) {
            foreach (ParticleSystem p in pool) {
                p.Stop();
            }
        }
    }
}
}