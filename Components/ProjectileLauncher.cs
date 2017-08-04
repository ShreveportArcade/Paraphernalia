using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class ProjectileLauncher : MonoBehaviour {

	public string projectileName;
	public float launchDelay = 1.5f;
	public bool showProjectileOnReady = true;

	private float launchTime;
	private Projectile projectile;

	void Awake () {
		launchTime = -launchDelay;
	}

	void Ready () {
		if (showProjectileOnReady && projectile == null && Time.time - launchTime > launchDelay) {
			projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
			projectile.Ready(transform);
		}
	}

	public bool Shoot (Vector3 direction, Vector3 parentVelocity = default(Vector3)) {
		if (Time.time - launchTime > launchDelay) {
			if (projectile == null) projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
			launchTime = Time.time;
			projectile.transform.position = transform.position;
			projectile.Fire(direction, parentVelocity);
			projectile = null;
			return true;
		}
		return false;
	}

	void Update () {
		Ready();
	}
}
