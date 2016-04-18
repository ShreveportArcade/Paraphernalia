using UnityEngine;
using System.Collections;

public class ContinuousDamage : MonoBehaviour {

	public float damagePerSecond = 1;

	void OnTriggerStay2D (Collider2D collider) {
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h != null) h.TakeDamage(damagePerSecond * Time.deltaTime);
	}

	void OnCollisionStay2D (Collision2D collision) {
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h != null) h.TakeDamage(damagePerSecond * Time.deltaTime);
	}
}
