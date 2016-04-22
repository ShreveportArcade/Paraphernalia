/*
Copyright (C) 2015 Nolan Baker

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Paraphernalia.Components {
public class AudioManager : MonoBehaviour {

	public AudioClip music;
	public bool autoStartMusic = true;
	public List<AudioClip> clips = new List<AudioClip>();

	[Range(0,2)] public float sfxVolume = 1;
	[Range(0,10)] public int sfxSourcesCount = 5;
	private int currentSFXSource = 0;
	private AudioSource[] sfxSources;

	[Range(0,2)] public float musicVolume = 1;
	private int currentMusicSource = 0;
	private AudioSource[] musicSources;

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

	public float minPlayInterval = 0.01f;
	private Dictionary<int, float> lastPlayed = new Dictionary<int, float>();

	void Awake () {
		if (_instance == null) {
			_instance = this;
			if (_instance.transform.parent == null) DontDestroyOnLoad(_instance.gameObject);
			CreateSFXPool();
			CreateMusicPool();
			if (autoStartMusic) PlayMusic();
		}
		else if (_instance != this) {
			Debug.LogWarning("AudioManager already initialized, destroying duplicate");
			GameObject.Destroy(this);
		}
	}

	void CreateSFXPool () {
		sfxSources = new AudioSource[sfxSourcesCount];
		for (int i = 0; i < sfxSourcesCount; i++) {
			GameObject sfxSource = new GameObject("SFXSource." + i);
			sfxSource.transform.parent = transform;
			sfxSources[i] = sfxSource.AddComponent<AudioSource>();
		}
	}

	void CreateMusicPool () {
		musicSources = new AudioSource[2];
		for (int i = 0; i < 2; i++) {
			GameObject musicSource = new GameObject("MusicSource." + i);
			musicSource.transform.parent = transform;
			musicSources[i] = musicSource.AddComponent<AudioSource>();
			musicSources[i].loop = true;
		}
	}

	public static void PlayVariedEffect(string name) {
		PlayEffect(name, null, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f));
	}

	public static void PlayEffect(string name, Transform t = null, float volume = 1, float pitch = 1) {
		if (string.IsNullOrEmpty(name)) return;
		AudioClip clip = instance.clips.Find(c => c.name == name);
		if (clip != null) PlayEffect(clip, t, volume, pitch);
	}

	public static void PlayEffect(AudioClip clip, Transform t = null, float volume = 1, float pitch = 1) {
		int id = clip.GetInstanceID();
		if (instance.lastPlayed.ContainsKey(id) && 
			Time.time - instance.lastPlayed[id] < instance.minPlayInterval) {
			return;
		}
		instance.lastPlayed[id] = Time.time;
		AudioSource source = instance.sfxSources[instance.currentSFXSource];
		if (t != null) source.gameObject.transform.position = t.position;
		source.pitch = pitch;
		source.clip = clip;
		source.volume = volume * instance.sfxVolume;
		source.Play();
		instance.currentSFXSource = (instance.currentSFXSource + 1) % instance.sfxSourcesCount;
	}

	public static void PlayMusic (AudioClip clip) {
		AudioSource currentSource = instance.musicSources[instance.currentMusicSource];
		if (currentSource.clip == clip) return;
		currentSource.clip = clip;
		currentSource.volume = instance.musicVolume;
		currentSource.Play();
	}

	public static void PlayMusic () {
		if (instance.music != null) PlayMusic(instance.music);
	}

	public static void PauseMusic () {
		AudioSource currentSource = instance.musicSources[instance.currentMusicSource];
		if (currentSource != null) currentSource.Pause();
	}

	public static void ResumeMusic () {
		AudioSource currentSource = instance.musicSources[instance.currentMusicSource];
		if (currentSource != null) currentSource.UnPause();
	}

	public static void StopMusic () {
		AudioSource currentSource = instance.musicSources[instance.currentMusicSource];
		if (currentSource != null) currentSource.Stop();
	}

	public static void CrossfadeMusic(AudioClip clip, float fadeDuration) {
		AudioSource currentSource = instance.musicSources[instance.currentMusicSource];
		AudioSource nextSource = instance.musicSources[(instance.currentMusicSource + 1) % 2];
		if (currentSource.clip == clip || nextSource.clip == clip) return;
		instance.StopCoroutine("CrossfadeMusicCoroutine");
		nextSource.clip = clip;
		instance.StartCoroutine("CrossfadeMusicCoroutine", fadeDuration);
	}

	IEnumerator CrossfadeMusicCoroutine(float fadeDuration) {
		AudioSource sourceA = musicSources[currentMusicSource];
		currentMusicSource = (currentMusicSource + 1) % 2;
		AudioSource sourceB = musicSources[currentMusicSource];
		sourceB.Play();

		float t = 0;
		while (t < fadeDuration) {
			float frac = t / fadeDuration;
			sourceA.volume = (1 - frac) * musicVolume;
			sourceB.volume = frac * musicVolume;
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		sourceA.volume = 0;
		sourceA.Stop();
		sourceB.volume = 1 * musicVolume;
	}
}
}