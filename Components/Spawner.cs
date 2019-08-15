using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class Spawner : MonoBehaviour {

    public static Spawner instance;
    public static List<Spawner> instances = new List<Spawner>();

    public bool reparentObjects = true;
    public GameObject[] prefabs;
    public int poolStartSize = 10;

    private Dictionary<string, GameObject> prefabsDict;
    private Dictionary<string, List<GameObject>> poolsDict;

    void Awake () {
        if (instance == null) instance = this;
        if (instances.Find(s => s.name == name)) {
            Destroy(gameObject);
            return;
        }

        instances.Add(this);

        prefabsDict = new Dictionary<string, GameObject>();
        poolsDict = new Dictionary<string, List<GameObject>>();

        foreach (GameObject prefab in prefabs) {
            poolsDict[prefab.name] = new List<GameObject>();
            prefabsDict[prefab.name] = prefab;
        }

        for (int i = 0; i < poolStartSize; i++) {
            foreach (string name in prefabsDict.Keys) {
                Spawner.Spawn(name);
            }
        }
        Disable();
    }

    void OnDestroy () {
        instances.Remove(this);
    }

    public static GameObject Prefab (string name) {
        if (instance == null || string.IsNullOrEmpty(name)) return null;

        foreach (Spawner spawner in instances) {
            if (!spawner.prefabsDict.ContainsKey(name)) continue;
            return spawner.prefabsDict[name];
        }
        return null;
    }

    public static GameObject Spawn (string name, bool active = true) {
        if (instance == null || string.IsNullOrEmpty(name)) return null;

        Spawner spawner = null;
        foreach (Spawner s in instances) {
            if (s.prefabsDict.ContainsKey(name)) {
                spawner = s;
                break;
            }
        }

        if (spawner == null) return null;
        List<GameObject> pool = spawner.poolsDict[name];
        pool.RemoveAll((i) => i == null);

        GameObject g = pool.Find((i) => !i.activeSelf);
        if (g == null) {
            g = Prefab(name).Instantiate() as GameObject;
            if (spawner.reparentObjects) g.transform.SetParent(spawner.transform);
            pool.Add(g);
        }
        g.SetActive(active);

        return g;
    }

    [ContextMenu("Disable")]
    public void Disable () {
        foreach (string poolName in poolsDict.Keys) {
            Disable(poolName);
        }
    }

    public void Disable (string name) {
        List<GameObject> pool = poolsDict[name];
        for (int i = pool.Count-1; i >= 0; i--) {
            GameObject g = pool[i];
            if (g == null) pool.RemoveAt(i);
            else g.SetActive(false);
        }
    }

    public static void DisableAll() {
        foreach (Spawner spawner in instances) {
            spawner.Disable();
        }
    }

    public static void DisableAll(string name) {
        if (instance == null || string.IsNullOrEmpty(name)) return;

        foreach (Spawner spawner in instances) {
            if (!spawner.poolsDict.ContainsKey(name)) continue;
            spawner.Disable(name);
            break;
        }
    }
}
