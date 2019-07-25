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
    public bool dontDestroyOnLoad = true;
    public List<AudioClip> clips = new List<AudioClip>();
    public AudioMixer mixer;
    public string[] mixerGroupPaths;
    public string musicGroupPath = "Master/Music";
    public string sfxGroupPath = "Master/SFX";

    List<AudioMixerGroup> mixers = new List<AudioMixerGroup>();
    AudioMixerGroup musicMixer;
    AudioMixerGroup defaultSFXMixer;

    [Range(0,100)] public int sfxSourcesCount = 5;
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
        get { return instance.musicSources[instance.currentMusicSource]; }
    }

    public static AudioClip currentMusic {
        get { return currentSource.clip; }
    }

    public float minPlayInterval = 0.01f;
    private Dictionary<int, float> lastPlayed = new Dictionary<int, float>();

    void Awake () {
        if (_instance == null) {
            _instance = this;
            if (_instance.transform.parent == null && dontDestroyOnLoad) DontDestroyOnLoad(_instance.gameObject);
            
            if (mixer != null) {
                AudioMixerGroup[] groups;
                foreach (string groupPath in mixerGroupPaths) {
                    groups = mixer.FindMatchingGroups(groupPath);
                    mixers.AddRange(groups);
                }

                groups = mixer.FindMatchingGroups(sfxGroupPath);
                if (groups.Length > 0) defaultSFXMixer = groups[0];
                if (defaultSFXMixer != null && !mixers.Contains(defaultSFXMixer)) mixers.Add(defaultSFXMixer);
                
                groups = mixer.FindMatchingGroups(musicGroupPath);
                if (groups.Length > 0) musicMixer = groups[0];
                if (musicMixer != null && !mixers.Contains(musicMixer)) mixers.Add(musicMixer);
            }

            CreateSFXPool();
            CreateMusicPool();
            if (autoStartMusic) PlayMusic();
        }
        else if (_instance != this) {
            if (autoStartMusic) {
                CrossfadeMusic(music, 0.3f);
            }
            GameObject.Destroy(this);
        }
    }

    void CreateSFXPool () {
        sfxSources = new AudioSource[sfxSourcesCount];
        for (int i = 0; i < sfxSourcesCount; i++) {
            GameObject go = new GameObject("SFXSource." + i);
            go.transform.parent = transform;
            AudioSource sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
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
        AudioMixerGroup mixer = instance.mixers.Find(m => m != null && m.name == mixerName);
        if (mixer == null) mixer = instance.defaultSFXMixer;
        PlayEffect(clip, mixer, t, volume, pitch, pan, spatialBlend, minDist, maxDist);
    }

    public static void PlayEffect(AudioClip clip, AudioMixerGroup mixer = null, Transform t = null, float volume = 1, float pitch = 1, float pan = 0, float spatialBlend = 0, float minDist = 1, float maxDist = 100) {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return;
        #endif
        if (clip == null) return;
        int id = clip.GetInstanceID();
        if (instance.lastPlayed.ContainsKey(id) && 
            Time.realtimeSinceStartup - instance.lastPlayed[id] < instance.minPlayInterval) {
            return;
        }
        instance.lastPlayed[id] = Time.realtimeSinceStartup;
        AudioSource source = instance.sfxSources[instance.currentSFXSource];
        if (source.isPlaying) {
            for (int i = 0; i < instance.sfxSourcesCount; i++) {
                instance.currentSFXSource = (instance.currentSFXSource + 1) % instance.sfxSourcesCount;
                source = instance.sfxSources[instance.currentSFXSource];
                if (!source.isPlaying) break;
            }
        }

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

    public static void StopEffects() {
        foreach (AudioSource source in instance.sfxSources) {
            source.Stop();
        }
    }

    public static void PlayMusic (AudioClip clip, bool clearQueue = true) {
        if (currentSource.clip == clip || clip == null) return;
        if (clearQueue) musicQueue.Clear();
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

    public static void FadeOutMusic (float fadeDuration) {
        instance.StartCoroutine("FadeOutMusicCoroutine", fadeDuration);
    }

    IEnumerator FadeOutMusicCoroutine (float fadeDuration) {
        AudioSource source = musicSources[currentMusicSource];
        
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime) {
            float frac = t / fadeDuration;
            source.volume = (1 - frac);
            yield return new WaitForEndOfFrame();
        }

        source.volume = 0;
        source.Stop();
    }

    static Coroutine crossfadeCoroutine;
    public static void CrossfadeMusic(AudioClip clip, float fadeDuration, bool looped = true, bool clearQueue = true) {
        if (clip == null || instance.musicSources == null) return;
        if (clearQueue) musicQueue.Clear();        
        AudioSource nextSource = instance.musicSources[(instance.currentMusicSource + 1) % 2];
        if (currentSource.clip == clip) {
            FadeMusicVolume(1, fadeDuration);
            return;
        }
        if (crossfadeCoroutine != null) instance.StopCoroutine(crossfadeCoroutine);
        nextSource.clip = clip;
        nextSource.loop = looped;
        crossfadeCoroutine = instance.StartCoroutine(
            instance.CrossfadeMusicCoroutine(fadeDuration)
        );
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
            t += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        sourceA.volume = 0;
        sourceA.Stop();
        sourceB.volume = 1;
    }

    static Coroutine fadeVolumeCoroutine;
    public static void FadeMusicVolume(float volume, float fadeDuration) {
        if (fadeVolumeCoroutine != null) instance.StopCoroutine(fadeVolumeCoroutine);
        fadeVolumeCoroutine = instance.StartCoroutine(
            instance.FadeMusicVolumeCoroutine(volume, fadeDuration)
        );
    }

    IEnumerator FadeMusicVolumeCoroutine(float volume, float fadeDuration) {
        AudioSource source = musicSources[currentMusicSource];
        if (!source.isPlaying) source.Play();

        float startVolume = source.volume;

        float t = 0;
        while (t < fadeDuration) {
            float frac = t / fadeDuration;
            source.volume = Mathf.Lerp(startVolume, volume, frac);
            t += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public static void QueueMusic (AudioClip clip) {
        if (currentSource.clip == clip) return;
        if (currentSource.clip == null) {
            currentSource.clip = clip;
            currentSource.Play();
            return;
        }
        instance.StopCoroutine("QueueMusicCoroutine");
        instance.StartCoroutine("QueueMusicCoroutine", clip);
    }

    static Queue<AudioClip> musicQueue = new Queue<AudioClip>();
    IEnumerator QueueMusicCoroutine (AudioClip clip) {
        musicQueue.Enqueue(clip);
        currentSource.loop = false;
        while (musicQueue.Count > 0) {
            yield return new WaitWhile(() => currentSource.isPlaying );
            currentSource.clip = musicQueue.Dequeue();
            currentSource.Play();
        }
        currentSource.loop = true;
    }
}
}