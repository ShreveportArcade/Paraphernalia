using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

public class Damage : MonoBehaviour {

    public Vector3 force;
    public float damage = 1;
    public float multiplier = 1;
    public bool disableOnCollision = false;
    public bool affectAncestor = false;
    public List<string> ignoreTags = new List<string>();

    protected virtual float GetDamage() {
        if (disableOnCollision) gameObject.SetActive(false);
        return damage * multiplier;
    }

    protected virtual float GetDamage(Vector3 velocity, Vector3 normal) {
        return GetDamage();
    }

    void OnTriggerEnter2D (Collider2D collider) {
        HealthController h = GetHealthController(collider.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage());
            ApplyForce(collider.gameObject);
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        HealthController h = GetHealthController(collision.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage(collision.relativeVelocity, collision.contacts[0].normal));
            ApplyForce(collision.gameObject);
        }
    }

    void OnTriggerEnter (Collider collider) {
        HealthController h = GetHealthController(collider.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage());
            ApplyForce(collider.gameObject);
        }
    }

    void OnCollisionEnter (Collision collision) {
        HealthController h = GetHealthController(collision.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage(collision.relativeVelocity, collision.contacts[0].normal));
            ApplyForce(collision.gameObject);
        }
    }

    HealthController GetHealthController (GameObject g) {
        if (ignoreTags.Contains(g.tag)) return null;
        HealthController h = g.GetComponent<HealthController>();
        if (h == null && affectAncestor) h = g.GetAncestorComponent<HealthController>();
        if (h != null && !h.enabled) return null;
        return h;
    }

    void ApplyForce (GameObject g) {
        Rigidbody r = g.GetComponentInParent<Rigidbody>();
        if (r != null) r.AddForce(transform.TransformVector(force), ForceMode.Impulse);
    }
}
