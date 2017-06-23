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
	[Range(0,10)] public int poolSize = 5;
	public static ParticleManager instance;

	private Dictionary<string, ParticleSystem> prefabs = new Dictionary<string, ParticleSystem>();
	private Dictionary<string, List<ParticleSystem>> pools = new Dictionary<string, List<ParticleSystem>>();
	private Dictionary<string, int> currentIndices = new Dictionary<string, int>();

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
			pools[name] = new List<ParticleSystem>();
			currentIndices[name] = 0;
			prefabs[name] = particlePrefabs[prefabIndex];
			for (int poolIndex = 0; poolIndex < poolSize; poolIndex++) {
				ParticleSystem particleSystem = particlePrefabs[prefabIndex].Instantiate() as ParticleSystem;
				particleSystem.transform.parent = transform;
				pools[name].Add(particleSystem);
			}
		}
	}

	public static ParticleSystem Play(string name, Transform t) {
		return Play(name, t.position, Vector3.up, 1, null, t);
	}

	public static ParticleSystem Play(string name, Transform t, Color color) {
		return Play(name, t.position, Vector3.up, 1, color, t);
	}

	public static ParticleSystem Play(string name, Vector3 position, Color? color = null) {
		return Play(name, position, Vector3.up, color);
	}

	public static ParticleSystem Play(string name, Vector3 position, Vector3 normal, Color? color = null) {
		return Play(name, position, normal, 1, color);
	}

	public static ParticleSystem Play(string name, Vector3 position, Vector3 normal, float size, Color? color = null, Transform t = null) {
		if (instance == null || !instance.currentIndices.ContainsKey(name)) return null;
		int index = instance.currentIndices[name];
		List<ParticleSystem> pool = instance.pools[name];
		pool.RemoveAll((i) => i == null);
		ParticleSystem particleSystem = pool[index];
		if (t != null) particleSystem.transform.parent = t;
		particleSystem.transform.position = position;
		particleSystem.transform.localScale = Vector3.one * size;
		particleSystem.transform.up = normal;
		ParticleSystem prefab = instance.prefabs[name];
		ParticleSystem.MainModule main = particleSystem.main;
		if (color != null) main.startColor = color.Value;
		particleSystem.Play();
		index = (index + 1) % instance.poolSize;
		instance.currentIndices[name] = index;
		return particleSystem;
	}
}
}