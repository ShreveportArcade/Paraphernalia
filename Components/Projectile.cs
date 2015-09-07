using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;
using Paraphernalia.Components;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	public float speed = 5;
	[Range(0,1)] public float gunVelocityDamping = 0.1f;
	public ParticleSystem particles;
	public string onHitAudioClipName = "laserHit";
	public bool shakeCamera = true;
	public string onHitParticleSystemName = "PlasmaExplosion";
	public Color onHitColor = Color.white;
	
	public void Fire (Vector3 position, Vector3 direction, Vector3 gunVelocity = default(Vector3)) {
		transform.right = direction;
		gameObject.SetActive(true);
		transform.position = position;
		GetComponent<Rigidbody2D>().velocity = direction.normalized * speed + gunVelocity * (1 - gunVelocityDamping);
		if (particles) particles.Play();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		OnHit();
	}

	void OnCollisionEnter2D (Collision2D collision) {
		OnHit();
	}

	public void OnHit() {
		AudioManager.PlayEffect(onHitAudioClipName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		ParticleManager.Play(onHitParticleSystemName, transform.position, onHitColor);
		gameObject.SetActive(false);
		if (shakeCamera) CameraShake.MainCameraShake();
	}
}
