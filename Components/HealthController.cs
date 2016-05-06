using UnityEngine;
using System.Collections;
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

	public string damageSoundName;
	public string deathSoundName;
	public string destructionSoundName;
	public string deathParticlesName;
	public string destructionParticlesName;
	public string destructionSpawnName;

	public float maxHealth = 3;
	public float destructionHealth = -1;
	private float _health;
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
					AudioManager.PlayVariedEffect(deathSoundName);
					ParticleManager.Play(deathParticlesName, transform);
				}
			}
			else if (_health <= destructionHealth && prevHealth > destructionHealth) {
				PlayDestruction();
			}
			else if (_health > 0 && prevHealth <= 0) {
				AudioManager.PlayVariedEffect(deathSoundName);
				onResurection();
			}
			else if (_health < prevHealth) {
				AudioManager.PlayVariedEffect(damageSoundName);
			}
			
			if (prevHealth != _health) {
				onHealthChanged(_health, prevHealth, maxHealth);
				onAnyHealthChanged(this, _health, prevHealth, maxHealth);
			}
		}
	}

	void PlayDestruction () {
		AudioManager.PlayVariedEffect(destructionSoundName);
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
		_health = maxHealth;
	}

	public void TakeDamage(float damage, bool allowRecovery = true) {
		health -= damage;
	}

    public bool Heal(float heal, bool allowRecovery = true)
    {
        if (health >= maxHealth) return false;
        health += heal;
        return true;
    }
}
