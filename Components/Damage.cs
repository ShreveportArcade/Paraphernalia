using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class Damage : MonoBehaviour {

	public float damage = 1;
	public float multiplier = 1;
	public bool disableOnCollision = false;
	public bool affectAncestor = false;
	public string ignoreTag;

	protected virtual float GetDamage() {
		if (disableOnCollision) gameObject.SetActive(false);
		return damage * multiplier;
	}

	protected virtual float GetDamage(Vector2 velocity, Vector2 normal) {
		return GetDamage();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (!string.IsNullOrEmpty(ignoreTag) && collider.gameObject.tag == ignoreTag) return;
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collider.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage());
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (!string.IsNullOrEmpty(ignoreTag) && collision.gameObject.tag == ignoreTag) return;
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collision.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage(collision.relativeVelocity, collision.contacts[0].normal));
	}		
}
