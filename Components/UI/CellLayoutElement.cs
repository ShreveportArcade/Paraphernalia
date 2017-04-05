using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[ExecuteInEditMode]
public class CellLayoutElement : UIBehaviour {

	public Vector2 sizeScale = Vector2.one;
	public Vector2 offset = Vector2.zero;

	void Update () {
		LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
	}
}
