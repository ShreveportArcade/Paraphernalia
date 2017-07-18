using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class Damage : MonoBehaviour {

	public float damage = 1;
	public float multiplier = 1;
	public bool disableOnCollision = false;
	public bool affectAncestor = false;
	public List<string> ignoreTags = new List<string>();

	protected virtual float GetDamage() {
		if (disableOnCollision) gameObject.SetActive(false);
		return damage * multiplier;
	}

	protected virtual float GetDamage(Vector2 velocity, Vector2 normal) {
		return GetDamage();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (ignoreTags.Contains(collider.gameObject.tag)) return;
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collider.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage());
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (ignoreTags.Contains(collision.gameObject.tag)) return;
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collision.gameObject.GetAncestorComponent<HealthController>();
		if (h != null && h.enabled) h.TakeDamage(GetDamage(collision.relativeVelocity, collision.contacts[0].normal));
	}		
}
