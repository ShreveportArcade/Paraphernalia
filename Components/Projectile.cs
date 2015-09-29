using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;
using Paraphernalia.Components;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	public float speed = 5;
	public float lifetime = 3;
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
		StopCoroutine("LifeCycleCoroutine");
		StartCoroutine("LifeCycleCoroutine");
	}

	IEnumerator LifeCycleCoroutine () {
		yield return new WaitForSeconds(lifetime);
		gameObject.SetActive(false);
	}

	void OnTriggerEnter2D (Collider2D collider) {
		OnHit((transform.position - collider.transform.position).normalized);
	}

	void OnCollisionEnter2D (Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) {
			OnHit(contact.normal);
			break;
		}
	}

	public void OnHit(Vector3 normal) {
		StopCoroutine("LifeCycleCoroutine");
		AudioManager.PlayEffect(onHitAudioClipName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		ParticleManager.Play(onHitParticleSystemName, transform.position, normal, onHitColor);
		gameObject.SetActive(false);
		if (shakeCamera) CameraShake.MainCameraShake();
	}
}
