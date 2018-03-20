using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Damagable : MonoBehaviour {

	public HealthController healthController;
	public bool flipRandomly = true;

	[System.Serializable]
	public class DamageState {
		public float health;
		public Sprite sprite;

		public DamageState () {
			health = 1;
			sprite = null;
		}
	}

	public List<DamageState> damageStates = new List<DamageState>();

	private SpriteRenderer _spriteRenderer;
	private SpriteRenderer spriteRenderer {
		get {
			if (_spriteRenderer == null) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			return _spriteRenderer;
		}
	}

	void OnEnable () {
		if (flipRandomly && spriteRenderer != null) spriteRenderer.flipX = (Random.value < 0.5f);
		healthController.onHealthChanged += OnHealthChanged;
		damageStates.Sort((d1, d2) => d1.health.CompareTo(d2.health));
	}

	void OnDisable () {
		healthController.onHealthChanged -= OnHealthChanged;
	}

	void OnHealthChanged(float health, float prevHealth, float maxHealth) {
		if (spriteRenderer == null) return;
		foreach (DamageState state in damageStates) {
			if (health < state.health) {
				spriteRenderer.sprite = state.sprite;
				break;
			}
		}
	}
}
