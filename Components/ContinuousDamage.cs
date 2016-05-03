using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class ContinuousDamage : MonoBehaviour {

	public float damagePerSecond = 1;
	public bool affectAncestor = false;

	void OnTriggerStay2D (Collider2D collider) {
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collider.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(damagePerSecond * Time.deltaTime, false);
	}

	void OnCollisionStay2D (Collision2D collision) {
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h == null && affectAncestor) h = collision.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(damagePerSecond * Time.deltaTime, false);
	}
}
