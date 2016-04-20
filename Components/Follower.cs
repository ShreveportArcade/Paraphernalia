using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {

	public string defaultTag = "Player";
	public Transform _target;
	public Transform target {
		get {
			if (_target == null) {
				GameObject defaultGameObject = GameObject.FindWithTag(defaultTag);
				if (defaultGameObject != null) _target = defaultGameObject.transform;
			}
			return _target;
		}
	}

	public Vector3 position {
		get {
			if (target != null) return target.position;

			return Vector3.zero;
		}
	}
}
