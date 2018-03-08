using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

namespace Paraphernalia.Utils {
public static class Steering {
    
    public static Vector3 Wander (Vector3 velocity, float maxAngle) {
        float r = velocity.magnitude * Mathf.Tan(maxAngle * Mathf.Deg2Rad);
        return velocity + Random.insideUnitSphere * r;
    }

    // Seek
    public static Vector3 Seek (Vector3 position, Vector3 velocity, Vector3 targetPos, float maxSpeed) {
        Vector3 desiredVel = (targetPos - position).normalized * maxSpeed;
        return desiredVel - velocity;
    }

    public static Vector2 Seek (Rigidbody2D r, Vector2 targetPos, float maxSpeed) {
        return (Vector2)Seek(r.transform.position, r.velocity, targetPos, maxSpeed);
    }

    public static Vector3 Seek (Rigidbody r, Vector3 targetPos, float maxSpeed) {
        return Seek(r.transform.position, r.velocity, targetPos, maxSpeed);
    }

    // Flee
    public static Vector3 Flee (Vector3 position, Vector3 velocity, Vector3 targetPos, float maxSpeed) {
        Vector3 desiredVel = (position - targetPos).normalized * maxSpeed;
        return desiredVel - velocity;
    }

    public static Vector2 Flee (Rigidbody2D r, Vector2 targetPos, float maxSpeed) {
        return (Vector2)Flee(r.transform.position, r.velocity, targetPos, maxSpeed);
    }

    public static Vector3 Flee (Rigidbody r, Vector3 targetPos, float maxSpeed) {
        return Flee(r.transform.position, r.velocity, targetPos, maxSpeed);
    }

    // Arrive
    public static Vector3 Arrive (Vector3 position, Vector3 velocity, Vector3 targetPos, float maxSpeed, float slowRad) {
        Vector3 desiredVel = (targetPos - position).normalized * maxSpeed;
        float dist = (targetPos - position).magnitude;
        if (dist < slowRad) desiredVel *= (dist / slowRad);		
        return desiredVel - velocity;
    }

    public static Vector2 Arrive (Rigidbody2D r, Vector2 targetPos, float maxSpeed, float slowRad) {
        return (Vector2)Arrive(r.transform.position, r.velocity, targetPos, maxSpeed, slowRad);
    }

    public static Vector3 Arrive (Rigidbody r, Vector3 targetPos, float maxSpeed, float slowRad) {
        return Arrive(r.transform.position, r.velocity, targetPos, maxSpeed, slowRad);
    }

    // Pursue
    public static Vector3 Pursue (Vector3 position, Vector3 velocity, Vector3 targetPos, Vector3 targetVel, float maxSpeed) {
        float dist = (targetPos - position).magnitude;
        Vector3 futurePos = targetPos + targetVel * dist / maxSpeed;
        return Seek(position, velocity, futurePos, maxSpeed);
    }

    public static Vector2 Pursue (Rigidbody2D r, Rigidbody2D target, float maxSpeed) {
        return (Vector2)Pursue(r.transform.position, r.velocity, target.transform.position, target.velocity, maxSpeed);
    }

    public static Vector3 Pursue (Rigidbody r, Rigidbody target, float maxSpeed) {
        return Pursue(r.transform.position, r.velocity, target.transform.position, target.velocity, maxSpeed);
    }

    // Evade
    public static Vector3 Evade (Vector3 position, Vector3 velocity, Vector3 targetPos, Vector3 targetVel, float maxSpeed) {
        float dist = (targetPos - position).magnitude;
        Vector3 futurePos = targetPos + targetVel * dist / maxSpeed;
        return Flee(position, velocity, futurePos, maxSpeed);
    }

    public static Vector2 Evade (Rigidbody2D r, Rigidbody2D target, float maxSpeed) {
        return (Vector2)Evade(r.transform.position, r.velocity, target.transform.position, target.velocity, maxSpeed);
    }

    public static Vector3 Evade (Rigidbody r, Rigidbody target, float maxSpeed) {
        return Evade(r.transform.position, r.velocity, target.transform.position, target.velocity, maxSpeed);
    }

    // Utils
    public static Vector2 ObstacleSweep2D (Vector2 origin, float radius, Vector2 dir, float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers, int deg = 15) {
        bool defaultBehaviour = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = true;
        for (int i = 0; i < 180; i += deg) {
            for (int j = -1; j < 2; j += 2) {
                Vector2 newDir = dir.normalized.RotatedByDeg((float)(i * j));
                RaycastHit2D hit = Physics2D.CircleCast(origin, radius, newDir, distance, layerMask);

                if (hit.collider == null) {
                    Physics2D.queriesHitTriggers = defaultBehaviour;
                    return newDir * dir.magnitude;
                }
            }
        }
        Physics2D.queriesHitTriggers = defaultBehaviour;
        return Vector2.zero;
    }
}
}
