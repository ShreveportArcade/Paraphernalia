using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour {

	public float damage = 1;

	void OnTriggerEnter2D (Collider2D collider) {
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h != null) h.TakeDamage(damage);
	}

	void OnCollisionEnter2D (Collision2D collision) {
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h != null) h.TakeDamage(damage);
	}	
}
