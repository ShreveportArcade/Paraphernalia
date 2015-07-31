using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;
using Paraphernalia.Components;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	public float speed = 5;
	public ParticleSystem particles;
	public AudioClip explosionSound;
	public string onHitParticleSystemName = "PlasmaExplosion";

	public void Fire (Vector3 position, Vector3 direction) {
		gameObject.SetActive(true);
		transform.position = position;
		GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
		particles.Play();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		OnHit();
	}

	void OnCollisionEnter2D (Collision2D collision) {
		OnHit();
	}

	void OnHit() {
		AudioManager.PlayEffect(explosionSound);
		ParticleManager.Play(onHitParticleSystemName, transform.position);
		gameObject.SetActive(false);
		CameraShake.MainCameraShake();
	}
}
