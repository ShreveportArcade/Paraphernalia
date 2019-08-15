/*
Copyright (C) 2014-2019 Nolan Baker

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
using Paraphernalia.Utils;
using Paraphernalia.Extensions;

namespace Paraphernalia.Components {
[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

    private static CameraController _instance;
    public static CameraController instance {
        get { 
            if (_instance == null) {
                _instance = FindObjectOfType<CameraController>();
            }
            return _instance; 
        }
    }

    private static CameraController[] _instances;
    public static CameraController[] instances {
        get { 
            if (_instances == null || _instances.Length == 0 || _instances[0] == null) {
                _instances = FindObjectsOfType<CameraController>();
            }
            return _instances; 
        }
    }

    private static Vector3 center;
    private static Bounds _splitBounds = new Bounds();
    public static Bounds splitBounds {
        get { return _splitBounds; }
    } 

    public List<CameraZone> cameraZones = new List<CameraZone>();
    public AudioClip defaultMusic;
    public string targetTag = "Player";
    new public Camera camera;
    public Transform target;
    public bool useFixedUpdate = false;
    public Vector3 offset = -Vector3.forward;
    public float speed = 1;
    public Vector3 velocityAdjustment = new Vector2(0.2f, 0);
    public bool platformLock = false;
    public bool bounded = false;
    public float mergeStartDistance = 10;
    public float mergeDistance = 5;
    public Vector3 splitOffset = -Vector3.forward;
    public GameObject boundsObject;
    public bool useBoundsObjectZ = false;
    public Bounds bounds;
    public Interpolate.EaseType easeType = Interpolate.EaseType.InOutQuad;
    public bool destroyDuplicates = true;
    private bool transitioning = false;
    private Vector3 rawPosition;

    private float _merge = 0;
    public float merge {
        get {
            return _merge;
        }
    }

    public static CameraController CameraControllerFromTarget (GameObject target) {
        foreach (CameraController c in CameraController.instances) {
            if (target.GetInstanceID() == c.target.gameObject.GetInstanceID()) {
                return c;
            }
        }
        return null;
    }


    [HideInInspector] public float platformY;
    Vector3 targetPosition {
        get {
            Vector3 v = Vector3.zero;
            Rigidbody2D r = target.GetComponent<Rigidbody2D>();
            if (r != null) {
                v = Vector3.Scale(r.velocity, velocityAdjustment);
                v = v - Vector3.forward * v.magnitude * velocityAdjustment.z;
            }

            Rigidbody b = target.GetComponent<Rigidbody>();
            if (b != null) v = Vector3.Scale(b.velocity, velocityAdjustment);

            Vector3 off = offset;
            if (instances.Length > 1) off = splitOffset;

            Vector3 pos = target.position;
            if (platformLock && pos.y > platformY) pos.y = platformY;

            return pos + off + v;
        }
    } 

    void Awake () {
        if (_instance == null) { 
            _instance = this;
            SetPosition();
            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }
        else if (_instance != this && destroyDuplicates) {
            Debug.LogWarning("Instance of CameraController already exists. Destroying duplicate.");
            GameObjectUtils.Destroy(gameObject);
        }
        else if (_instance != this && !destroyDuplicates) {
            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }
    }

    void Start () {
        transitioning = true;
        SetPosition();
        if (instance.defaultMusic != null && Application.isPlaying) AudioManager.CrossfadeMusic(instance.defaultMusic, 0.5f);
        transitioning = false;
    }

    void LateUpdate () {
        if (!useFixedUpdate || !Application.isPlaying) UpdatePosition();
    }

    void FixedUpdate () {
        if (useFixedUpdate) UpdatePosition();
    }

    void Update () {
        if (this != instance || instances[0].target == null) return;
        center = System.Array.ConvertAll(instances, (i) => i.rawPosition).Average();

        _splitBounds = new Bounds(instances[0].target.position, Vector3.zero);
        int len = instances.Length;
        for (int i = 1; i < len; i++) {
            Vector3 v = _instances[i].target.position;
            _splitBounds.Encapsulate(v);
        }
        _splitBounds.Expand(0.01f);
    }

    void SetPosition () {
        if (target == null) {
            GameObject go = GameObject.FindWithTag(targetTag);
            if (go == null) return;
            target = go.transform;
        }
        if (target != null) {
            platformY = target.position.y;
        }
        Vector3 off = offset;
        if (instances.Length > 1) off = splitOffset;
        Vector3 pos = target.position;
        if (platformLock && pos.y > platformY) pos.y = platformY;
        transform.position = pos + off;
        rawPosition = transform.position;
        
        Collider2D[] collider2Ds = Physics2D.OverlapPointAll(target.position);
        foreach (Collider2D collider2D in collider2Ds) {
            CameraZone zone = collider2D.gameObject.GetComponent<CameraZone>();
            if (zone != null) {
                transform.position = zone.position.Lerp3(transform.position, zone.axisLock);
                break;
            }
        }

        Collider[] colliders = Physics.OverlapSphere(target.position, 1);
        foreach (Collider collider in colliders) {
            CameraZone zone = collider.gameObject.GetComponent<CameraZone>();
            if (zone != null) {
                transform.position = zone.position.Lerp3(transform.position, zone.axisLock);
                break;
            }
        }

        if (bounded) BoundPosition();
    }

    void BoundPosition () {
        if (boundsObject) {
            bounds = boundsObject.RendererBounds(); //TODO: don't do this every frame
            if (!useBoundsObjectZ) {
                Vector3 ext = bounds.extents;
                ext.z = Mathf.Infinity;
                bounds.extents = ext;
            }
        }
        transform.position = camera.GetBoundedPos(bounds);
    }

    public static void SetOffset(Vector3 offset) {
        instance.offset = offset;
        instance.SetPosition();
    }

    void UpdatePosition () {
        if (target != null && camera != null) {

            #if UNITY_EDITOR
            if (!Application.isPlaying) SetPosition();
            else {
            #endif
                if (transitioning) return;
                LerpToTarget();

                Vector3 mergedPosition = rawPosition;
                Vector3 diff = center - rawPosition;
                float dist = diff.magnitude;
                if (dist < mergeDistance) {
                    _merge = 1;
                    mergedPosition = center;
                }
                else if (dist < mergeStartDistance) {
                    _merge = (mergeStartDistance - dist) / (mergeStartDistance - mergeDistance);
                    mergedPosition = Vector3.Lerp(rawPosition, center, _merge);
                }
                else _merge = 0;

                CameraZone zone = (cameraZones.Count > 0) ? cameraZones[cameraZones.Count - 1] : null;
                if (zone != null) mergedPosition = zone.position.Lerp3(mergedPosition, zone.axisLock);
                
                transform.position = mergedPosition;

                if (bounded) BoundPosition();
            #if UNITY_EDITOR
            }
            #endif
        }
        else {
            GameObject g = GameObject.FindWithTag(targetTag);
            if (g!= null) {
                target = g.transform;
                SetPosition();
            }
        }
    }

    void LerpToTarget () {
        float dT = useFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        rawPosition = Vector3.Lerp(rawPosition, targetPosition, speed * dT);
    }

    public void AddZone(CameraZone zone) {
        if (cameraZones.Contains(zone)) return;
        cameraZones.Add(zone);
        StopCoroutine("TransitionToZoneCoroutine");
        StartCoroutine("TransitionToZoneCoroutine");
    }

    public void RemoveZone(CameraZone zone) {
        cameraZones.Remove(zone);
        transitioning = false;
        StopCoroutine("TransitionToZoneCoroutine");
        if (cameraZones.Count > 0) StartCoroutine("TransitionToZoneCoroutine");
        else {
            rawPosition = transform.position;
            if (defaultMusic != null) AudioManager.CrossfadeMusic(instance.defaultMusic, 0.5f);
        }
    }

    IEnumerator TransitionToZoneCoroutine () {
        transitioning = true;
        CameraZone zone = cameraZones[cameraZones.Count - 1];
        if (zone.music != null) AudioManager.CrossfadeMusic(zone.music, zone.transitionTime);
        Vector3 pos = transform.position;
        float elapsedTime = 0;
        while (elapsedTime < zone.transitionTime) {
            float dT = zone.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += dT;
            float t = zone.transitionTime - elapsedTime;

            Vector3 dir = zone.position.Lerp3(targetPosition, zone.axisLock) - transform.position;
            pos += dir * dT / t;

            transform.position = Vector3.Lerp(transform.position, pos, dT * speed);
            if (bounded) BoundPosition();
            yield return new WaitForEndOfFrame();
        }
        SetPosition();
        transitioning = false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
}