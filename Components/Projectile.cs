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
	public bool dieOffScreen = false;
	public bool dieOnHit = true;
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
	
	private Rigidbody2D body;
	private Vector2 startPosition;

	void Awake () {
		body = GetComponent<Rigidbody2D>();
	}

	void OnDisable () {
		StopCoroutine("LifeCycleCoroutine");
	}

	public void Ready (Transform parent) {
		StopCoroutine("LifeCycleCoroutine");
		gameObject.SetActive(true);
		transform.SetParent(parent);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		body.velocity = Vector3.zero;
		body.angularVelocity = 0;
		Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider in colliders) {
			collider.enabled = false;
		}
	}

	public void Fire (Vector3 direction, Vector3 gunVelocity = default(Vector3)) {
		startPosition = transform.position;
		transform.SetParent(Spawner.root);
		AudioManager.PlayEffect(onFireAudioClipName, audioMixerName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		ParticleManager.Play(onFireParticleSystemName, transform.position, direction, size * size);
		if (orientToVelocity) transform.right = direction;
		gameObject.SetActive(true);
		body.angularVelocity = 0;
		body.velocity = direction.normalized * speed + gunVelocity * (1 - gunVelocityDamping);
		if (particles) particles.Play();
		if (lifetime > 0) StartCoroutine("LifeCycleCoroutine");
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
		OnHit(transform.position, (transform.position - collider.transform.position).normalized);
	}

	void OnCollisionEnter2D (Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) {
			OnHit(contact.point, contact.normal);
			break;
		}
	}

	public void OnHit(Vector3 point, Vector3 normal) {
		AudioManager.PlayEffect(onHitAudioClipName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
		ParticleManager.Play(onHitParticleSystemName, point, normal, size * size, onHitColor);
		if (dieOnHit) gameObject.SetActive(false);
		if (shakeCamera) CameraShake.MainCameraShake();
	}

	void FixedUpdate () {
		if (target != null && speed > 0) {
			Vector2 steering = Steering.Seek(body, target.transform.position, speed);
			body.AddForce(steering * pursuitDamping, ForceMode.VelocityChange);
			transform.right = body.velocity.normalized;
		}

		Vector2 diff = body.position + body.velocity * Time.fixedDeltaTime - startPosition;
		if (limitDistance && diff.sqrMagnitude > maxDistance * maxDistance) {
			transform.position = startPosition + diff.normalized * maxDistance;
			ParticleManager.Play(onFinishParticleSystemName, gameObject.RendererBounds().center);
			gameObject.SetActive(false);
		}

		if (dieOffScreen && !gameObject.IsVisible()) gameObject.SetActive(false);
	}
}
