using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {

	public string defaultTag = "Player";
	public Transform target;

	public Vector3 position {
		get {
			if (target == null) {
				GameObject defaultGameObject = GameObject.FindWithTag(defaultTag);
				if (defaultGameObject != null) target = defaultGameObject.transform;
			}

			if (target != null) return target.position;

			return Vector3.zero;
		}
	}
}
