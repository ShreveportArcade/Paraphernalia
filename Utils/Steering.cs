using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

namespace Paraphernalia.Utils {
public static class Steering {
	
	public static Vector3 Wander (Vector3 velocity, float maxAngle) {
		float r = velocity.magnitude * Mathf.Tan(maxAngle * Mathf.Deg2Rad);
		return velocity + Random.insideUnitSphere * r;
	}

	public static Vector3 Seek (Vector3 position, Vector3 velocity, Vector3 targetPos, float maxSpeed) {
		Vector3 desiredVel = (targetPos - position).normalized * maxSpeed;
		return desiredVel - velocity;
	}

	public static Vector3 Flee (Vector3 position, Vector3 velocity, Vector3 targetPos, float maxSpeed) {
		Vector3 desiredVel = (position - targetPos).normalized * maxSpeed;
		return desiredVel - velocity;
	}

	public static Vector3 Arrive (Vector3 position, Vector3 velocity, Vector3 targetPos, float maxSpeed, float slowRad) {
		Vector3 desiredVel = (targetPos - position).normalized * maxSpeed;
		float dist = (targetPos - position).magnitude;
		if (dist < slowRad) desiredVel *= (dist / slowRad);		
		return desiredVel - velocity;
	}

	public static Vector3 Pursue (Vector3 position, Vector3 velocity, Vector3 targetPos, Vector3 targetVel, float maxSpeed) {
		float dist = (targetPos - position).magnitude;
		Vector3 futurePos = targetPos + targetVel * dist / maxSpeed;
		return Seek(position, velocity, futurePos, maxSpeed);
	}

	public static Vector3 Evade (Vector3 position, Vector3 velocity, Vector3 targetPos, Vector3 targetVel, float maxSpeed) {
		float dist = (targetPos - position).magnitude;
		Vector3 futurePos = targetPos + targetVel * dist / maxSpeed;
		return Flee(position, velocity, futurePos, maxSpeed);

	}

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
