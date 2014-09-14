using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class VerticalSpriteOrdering : MonoBehaviour {

	public float orderMultiplier = 100;
	public int orderOffset = 0;
	public bool isStatic = true;

	[SerializeField, HideInInspector]private SpriteRenderer _spriteRenderer;
	public SpriteRenderer spriteRenderer {
		get {
			if (_spriteRenderer == null) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			return _spriteRenderer;
		}
	}
	
	void Start () {
		UpdateSortOrder();
	}

	#if UNITY_EDITOR
	void Update () {
		if (!Application.isPlaying) UpdateSortOrder();
	}
	#endif

	void OnEnable () {
		if (!isStatic) StartCoroutine("SortCoroutine");
	}

	IEnumerator SortCoroutine () {
		while (enabled && !isStatic) {
			UpdateSortOrder();
			yield return new WaitForEndOfFrame();
		}
	}

	[ContextMenu("Update Sort Order")]
	public void UpdateSortOrder () {
		spriteRenderer.sortingOrder = (int)(-transform.position.y * orderMultiplier) + orderOffset;
	}
}
