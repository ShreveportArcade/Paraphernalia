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
	public bool limitDistance = false;
	public float maxDistance = 4;
	[Range(0,1)] public float pursuitDamping = 0.1f;
	[Range(0,1)] public float gunVelocityDamping = 0.1f;
	public ParticleSystem particles;
	public string audioMixerName = "";
	public string onFireAudioClipName = "";
	public string onHitAudioClipName = "";
	public bool shakeCamera = true;
	public string onFireParticleSystemName = "";
	public string onFinishParticleSystemName = "";
	public string onHitParticleSystemName = "";
	public Color onHitColor = Color.white;
	public Rigidbody2D target;
	
	new private Rigidbody2D rigidbody2D;
	private Vector2 startPosition;

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
		startPosition = transform.position;
		transform.parent = Spawner.root;
		AudioManager.PlayEffect(onFireAudioClipName, audioMixerName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		ParticleManager.Play(onFireParticleSystemName, transform.position, direction, size * size);
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
		ParticleManager.Play(onFinishParticleSystemName, gameObject.RendererBounds().center);
		gameObject.SetActive(false);
	}

	void OnTriggerEnter2D (Collider2D collider) {
		// there has to be a more accurate way to do get a contact point with triggers
		Vector3 point = (transform.position + collider.transform.position) * 0.5f;
		OnHit(point, (transform.position - collider.transform.position).normalized);
	}

	void OnCollisionEnter2D (Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) {
			OnHit(contact.point, contact.normal);
			break;
		}
	}

	public void OnHit(Vector3 point, Vector3 normal) {
		StopCoroutine("LifeCycleCoroutine");
		AudioManager.PlayEffect(onHitAudioClipName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		ParticleManager.Play(onHitParticleSystemName, point, normal, size * size, onHitColor);
		gameObject.SetActive(false);
		if (shakeCamera) CameraShake.MainCameraShake();
	}

	void FixedUpdate () {
		if (target != null && speed > 0) {
			Vector2 steering = Steering.Seek(rigidbody2D, target.transform.position, speed);
			rigidbody2D.AddForce(steering * pursuitDamping, ForceMode.VelocityChange);
			transform.right = rigidbody2D.velocity.normalized;
		}

		Vector2 diff = rigidbody2D.position + rigidbody2D.velocity * Time.fixedDeltaTime - startPosition;
		if (limitDistance && diff.sqrMagnitude > maxDistance * maxDistance) {
			transform.position = startPosition + diff.normalized * maxDistance;
			StopCoroutine("LifeCycleCoroutine");
			ParticleManager.Play(onFinishParticleSystemName, gameObject.RendererBounds().center);
			gameObject.SetActive(false);
		}
	}
}
