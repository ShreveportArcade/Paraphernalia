using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class ProjectileLauncher : MonoBehaviour {

	public Projectile projectilePrefab;
	public float launchDelay = 1.5f;
	public bool showProjectileOnReady = true;
	public float kickbackForce = 1f;
	public int initialPoolSize = 10;

	private float launchTime;
	private List<Projectile> projectilePool = new List<Projectile>();

	void Awake () {
		launchTime = -launchDelay;
		for (int i = 0; i < initialPoolSize; i++) {
			Projectile projectile = projectilePrefab.Instantiate() as Projectile;
			projectile.gameObject.SetActive(false);
			projectilePool.Add(projectile);
		}
	}

	Projectile GetNextProjectile () {
		Projectile projectile = projectilePool.Find((p) => !p.gameObject.activeSelf);
		
		if (projectile == null) {
			projectile = projectilePrefab.Instantiate() as Projectile;
			projectilePool.Add(projectile);
		}
		
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
