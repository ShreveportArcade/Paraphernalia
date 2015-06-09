using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	[Range(0,10)] public int sfxSourcesCount = 5;
	private int currentSFXSource = 0;
	private AudioSource[] sfxSources;

	private static AudioManager _instance;
	public static AudioManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType(typeof(AudioManager)) as AudioManager;
			}
			if (_instance == null) {
				GameObject audioMan = new GameObject("AudioManager");
				_instance = audioMan.AddComponent<AudioManager>();
			}
			return _instance;
		}
	}

	void Awake () {
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(_instance.gameObject);
			CreateFXPool();
		}
		else if (_instance != this) {
			Debug.LogWarning("AudioManager already initialized, destroying duplicate");
			GameObject.Destroy(this);
		}
	}

	void CreateFXPool () {
		sfxSources = new AudioSource[sfxSourcesCount];
		for (int i = 0; i < sfxSourcesCount; i++) {
			GameObject sfxSource = new GameObject("SFXSource." + i);
			sfxSource.transform.parent = transform;
			sfxSources[i] = sfxSource.AddComponent<AudioSource>();
		}
	}

	public static void PlayEffect(AudioClip clip) {
		AudioSource source = instance.sfxSources[instance.currentSFXSource];
		source.PlayOneShot(clip, 1);
		instance.currentSFXSource = (instance.currentSFXSource + 1) % instance.sfxSourcesCount;
	}
}
