using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;

public class ParticleManager : MonoBehaviour {

	public ParticleSystem[] particlePrefabs;
	[Range(0,10)] public int poolSize = 5;
	public static ParticleManager instance;

	private Dictionary<string, ParticleSystem> prefabs = new Dictionary<string, ParticleSystem>();
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
			prefabs[name] = particlePrefabs[prefabIndex];
			for (int poolIndex = 0; poolIndex < poolSize; poolIndex++) {
				ParticleSystem particleSystem = particlePrefabs[prefabIndex].Instantiate() as ParticleSystem;
				particleSystem.transform.parent = transform;
				pools[name][poolIndex] = particleSystem;
			}
		}
	}

	public static void Play(string name, Vector3 position, Color? color = null) {
		Play(name, position, Vector3.up, color);
	}

	public static void Play(string name, Vector3 position, Vector3 normal, Color? color = null) {
		Play(name, position, normal, 1, color);
	}

	public static void Play(string name, Vector3 position, Vector3 normal, float size, Color? color = null) {
		if (instance == null || !instance.currentIndices.ContainsKey(name)) return;
		int index = instance.currentIndices[name];
		ParticleSystem particleSystem = instance.pools[name][index];
		particleSystem.transform.position = position;
		particleSystem.transform.up = normal;
		// particleSystem.transform.localScale = Vector3.one * size; // This didn't work.
		ParticleSystem prefab = instance.prefabs[name]; // THIS IS PROBABLY DUMB AND EXPENSIVE
		ScaleParticleSystem(particleSystem, prefab, size);
		if (color != null) particleSystem.startColor = color.Value;
		particleSystem.Play();
		index = (index + 1) % instance.poolSize;
		instance.currentIndices[name] = index;
	}

	public static void ScaleParticleSystem(ParticleSystem s, ParticleSystem p, float size) {
		s.startSize = p.startSize * size;
		s.startSpeed = p.startSpeed * size;
		for (int i = 0; i < p.transform.childCount; i++) {
            GameObject pChild = p.transform.GetChild(i).gameObject;
            ParticleSystem pChildSys = pChild.GetComponent<ParticleSystem>();
            if (pChildSys == null) continue;

            GameObject sChild = s.transform.GetChild(i).gameObject;
			ParticleSystem sChildSys = sChild.GetComponent<ParticleSystem>();
            if (sChildSys == null) {
            	Debug.LogWarning("ParticleManager: Instance of prefab particle system is no longer like parent.");
            	continue;
            }
            sChildSys.startSize = pChildSys.startSize * size;
            sChildSys.startSpeed = pChildSys.startSpeed * size;
        }
		
	}
}
