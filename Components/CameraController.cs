/*
Copyright (C) 2014 Nolan Baker

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Utils;
using Paraphernalia.Extensions;

namespace Paraphernalia.Components {
[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

	private static CameraController _instance;
	public static CameraController instance {
		get { return _instance; }
	}

	public List<CameraZone> cameraZones = new List<CameraZone>();
	public AudioClip defaultMusic;
	public string targetTag = "Player";
	new public Camera camera;
	public Transform target;
	public bool useFixedUpdate = false;
	public Vector3 offset = -Vector3.forward;
	public float speed = 1;
	public float moveStartDist = 1f;
	public Vector3 velocityAdjustment = new Vector2(0.2f, 0);
	public bool bounded = false;
	public Bounds bounds;
	public Interpolate.EaseType easeType = Interpolate.EaseType.InOutQuad;

	void Awake () {
		if (_instance == null) { 
			_instance = this;
			SetPosition();
		}
		else {
			Debug.LogWarning("Instance of CameraController already exists. Destroying duplicate.");
			GameObjectUtils.Destroy(gameObject);
		}
	}

	void LateUpdate () {
		if (!useFixedUpdate || !Application.isPlaying) UpdatePosition();
	}

	void FixedUpdate () {
		if (useFixedUpdate) UpdatePosition();
	}

	void SetPosition () {
		GameObject go = GameObject.FindWithTag("Player");
		Collider2D[] colliders = Physics2D.OverlapPointAll(go.transform.position);

		if (target == null) target = go.transform;
		transform.position = target.position + offset;
		foreach (Collider2D collider in colliders) {
			CameraZone zone = collider.gameObject.GetComponent<CameraZone>();
			if (zone != null) {
				transform.position = zone.position;
				return;
			}
		}
		if (bounded) transform.position = camera.GetBoundedPos(bounds);
	}

	void UpdatePosition () {
		if (target != null && camera != null) {

			#if UNITY_EDITOR
			if (!Application.isPlaying) SetPosition();
			else {
			#endif
				LerpToTarget();
			#if UNITY_EDITOR
			}
			#endif
		}
		else {
			GameObject g = GameObject.FindWithTag(targetTag);
			if (g!= null) {
				target = g.transform;
				transform.position = target.position + offset;
			}
		}
	}

	void LerpToTarget () {
		if (cameraZones.Count > 0) return;

		Rigidbody2D r = target.GetComponent<Rigidbody2D>();
		Vector2 v = (r == null)? Vector2.zero: Vector2.Scale(r.velocity, (Vector2)velocityAdjustment);
		float d = Vector3.Distance(target.position, transform.position + offset);
		if (d > moveStartDist) {
			Vector3 targetPosition = Vector3.Lerp(
				transform.position,
				target.position + offset + (Vector3)v - Vector3.forward * v.magnitude * velocityAdjustment.z,
				Time.deltaTime * speed
			);
			transform.position = targetPosition;
		}

		if (bounded) transform.position = camera.GetBoundedPos(bounds);
	}

	public static void AddZone(CameraZone zone) {
		if (instance.cameraZones.Contains(zone)) return;
		instance.cameraZones.Add(zone);
		instance.StopCoroutine("TransitionToZoneCoroutine");
		instance.StartCoroutine("TransitionToZoneCoroutine");
	}

	public static void RemoveZone(CameraZone zone) {
		instance.cameraZones.Remove(zone);
		instance.StopCoroutine("TransitionToZoneCoroutine");
		if (instance.cameraZones.Count > 0) instance.StartCoroutine("TransitionToZoneCoroutine");
		else if (instance.defaultMusic != null) AudioManager.CrossfadeMusic(instance.defaultMusic, 0.5f);
		
	}

	IEnumerator TransitionToZoneCoroutine () {
		CameraZone zone = cameraZones[cameraZones.Count - 1];
		if (zone.music != null) AudioManager.CrossfadeMusic(zone.music, zone.transitionTime);
		Vector3 startPos = transform.position;
		float t = 0;
		while (t < zone.transitionTime) {
			t += Time.deltaTime;
			float frac = t / zone.transitionTime;
			transform.position = Vector3.Lerp(startPos, zone.position, frac);
			yield return new WaitForEndOfFrame();
		}
		transform.position = zone.position;
	}

	void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
}