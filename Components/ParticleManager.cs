using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;

public class ParticleManager : MonoBehaviour {

	public ParticleSystem[] particlePrefabs;
	[Range(0,10)] public int poolSize = 5;
	public static ParticleManager instance;

	private Dictionary<string, ParticleSystem[]> pools = new Dictionary<string, ParticleSystem[]>();
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
			pools[name] = new ParticleSystem[poolSize];
			currentIndices[name] = 0;
			for (int poolIndex = 0; poolIndex < poolSize; poolIndex++) {
				ParticleSystem particleSystem = particlePrefabs[prefabIndex].Instantiate() as ParticleSystem;
				particleSystem.transform.parent = transform;
				pools[name][poolIndex] = particleSystem;
			}
		}
	}

	public static void Play(string name, Vector3 position, Color? color = null) {
		if (instance == null || !instance.currentIndices.ContainsKey(name)) return;
		int index = instance.currentIndices[name];
		ParticleSystem particleSystem = instance.pools[name][index];
		particleSystem.transform.position = position;
		if (color != null) particleSystem.startColor = color.Value;
		particleSystem.Play();
		index = (index + 1) % instance.poolSize;
		instance.currentIndices[name] = index;
	}
}
