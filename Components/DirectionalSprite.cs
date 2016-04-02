using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DirectionalSprite : DirectionalComponent {

	public Sprite leftSprite;
	public Sprite rightSprite;
	public Sprite upSprite;
	public Sprite downSprite;

	private SpriteRenderer _spriteRenderer;
	public SpriteRenderer spriteRenderer {
		get {
			if (_spriteRenderer == null) {
				_spriteRenderer = GetComponent<SpriteRenderer>();	
			}
			return _spriteRenderer;
		}
	}

	[ContextMenu("Look Left")]
	protected override void SetLeft () {
		spriteRenderer.sprite = leftSprite;
	}

	[ContextMenu("Look Right")]
	protected override void SetRight () {
		spriteRenderer.sprite = rightSprite;
	}

	[ContextMenu("Look Up")]
	protected override void SetUp () {
		spriteRenderer.sprite = upSprite;
	}

	[ContextMenu("Look Down")]
	protected override void SetDown () {
		spriteRenderer.sprite = downSprite;
	}
}
