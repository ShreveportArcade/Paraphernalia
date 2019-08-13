using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;
using Paraphernalia.Components;

public class Damage : MonoBehaviour {

    public Vector3 force;
    public float outwardForce;
    public float damage = 1;
    public float multiplier = 1;
    public bool disableOnCollision = false;
    public bool affectAncestor = false;
    public bool allowRecovery = true;
    public string hitSoundName;
    public List<string> ignoreTags = new List<string>();

    protected virtual float GetDamage() {
        if (disableOnCollision) gameObject.SetActive(false);
        return damage * multiplier;
    }

    protected virtual float GetDamage(Vector3 velocity, Vector3 normal) {
        return GetDamage();
    }

    void OnTriggerEnter2D (Collider2D collider) {
        if (!enabled) return;
        HealthController h = GetHealthController(collider.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage(), allowRecovery);
            HitObject(collider.gameObject);
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (!enabled) return;
        HealthController h = GetHealthController(collision.collider.gameObject);
        if (h != null) {
            Vector3 norm = Vector3.up;
            if (collision.contacts.Length > 0) norm = collision.contacts[0].normal;
            h.TakeDamage(GetDamage(collision.relativeVelocity, norm), allowRecovery);
            HitObject(collision.collider.gameObject);
        }
    }

    void OnTriggerEnter (Collider collider) {
        if (!enabled) return;
        HealthController h = GetHealthController(collider.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage(), allowRecovery);
            HitObject(collider.gameObject);
        }
    }

    void OnCollisionEnter (Collision collision) {
        if (!enabled) return;
        HealthController h = GetHealthController(collision.collider.gameObject);
        if (h != null) {
            h.TakeDamage(GetDamage(collision.relativeVelocity, collision.contacts[0].normal), allowRecovery);
        }
        HitObject(collision.collider.gameObject);
    }

    HealthController GetHealthController (GameObject g) {
        if (ignoreTags.Contains(g.tag)) return null;
        HealthController h = g.GetComponent<HealthController>();
        if (h == null && affectAncestor) h = g.GetAncestorComponent<HealthController>();
        if (h != null) {
            if (transform.IsChildOf(h.transform)) return null;
            if (!h.enabled) return null;
            if (h.ignoreTags.Contains(tag)) return null;
        }
        return h;
    }

    void HitObject (GameObject g) {
        Vector3 dir = (g.RendererBounds().center - gameObject.RendererBounds().center);
        Vector3 f = transform.TransformVector(force) + dir * outwardForce;
        Rigidbody r = g.GetComponentInParent<Rigidbody>();
        if (r != null) r.AddForce(f, ForceMode.Impulse);
        else {
            Rigidbody2D r2D = g.GetComponentInParent<Rigidbody2D>();
            if (r2D != null) r2D.AddForce(f, ForceMode.Impulse);
        }
        AudioManager.PlayVariedEffect(hitSoundName);
    }
}
