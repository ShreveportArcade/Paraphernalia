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
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace Paraphernalia.Components {
public class AudioManager : MonoBehaviour {

	public AudioClip music;
	public bool autoStartMusic = true;
	public List<AudioClip> clips = new List<AudioClip>();
	public List<AudioMixerGroup> mixers = new List<AudioMixerGroup>();
	public AudioMixerGroup musicMixer;
	public AudioMixerGroup defaultSFXMixer;

	[Range(0,10)] public int sfxSourcesCount = 5;
	private int currentSFXSource = 0;
	private AudioSource[] sfxSources;

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

	public static AudioSource currentSource {
		get {
			return instance.musicSources[instance.currentMusicSource];
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
			GameObject go = new GameObject("SFXSource." + i);
			go.transform.parent = transform;
			AudioSource sfxSource = go.AddComponent<AudioSource>();
			sfxSources[i] = sfxSource;
		}
	}

	void CreateMusicPool () {
		musicSources = new AudioSource[2];
		for (int i = 0; i < 2; i++) {
			GameObject go = new GameObject("MusicSource." + i);
			go.transform.parent = transform;
			AudioSource musicSource = go.AddComponent<AudioSource>();
			musicSource.loop = true;
			musicSource.outputAudioMixerGroup = musicMixer;
			musicSources[i] = musicSource;
		}
	}

	public static void PlayVariedEffect(string name) {
		PlayEffect(name, null, Random.Range(0.9f,1.1f), Random.Range(0.9f,1.1f));
	}

	public static void PlayEffect(string name, Transform t = null, float volume = 1, float pitch = 1) {
		if (string.IsNullOrEmpty(name)) return;
		AudioClip clip = instance.clips.Find(c => c.name == name);
		PlayEffect(clip, t, volume, pitch);
	}

	public static void PlayEffect(AudioClip clip, Transform t = null, float volume = 1, float pitch = 1) {
		PlayEffect(clip, instance.defaultSFXMixer, t, volume, pitch);
	}

	public static void PlayEffect(string name, string mixerName, Transform t = null, float volume = 1, float pitch = 1, float pan = 0, float spatialBlend = 0, float minDist = 1, float maxDist = 100) {
		if (string.IsNullOrEmpty(name)) return;
		AudioClip clip = instance.clips.Find(c => c.name == name);
		AudioMixerGroup mixer = instance.mixers.Find(m => m.name == mixerName);
		if (mixer == null) mixer = instance.defaultSFXMixer;
		PlayEffect(clip, mixer, t, volume, pitch);
	}

	public static void PlayEffect(AudioClip clip, AudioMixerGroup mixer = null, Transform t = null, float volume = 1, float pitch = 1, float pan = 0, float spatialBlend = 0, float minDist = 1, float maxDist = 100) {
		#if UNITY_EDITOR
		if (!Application.isPlaying) return;
		#endif
		if (clip == null) return;
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
		source.volume = volume;
		source.spatialBlend = spatialBlend;
		source.panStereo = pan;
		source.minDistance = minDist;
		source.maxDistance = maxDist;
		source.outputAudioMixerGroup = mixer;
		source.spatialBlend = spatialBlend;
		source.minDistance = minDist;
		source.maxDistance = maxDist;
		source.Play();
		instance.currentSFXSource = (instance.currentSFXSource + 1) % instance.sfxSourcesCount;
	}

	public static void PlayMusic (AudioClip clip) {
		if (currentSource.clip == clip) return;
		currentSource.clip = clip;
		currentSource.Play();
	}

	public static void PlayMusic () {
		if (instance.music != null) PlayMusic(instance.music);
	}

	public static void PauseMusic () {
		if (currentSource != null) currentSource.Pause();
	}

	public static void ResumeMusic () {
		if (currentSource != null) currentSource.UnPause();
	}

	public static void StopMusic () {
		if (currentSource != null) currentSource.Stop();
	}

	public static void CrossfadeMusic(AudioClip clip, float fadeDuration) {
		if (clip == null || instance.musicSources == null) return;
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
			sourceA.volume = (1 - frac);
			sourceB.volume = frac;
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		sourceA.volume = 0;
		sourceA.Stop();
		sourceB.volume = 1;
	}
}
}