using UnityEngine;
using System.Collections;

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
}
}
