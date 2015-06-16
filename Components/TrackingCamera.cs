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
using Paraphernalia.Utils;
using Paraphernalia.Extensions;

namespace Paraphernalia.Components {
[ExecuteInEditMode]
public class TrackingCamera : MonoBehaviour {

	public string targetTag = "Player";
	public Transform target;
	public Vector3 offset = -Vector3.forward;
	public float speed = 1;
	public float moveStartDist = 1f;
	public bool adjustForVelocity = true;
	public bool bounded = false;
	public Bounds bounds;
	public Interpolate.EaseType easeType = Interpolate.EaseType.InOutQuad;

	void LateUpdate () {
		if (target != null) {

			#if UNITY_EDITOR
			if (!Application.isPlaying) {
				transform.position = target.position + offset;
				if (bounded) transform.position = transform.position.ClipToBounds(bounds);
			}
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
		Vector2 v = (target.GetComponent<Rigidbody2D>() == null)? Vector2.zero: target.GetComponent<Rigidbody2D>().velocity;
		float d = Vector3.Distance(target.position, transform.position + offset);
		if (d > moveStartDist) {
			Vector3 off = adjustForVelocity? offset + (Vector3)v * Time.deltaTime : offset;
			Vector3 targetPosition = Vector3.Lerp(
				transform.position,
				target.position + off,
				Time.deltaTime * speed
			);
			
			if (bounded) {
				targetPosition = targetPosition.ClipToBounds(bounds);
			}
			
			transform.position = targetPosition;
		}
	}

	void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
}