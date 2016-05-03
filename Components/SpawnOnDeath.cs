using UnityEngine;
using System.Collections;

public class SpawnOnDeath : MonoBehaviour {

	[Range(0,1)] public float spawnChance = 0.1f;
	
	[System.Serializable]
	public class Spawnable {
		public string name;
		public float weight;
		public int maxSpawnCount;
	}
	public Spawnable[] spawnables;

	private HealthController health;

	void Awake () {
		health = GetComponent<HealthController>();
	}

	void OnEnable () {
		health.onDeath += Spawn;
	}

	void OnDisable () {
		health.onDeath -= Spawn;
	}

	void Spawn () {
		if (Random.value > spawnChance) return;

		float sum = 0;
		foreach (Spawnable spawnable in spawnables) {
			sum += spawnable.weight;
		}

		float rand = Random.Range(0, sum);
		sum = 0;
		foreach (Spawnable spawnable in spawnables) {
			sum += spawnable.weight;
			if (rand < sum) {
				for (int i = 0; i < Random.Range(1, spawnable.maxSpawnCount); i++) {
					GameObject go = Spawner.Spawn(spawnable.name);
					go.transform.position = transform.position + (Vector3)Random.insideUnitCircle;
				}
				break;
			}
		}
	}
}
