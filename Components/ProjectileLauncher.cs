using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class ProjectileLauncher : MonoBehaviour {

    public string projectileName;
    
    public float launchDelay = 1.5f;
    public bool showProjectileOnReady = false;
    [Range(0,360)] public float spread = 0;
    [Range(0,1)] public float accuracy = 1;
    public int projectilesPerShot = 1;
    [Tooltip("per second")] public float ammoRecoveryRate = 0;
    
    [Tooltip("-1 = infinite ammo")] public int maxAmmo = -1;
    private int _ammo;
    public int ammo {
        get { 
            if (maxAmmo < 1) return int.MaxValue;
            return _ammo; 
        }
        set { 
            if (maxAmmo > 0) _ammo = Mathf.Clamp(value, 0, maxAmmo); 
            else if (value < 0) _ammo = 0;
            else _ammo = value;	
        }
    }

    public string animTriggerName = "shoot"; 
    public Animator anim;
    
    private float launchTime;
    private Projectile projectile;

    void Start () {
        if (maxAmmo > 0) _ammo = maxAmmo;
        launchTime = -launchDelay;
    }

    void Ready () {
        if (showProjectileOnReady && projectile == null && Time.time - launchTime > launchDelay) {
            projectile = Spawner.Spawn(projectileName).GetComponent<Projectile>();
            projectile.Ready(transform);
        }
    }

    public int Shoot (Vector3 direction, Vector3 parentVelocity = default(Vector3)) {
        if (!isActiveAndEnabled) return 0;
        if (Time.time - launchTime > launchDelay) {
            launchTime = Time.time;
            int projectileCount = Mathf.Min(ammo, projectilesPerShot);
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
            if (projectileCount > 0 && anim != null) anim.SetTrigger(animTriggerName);
            return projectileCount;
        }
        return 0;
    }

    float ammoRecovered = 0;
    void Update () {
        if (ammoRecoveryRate > 0) {
            ammoRecovered += ammoRecoveryRate * Time.deltaTime;
            ammo += Mathf.FloorToInt(ammoRecovered);
            ammoRecovered %= 1f;
        }
        Ready();
    }
}
