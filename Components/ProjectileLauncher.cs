using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class ProjectileLauncher : MonoBehaviour {

	public Projectile projectilePrefab;
	public float launchDelay = 1.5f;
	
	private float launchTime;
	private List<Projectile> projectilePool = new List<Projectile>();

	void Awake () {
		launchTime = -launchDelay;
	}

	Projectile GetNextProjectile () {
		Projectile projectile = projectilePool.Find((p) => !p.gameObject.activeSelf);
		
		if (projectile == null) {
			projectile = projectilePrefab.Instantiate() as Projectile;
			projectilePool.Add(projectile);
		}
		
		return projectile;
	}

	void Reload () {
		if (transform.childCount == 0 && Time.time - launchTime > launchDelay) {
			Projectile projectile = GetNextProjectile();
			projectile.Ready(transform);
		}
	}

	public void Shoot (Vector3 direction, Vector3 parentVelocity = default(Vector3)) {
		if (transform.childCount == 1) {
			launchTime = Time.time;
			Projectile projectile = transform.GetChild(0).gameObject.GetComponent<Projectile>() as Projectile;
			projectile.Fire(Vector2.up, parentVelocity);
		}
	}

	void Update () {
		Reload();
	}
}
