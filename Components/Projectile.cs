using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;
using Paraphernalia.Components;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	public float speed = 5;
	public float lifetime = 3;
	public float size = 1;
	public bool orientToVelocity = true;
	[Range(0,1)] public float pursuitDamping = 0.1f;
	[Range(0,1)] public float gunVelocityDamping = 0.1f;
	public ParticleSystem particles;
	public string onFireAudioClipName = "";
	public string onHitAudioClipName = "";
	public bool shakeCamera = true;
	public string onHitParticleSystemName = "";
	public Color onHitColor = Color.white;
	public Rigidbody2D target;
	
	new private Rigidbody2D rigidbody2D;

	void Awake () {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public void Ready (Transform parent, bool activate = true) {
		StopCoroutine("LifeCycleCoroutine");
		gameObject.SetActive(activate);
		transform.parent = parent;
		transform.localScale = Vector3.one;
		transform.position = parent.position;
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider in colliders) {
			collider.enabled = false;
		}
	}

	public void Fire (Vector3 direction, Vector3 gunVelocity = default(Vector3)) {
		transform.parent = null;
		AudioManager.PlayEffect(onFireAudioClipName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		if (orientToVelocity) transform.right = direction;
		gameObject.SetActive(true);
		GetComponent<Rigidbody2D>().velocity = direction.normalized * speed + gunVelocity * (1 - gunVelocityDamping);
		if (particles) particles.Play();
		StopCoroutine("LifeCycleCoroutine");
		StartCoroutine("LifeCycleCoroutine");
		Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider in colliders) {
			collider.enabled = true;
		}
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
		ParticleManager.Play(onHitParticleSystemName, transform.position, normal, size * size, onHitColor);
		gameObject.SetActive(false);
		if (shakeCamera) CameraShake.MainCameraShake();
	}

	void Update () {
		if (target != null && speed > 0) {
			Vector2 steering = Steering.Seek(rigidbody2D, target.transform.position, speed);
			rigidbody2D.AddForce(steering * pursuitDamping, ForceMode.VelocityChange);
			transform.right = rigidbody2D.velocity.normalized;
		}
	}
}
