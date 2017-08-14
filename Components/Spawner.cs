using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class Spawner : MonoBehaviour {

	public static Spawner instance;
	public static Transform root {
		get {
			if (instance == null) return null;
			return instance.gameObject.transform;
		}
	}

	public bool reparentObjects = true;
	public GameObject[] prefabs;

	private Dictionary<string, GameObject> prefabsDict;
	private Dictionary<string, List<GameObject>> poolsDict;

	void Awake () {
		if (instance == null) {
			instance = this;
			prefabsDict = new Dictionary<string, GameObject>();
			poolsDict = new Dictionary<string, List<GameObject>>();

			foreach (GameObject prefab in prefabs) {
				poolsDict[prefab.name] = new List<GameObject>();
				prefabsDict[prefab.name] = prefab;
			}
		}
	}

	public static GameObject Spawn(string name, bool active = true) {
		if (instance == null || 
			string.IsNullOrEmpty(name) ||
			!instance.poolsDict.ContainsKey(name) ||
			!instance.prefabsDict.ContainsKey(name)) {
			
			return null;
		}


		List<GameObject> pool = instance.poolsDict[name];
		pool.RemoveAll((i) => i == null);
		GameObject g = pool.Find((i) => !i.activeSelf);
		if (g == null) {
			g = instance.prefabsDict[name].Instantiate() as GameObject;
			if (instance.reparentObjects) g.transform.SetParent(instance.transform);
			pool.Add(g);
		}
		
		g.SetActive(active);
		
		return g;
	}
}
