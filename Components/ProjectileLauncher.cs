using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class ProjectileLauncher : MonoBehaviour {

	public string projectileName;
	public float launchDelay = 1.5f;
	public bool showProjectileOnReady = true;
	public float kickbackForce = 1f;
	public int initialPoolSize = 10;

	private float launchTime;

	void Awake () {
		launchTime = -launchDelay;
	}

	Projectile GetNextProjectile () {
		Projectile projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
		return projectile;
	}

	void Ready () {
		Projectile[] projectiles = transform.GetChildComponents<Projectile>();
		if (projectiles.Length == 0 && Time.time - launchTime > launchDelay) {
			Projectile projectile = GetNextProjectile();
			projectile.Ready(transform, showProjectileOnReady);
		}
	}

	public bool Shoot (Vector3 direction, Vector3 parentVelocity = default(Vector3)) {
		Projectile[] projectiles = transform.GetChildComponents<Projectile>();
		if (projectiles.Length > 0) {
			launchTime = Time.time;
			Projectile projectile = projectiles[0];
			projectile.Fire(direction, parentVelocity);
			return true;
		}
		return false;
	}

	void Update () {
		Ready();
	}
}
