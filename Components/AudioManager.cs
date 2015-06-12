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

	void Awake () {
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(_instance.gameObject);
			CreateSFXPool();
			CreateMusicPool();
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

	public static void PlayEffect(AudioClip clip) {
		AudioSource source = instance.sfxSources[instance.currentSFXSource];
		source.PlayOneShot(clip, 1);
		instance.currentSFXSource = (instance.currentSFXSource + 1) % instance.sfxSourcesCount;
	}

	public static void PlayMusic(AudioClip clip) {
		AudioSource currentSource = instance.musicSources[instance.currentMusicSource];
		currentSource.clip = clip;
		currentSource.Play();
	}

	public static void CrossfadeMusic(AudioClip clip, float fadeDuration) {
		instance.musicSources[(instance.currentMusicSource + 1) % 2].clip = clip;
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
			sourceA.volume = 1 - frac;
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