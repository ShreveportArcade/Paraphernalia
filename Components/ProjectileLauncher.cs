using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class ProjectileLauncher : MonoBehaviour {

	public string projectileName;
	public float launchDelay = 1.5f;
	public bool showProjectileOnReady = true;
	[Range(0,360)] public float spread = 0;
	[Range(0,1)] public float accuracy = 1;
	public int projectilesPerShot = 1;
	public int maxAmmo = 10;
	private int _ammo;
	public int ammo {
		get { return _ammo; }
		set { _ammo = Mathf.Clamp(value, 0, maxAmmo); }
	}

	private float launchTime;
	private Projectile projectile;

	void Start () {
		_ammo = maxAmmo;
		launchTime = -launchDelay;
	}

	void Ready () {
		if (showProjectileOnReady && projectile == null && Time.time - launchTime > launchDelay) {
			projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
			projectile.Ready(transform);
		}
	}

	public int Shoot (Vector3 direction, Vector3 parentVelocity = default(Vector3)) {
		if (Time.time - launchTime > launchDelay) {
			launchTime = Time.time;
			int projectileCount = (maxAmmo < 0) ? projectilesPerShot : Mathf.Min(ammo, projectilesPerShot);
			direction = Quaternion.AngleAxis(-spread * 0.5f, Vector3.forward) * direction;
			Quaternion rotation = Quaternion.AngleAxis(spread / (float)projectileCount, Vector3.forward);
			for (int i = 0; i < projectileCount; i++) {
				if (projectile == null) projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
				projectile.transform.position = transform.position;
				direction = rotation * direction;
				Vector3 fuzzedDir = Quaternion.AngleAxis(180 * Random.Range(accuracy-1, 1-accuracy), Vector3.forward) * direction;
				projectile.Fire(fuzzedDir, parentVelocity);
				projectile = null;
				ammo--;
			}
			return projectileCount;
		}
		return 0;
	}

	void Update () {
		Ready();
	}
}
