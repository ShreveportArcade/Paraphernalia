﻿using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class Damage : MonoBehaviour {

	public float damage = 1;
	public float multiplier = 1;
	public bool disableOnCollision = false;

	public bool affectAncestor = false;

	protected virtual float GetDamage() {
		if (disableOnCollision) gameObject.SetActive(false);
		return damage * multiplier;
	}

	protected virtual float GetDamage(Vector2 velocity, Vector2 normal) {
		return GetDamage();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collider.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage());
	}

	void OnCollisionEnter2D (Collision2D collision) {
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collision.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage(collision.relativeVelocity, collision.contacts[0].normal));
	}		
}
