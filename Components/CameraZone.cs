using UnityEngine;
using System.Collections;

namespace Paraphernalia.Components {
public class CameraZone : MonoBehaviour {

	public AudioClip music;
	public Vector3 offset = new Vector3(0,0,-150);
	public float transitionTime = 1;
	public MonoBehaviour[] behavioursToActivate;

	public Vector3 position {
		get {
			return transform.position + offset;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			CameraController.AddZone(this);
			foreach (MonoBehaviour b in behavioursToActivate) {
				b.enabled = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			CameraController.RemoveZone(this);
			foreach (MonoBehaviour b in behavioursToActivate) {
				b.enabled = false;
			}
		}
	}


	void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(position, 0.5f);
    }
}
}
