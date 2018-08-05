using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

namespace Paraphernalia.Components {
public class CameraShake : MonoBehaviour {

	public Vector3 extents = new Vector3(1,1,0);
	public float magnitude = 1;
	public float duration = 1;

	void Awake () {
		if (transform.parent == null) {
			Debug.LogWarning("CameraShake expects an object with a parent.");
		}
		if (transform.localPosition != Vector3.zero) {
			Debug.LogWarning("CameraShake expects an the object to be at the local origin.");
		}
	}

	public static void MainCameraShake () {
		if (Camera.main == null) return;
		CameraShake s = Camera.main.gameObject.GetOrAddComponent<CameraShake>();
		s.Shake();
	}

	public static void MainCameraShake (float magnitude, float duration) {
		CameraShake s = Camera.main.gameObject.GetOrAddComponent<CameraShake>();
		s.Shake(magnitude, duration);
	}

	[ContextMenu("Shake")]
	public void Shake () {
		StopCoroutine("ShakeCoroutine");
		StartCoroutine("ShakeCoroutine");
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
			t += Time.unscaledDeltaTime;
		}
		transform.localPosition = Vector3.zero;
	}
}
}