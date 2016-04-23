using UnityEngine;
using System.Collections;

public abstract class DirectionalComponent : MonoBehaviour {

	private Vector2 _direction = Vector2.down;
	public Vector2 direction {
		get { return _direction; }
		set {
			if (Vector2.Angle(_direction, value) > 45) {
				if (Mathf.Abs(value.x) > Mathf.Abs(value.y)) {
					if (value.x < 0) {
						_direction = Vector2.left;
						SetLeft();
					}
					else {
						_direction = Vector2.right;
						SetRight();
					}
				}
				else {
					if (value.y < 0) {
						_direction = Vector2.down;
						SetDown();
					}
					else {
						_direction = Vector2.up;
						SetUp();
					}
				}
			}
		}
	}

	[ContextMenu("Look Left")]
	protected virtual void SetLeft () {
	}

	[ContextMenu("Look Right")]
	protected virtual void SetRight () {	}

	[ContextMenu("Look Up")]
	protected virtual void SetUp () {
	}

	[ContextMenu("Look Down")]
	protected virtual void SetDown () {
	}	

}
