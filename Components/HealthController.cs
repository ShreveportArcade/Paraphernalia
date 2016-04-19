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
	public event OnLifeChangeEvent onResurection = delegate {};

	public AudioClip deathSound;
	public string particlesName;
	public bool disableOnDeath = false;

	public float maxHealth = 3;
	private float _health = 3;
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
			else if (_health < 0) {
				_health = 0;
			}

			if (_health == 0 && prevHealth > 0) {
				if (deathSound != null) AudioManager.PlayEffect(deathSound);
				if (!string.IsNullOrEmpty(particlesName)) ParticleManager.Play(particlesName, transform.position);
				if (disableOnDeath) gameObject.SetActive(false);
				onAnyDeath(this);
				onDeath();
			}
			else if (_health > 0 && prevHealth == 0) {
				onResurection();
			}
			
			if (prevHealth != _health) {
				onHealthChanged(_health, prevHealth, maxHealth);
				onAnyHealthChanged(this, _health, prevHealth, maxHealth);
			}
		}
	}

	public bool isDead {
		get { return health <= 0; }
	}
	
	void Start() {
		health = maxHealth;
	}

	public void TakeDamage(float damage, bool allowRecovery = true) {
		health -= damage;
	}
}
