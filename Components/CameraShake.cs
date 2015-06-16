using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

namespace Paraphernalia.Components {
public class CameraShake : MonoBehaviour {

	public Vector3 extents = new Vector3(0.1f, 0.1f, 0);
	private float magnitude;
	private float duration;

	void Awake () {
		if (transform.parent == null) {
			Debug.LogWarning("CameraShake expects an object with a parent.");
		}
		if (transform.localPosition != Vector3.zero) {
			Debug.LogWarning("CameraShake expects an the object to be at the local origin.");
		}
	}

	public void Shake (float magnitude, float duration) {
		StopCoroutine("ShakeCoroutine");
		this.magnitude = magnitude;
		this.duration = duration;
		StartCoroutine("ShakeCoroutine");
	}

	IEnumerator ShakeCoroutine () {
		float t = 0;
		while (t < duration) {
			float m = magnitude * (1 - t / duration);
			transform.localPosition = (Random.insideUnitSphere * m).ClipToExtents(extents);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime;
		}
		transform.localPosition = Vector3.zero;
	}
}
}