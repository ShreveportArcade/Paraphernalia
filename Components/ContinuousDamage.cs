using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class ContinuousDamage : MonoBehaviour {

	public float damagePerSecond = 1;

	void OnTriggerStay2D (Collider2D collider) {
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h == null) h = collider.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(damagePerSecond * Time.deltaTime, false);
	}

	void OnCollisionStay2D (Collision2D collision) {
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h == null) h = collision.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(damagePerSecond * Time.deltaTime, false);
	}
}
