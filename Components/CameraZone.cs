using UnityEngine;
using System.Collections;

namespace Paraphernalia.Components {
public class CameraZone : MonoBehaviour {

    public AudioClip music;
    public Vector3 offset = new Vector3(0,0,-150);
    public Vector3 axisLock = Vector3.one;
    public float transitionTime = 1;
    public bool useUnscaledTime = false;
    public MonoBehaviour[] behavioursToActivate;

    public Vector3 position {
        get {
            return transform.position + offset;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        CameraController cam = CameraController.CameraControllerFromTarget(collider.gameObject);
        if (cam != null) AddRemoveZone(true, cam);
    }

    void OnTriggerExit2D(Collider2D collider) {
        CameraController cam = CameraController.CameraControllerFromTarget(collider.gameObject);
        if (cam != null) AddRemoveZone(false, cam);
    }

    void OnTriggerEnter(Collider collider) {
    CameraController cam = CameraController.CameraControllerFromTarget(collider.gameObject);
        if (cam != null) AddRemoveZone(true, cam);    }

    void OnTriggerExit(Collider collider) {
        CameraController cam = CameraController.CameraControllerFromTarget(collider.gameObject);
        if (cam != null) AddRemoveZone(false, cam);
    }

    void AddRemoveZone (bool add, CameraController cam) {
        if (add) cam.AddZone(this);
        else cam.RemoveZone(this);
        foreach (MonoBehaviour b in behavioursToActivate) {
            b.enabled = add;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(position + Vector3.up, position - Vector3.up);
        Gizmos.DrawLine(position + Vector3.right, position - Vector3.right);
    }
}
}
