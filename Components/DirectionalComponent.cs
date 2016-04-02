using UnityEngine;
using System.Collections;

public abstract class DirectionalComponent : MonoBehaviour {

	public void SetDirection (Vector2 dir) {
		if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
			if (dir.x < 0) SetLeft();
			else SetRight();
		}
		else {
			if (dir.y < 0) SetDown();
			else SetUp();
		}
	}

	protected abstract void SetLeft();
	protected abstract void SetRight();
	protected abstract void SetUp();
	protected abstract void SetDown();	
}
