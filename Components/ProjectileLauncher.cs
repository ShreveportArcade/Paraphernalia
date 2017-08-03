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
		if ((projectile == null || projectile.transform.parent != transform) 
			&& Time.time - launchTime > launchDelay) {
			projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
			projectile.Ready(transform, showProjectileOnReady);
		}
	}

	public bool Shoot (Vector3 direction, Vector3 parentVelocity = default(Vector3)) {
		if (projectile != null) {
			launchTime = Time.time;
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
