using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Components;
using Paraphernalia.Utils;

public class HealthController : MonoBehaviour {

    public delegate void OnHealthChanged(float health, float prevHealth, float maxHealth);
    public event OnHealthChanged onHealthChanged = delegate {};

    public delegate void OnAnyHealthChanged(HealthController healthController, float health, float prevHealth, float maxHealth);
    public static event OnAnyHealthChanged onAnyHealthChanged = delegate {};

    public delegate void OnAnyLifeChangeEvent(HealthController controller);
    public static event OnAnyLifeChangeEvent onAnyDeath = delegate {};

    public delegate void OnLifeChangeEvent();
    public event OnLifeChangeEvent onDeath = delegate {};
    public event OnLifeChangeEvent onDestruction = delegate {};
    public event OnLifeChangeEvent onResurection = delegate {};

    public List<string> ignoreTags = new List<string>();

    public string damageSoundName;
    public string deathSoundName;
    public string resurectionSoundName;
    public string destructionSoundName;

    public string damageParticlesName;
    public string deathParticlesName;
    public string destructionParticlesName;

    public string destructionSpawnName;

    public Animator anim;
    public string damageTriggerName;
    public string deathTriggerName;
    public string resurectionTriggerName;

    [Range(0,1)] public float audioSpatialBlend = 0;

    public float recoveryTime = 3;
    private bool _isRecovering = false;
    public bool isRecovering {
        get { return _isRecovering; }
    }

    public bool spawnInvincible = false;
    public bool healOnEnable = false;
    public float _maxHealth = 3;
    public float maxHealth {
        get { return _maxHealth; }
        set {
            if (value != _maxHealth) {
                float prevMax = _maxHealth;
                _maxHealth = value;
                float diff = _maxHealth - prevMax;
                if (health < _maxHealth) health += diff;
                else if (health > _maxHealth) health = maxHealth;
            }
        }
    }

    public float healthPct {
        get { return Mathf.Clamp01(health / maxHealth); }
    }
    
    public float destructionHealth = -1;
    private float _health = -1;
    public float health {
        get {
            return _health;
        }
        set {
            float prevHealth = _health;
            _health = value;
            if (_health > maxHealth) {
                _health = maxHealth;
            }

            if (_health <= 0 && prevHealth > 0) {
                onAnyDeath(this);
                onDeath();
                if (_health <= destructionHealth && prevHealth > destructionHealth) {
                    PlayDestruction();
                }
                else {
                    AudioManager.PlayEffect(deathSoundName, null, transform, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f), 0, audioSpatialBlend);
                    ParticleManager.Play(deathParticlesName, transform);
                    TriggerAnimation(deathTriggerName);
                }
            }
            else if (_health <= destructionHealth && prevHealth > destructionHealth) {
                PlayDestruction();
            }
            else if (_health > 0 && prevHealth <= 0) {
                AudioManager.PlayEffect(resurectionSoundName, null, transform, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f), 0, audioSpatialBlend);
                onResurection();
                TriggerAnimation(resurectionTriggerName);
            }
            else if (_health < prevHealth && _health > 0) {
                AudioManager.PlayEffect(damageSoundName, null, transform, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f), 0, audioSpatialBlend);
                ParticleManager.Play(damageParticlesName, transform);
                TriggerAnimation(damageTriggerName);
            }
            else if (_health < prevHealth && _health <= 0) {
                AudioManager.PlayEffect(damageSoundName, null, transform, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f), 0, audioSpatialBlend);
                ParticleManager.Play(damageParticlesName, transform);
            }
            
            if (prevHealth != _health) {
                onHealthChanged(_health, prevHealth, maxHealth);
                onAnyHealthChanged(this, _health, prevHealth, maxHealth);
            }
        }
    }

    void PlayDestruction () {
        AudioManager.PlayEffect(destructionSoundName, null, transform, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f), 0, audioSpatialBlend);
        ParticleManager.Play(destructionParticlesName, transform.position);
        GameObject destructionSpawn = Spawner.Spawn(destructionSpawnName);
        if (destructionSpawn != null) {
            destructionSpawn.transform.position = transform.position;
            destructionSpawn.transform.rotation = transform.rotation;
        }
        gameObject.SetActive(false);
        onDestruction();
    }

    public bool isDead {
        get { return health <= 0; }
    }
    
    void Start() {
        health = maxHealth;
        if (anim == null) anim = GetComponentInParent<Animator>();
    }

    void OnEnable () {
        if (healOnEnable) health = maxHealth;
        if (spawnInvincible) StartCoroutine("Recover");
        else _isRecovering = false;
    }

    public void TakeDamage(float damage, bool allowRecovery = true) {
        if (!enabled || isRecovering) return;
        if (allowRecovery && recoveryTime > 0.001f && gameObject.activeInHierarchy) StartCoroutine("Recover");
        health -= damage;
    }

    IEnumerator Recover () {
        _isRecovering = true;
        yield return new WaitForSeconds(recoveryTime);
        _isRecovering = false;
    }

    void TriggerAnimation(string triggerName) {
        if (anim == null || string.IsNullOrEmpty(triggerName)) return;
        anim.SetTrigger(triggerName);
    }
}
