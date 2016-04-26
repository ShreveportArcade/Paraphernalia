using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class DirectionalAnimator : DirectionalComponent {

	public bool useRightForLeft = false;

	[System.Serializable]
	public class DirectionalAnimationClip {
		public string name;
		public Sprite[] leftSprites;
		public Sprite[] rightSprites;
		public Sprite[] upSprites;
		public Sprite[] downSprites;
		public bool looping;
		public bool blocking;
		public float frameInterval;

		private Sprite[] _currentSprites;
		public Sprite[] currentSprites {
			get { 
				if (_currentSprites == null) {
					_currentSprites = downSprites;
				}
				return _currentSprites; 
			}
			set { _currentSprites = value; }
		}

		public DirectionalAnimationClip () {
			string name = "Idle";
			bool looping = true;
			bool blocking = false;
			float frameInterval = 0.3f;
		}
	}

	public List<DirectionalAnimationClip> animationClips = new List<DirectionalAnimationClip>();
	public string defaultAnimation = "Idle";
	
	private int currentFrame = 0;
	private DirectionalAnimationClip currentClip;
	private Coroutine animateCoroutine;

	private SpriteRenderer _spriteRenderer;
	public SpriteRenderer spriteRenderer {
		get {
			if (_spriteRenderer == null) {
				_spriteRenderer = GetComponent<SpriteRenderer>();	
			}
			return _spriteRenderer;
		}
	}

	void Awake () {
		currentClip = animationClips[0];
	}

	void OnEnable () {
		StartCoroutine("AnimateCoroutine");
	}

	void Start () {

	}

	public void PlayAnimation (string animationName) {
		StartCoroutine("PlayAnimationCoroutine", animationName);
	}

	IEnumerator PlayAnimationCoroutine (string animationName) {
		if (currentClip != null && currentClip.blocking && !currentClip.looping) {
			yield return animateCoroutine;
		}
		if (animateCoroutine != null) StopCoroutine(animateCoroutine);
		currentClip = animationClips.Find((clip) => clip.name == animationName);
		animateCoroutine = StartCoroutine(AnimateCoroutine());
	}

	IEnumerator AnimateCoroutine () {
		yield return new WaitForSeconds(Random.Range(0, currentClip.frameInterval));
		while (enabled) {
			if (currentClip.currentSprites == null) {
				yield return new WaitForEndOfFrame();
				continue;
			}
			currentFrame = currentFrame % currentClip.currentSprites.Length;
			spriteRenderer.sprite = currentClip.currentSprites[currentFrame];
			currentFrame++;
			yield return new WaitForSeconds(currentClip.frameInterval);
		}
	}

	[ContextMenu("Look Left")]
	protected override void SetLeft () {
		if (useRightForLeft) {
			spriteRenderer.flipX = true;
			foreach (DirectionalAnimationClip clip in animationClips) {
				clip.currentSprites = clip.rightSprites;
			}
		}
		else {
			spriteRenderer.flipX = false;
			foreach (DirectionalAnimationClip clip in animationClips) {
				clip.currentSprites = clip.leftSprites;
			}
		}
	}

	[ContextMenu("Look Right")]
	protected override void SetRight () {	
		spriteRenderer.flipX = false;
		foreach (DirectionalAnimationClip clip in animationClips) {
			clip.currentSprites = clip.rightSprites;
		}
	}

	[ContextMenu("Look Up")]
	protected override void SetUp () {
		spriteRenderer.flipX = false;
		foreach (DirectionalAnimationClip clip in animationClips) {
			clip.currentSprites = clip.upSprites;
		}
	}

	[ContextMenu("Look Down")]
	protected override void SetDown () {
		spriteRenderer.flipX = false;
		foreach (DirectionalAnimationClip clip in animationClips) {
			clip.currentSprites = clip.downSprites;
		}
	}
}
