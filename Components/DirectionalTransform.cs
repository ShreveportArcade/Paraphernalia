using UnityEngine;
using System.Collections;

public class DirectionalTransform : DirectionalComponent {

	public Vector3 leftPosition;
	public Vector3 rightPosition;
	public Vector3 upPosition;
	public Vector3 downPosition;

	protected override void SetLeft () {
		transform.localPosition = leftPosition;
	}

	protected override void SetRight () {
		transform.localPosition = rightPosition;
	}

	protected override void SetUp () {
		transform.localPosition = upPosition;
	}

	protected override void SetDown () {
		transform.localPosition = downPosition;
	}

	void OnDrawGizmos () {
		Gizmos.matrix = transform.parent.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(leftPosition, rightPosition);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(downPosition, upPosition);
	}
}
