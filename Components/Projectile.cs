using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;
using Paraphernalia.Components;

public class Projectile : MonoBehaviour {

    public float speed = 5;
    public float speedVariation = 0;
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
    [Range(0, 1)] public float audioSpatialBlend = 0;
    public bool screenSpacePan = false;
    public bool scaleImpactVolume = false;
    public bool shakeCamera = true;
    public string onFireParticleSystemName = "";
    public string onFinishParticleSystemName = "";
    public string onHitParticleSystemName = "";
    public string onHitSpawnName = "";
    public Color onHitColor = Color.white;
    public Rigidbody2D target;
    
    private static Camera _cam;
    private static Camera cam {
        get {
            if (_cam == null) _cam = Camera.main;
            return _cam;
        }
    }

    
    private Rigidbody body;
    private Rigidbody2D body2D;
    private Vector3 startPosition;

    public Vector3 velocity {
        get {
            if (body != null) return body.velocity;
            if (body2D != null) return (Vector3)body2D.velocity;
            return Vector3.zero;
        }
    }

    void Awake () {
        body = GetComponent<Rigidbody>();
        if (body == null) body2D = GetComponent<Rigidbody2D>();
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
        if (body != null) {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders) {
                collider.enabled = false;
            }
        }
        else {
            body2D.velocity = Vector3.zero;
            body2D.angularVelocity = 0;
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in colliders) {
                collider.enabled = false;
            }
        }
    }

    public void Fire (Vector3 direction, Vector3 gunVelocity = default(Vector3)) {
        if (transform.parent == null) ParticleManager.Play(onFireParticleSystemName, transform.position, direction, size * size);
        else ParticleManager.Play(onFireParticleSystemName, transform.position, direction, size * size, null, transform.parent);

        float pan = 0;
        if (screenSpacePan && cam != null) {
            float x = cam.WorldToViewportPoint(startPosition).x;
            pan = x * 2 - 1;
        }
        AudioManager.PlayEffect(onFireAudioClipName, audioMixerName, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f), pan, audioSpatialBlend);

        startPosition = transform.position;
        if (orientToVelocity) {
            // TODO: t.forward for RB(3D)
            transform.right = direction;
            if (direction.x * direction.y < 0) {
                transform.Rotate(Vector3.right * 180, Space.Self);
            }
        }
        gameObject.SetActive(true);
        if (particles) particles.Play();
        if (lifetime > 0) StartCoroutine("LifeCycleCoroutine");
        if (body != null) {
            body.angularVelocity = Vector3.zero;
            float s = speed + Random.Range(-speedVariation, speedVariation);
            body.velocity = direction.normalized * s + gunVelocity * (1 - gunVelocityDamping);
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders) {
                collider.enabled = true;
            }
        }
        else {
            body2D.angularVelocity = 0;
            float s = speed + Random.Range(-speedVariation, speedVariation);
            body2D.velocity = direction.normalized * s + gunVelocity * (1 - gunVelocityDamping);
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in colliders) {
                collider.enabled = true;
            }
        }
    }

    IEnumerator LifeCycleCoroutine () {
        yield return new WaitForSeconds(lifetime);
        ParticleManager.Play(onFinishParticleSystemName, gameObject.RendererBounds().center);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter (Collider collider) {
        OnHit(transform.position, (transform.position - collider.transform.position).normalized, collider.gameObject.transform);
    }

    void OnCollisionEnter (Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            OnHit(contact.point, contact.normal, collision.gameObject.transform);
            break;
        }
    }

    void OnTriggerEnter2D (Collider2D collider) {
        OnHit(transform.position, (transform.position - collider.transform.position).normalized, collider.gameObject.transform);
    }

    void OnCollisionEnter2D (Collision2D collision) {
        foreach (ContactPoint2D contact in collision.contacts) {
            OnHit(contact.point, contact.normal, collision.gameObject.transform);
            break;
        }
    }


    public void OnHit(Vector3 point, Vector3 normal, Transform t) {
        float pan = 0;
        float spatialBlend = 0;
        if (screenSpacePan && cam != null) {
            float x = cam.WorldToViewportPoint(point).x;
            pan = x * 2 - 1;
        }
        float volume = Random.Range(0.7f, 1);
        if (scaleImpactVolume) {
            float scale = velocity.magnitude / speed;
            volume *= scale * scale;
        }
        AudioManager.PlayEffect(onHitAudioClipName, audioMixerName, transform, volume, Random.Range(0.95f, 1.05f), pan, spatialBlend);
        ParticleManager.Play(onHitParticleSystemName, point, normal, size * size, onHitColor, t);
        if (!string.IsNullOrEmpty(onHitSpawnName)) {
            GameObject hitSpawn = Spawner.Spawn(onHitSpawnName);
            hitSpawn.transform.position = point;
            hitSpawn.transform.up = normal;
            hitSpawn.transform.SetParent(t);
        }
        if (dieOnHit) gameObject.SetActive(false);
        if (shakeCamera) CameraShake.MainCameraShake();
    }

    void FixedUpdate () {
        if (target != null && speed > 0) {
            if (body != null) {
                Vector3 steering = Steering.Seek(body, (Vector2)target.position, speed);
                body2D.AddForce(steering * pursuitDamping, ForceMode.VelocityChange);
                transform.forward = body2D.velocity.normalized;
            }
            else {
                Vector2 steering = Steering.Seek(body2D, (Vector2)target.position, speed);
                body2D.AddForce(steering * pursuitDamping, ForceMode.VelocityChange);
                transform.right = body2D.velocity.normalized;
            }
        }

        Vector3 diff = transform.position - startPosition;
        if (body != null) diff = body.position + body.velocity * Time.fixedDeltaTime - startPosition;
        if (body2D != null) diff = body2D.position + body2D.velocity * Time.fixedDeltaTime - (Vector2)startPosition;
        if (limitDistance && diff.sqrMagnitude > maxDistance * maxDistance) {
            transform.position = startPosition + diff.normalized * maxDistance;
            StopCoroutine("LifeCycleCoroutine");
            ParticleManager.Play(onFinishParticleSystemName, gameObject.RendererBounds().center);
            gameObject.SetActive(false);
        }

        if (dieOffScreen && !gameObject.IsVisible()) gameObject.SetActive(false);
    }
}
