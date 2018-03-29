/*
Copyright (C) 2014 Nolan Baker

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
            if (_instances == null || _instances.Length > 0 && _instances[0] == null) {
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
    public float moveStartDist = 1f;
    public Vector3 velocityAdjustment = new Vector2(0.2f, 0);
    public bool bounded = false;
    public float mergeStartDistance = 10;
    public float mergeDistance = 5;
    public Vector3 splitOffset = -Vector3.forward;
    public GameObject boundsObject;
    public Bounds bounds;
    public Interpolate.EaseType easeType = Interpolate.EaseType.InOutQuad;
    public bool destroyDuplicates = true;
    private bool transitioning = false;
    private Vector3 unmergedPosition;

    private float _merge = 0;
    public float merge {
        get {
            return _merge;
        }
    }

    void Awake () {
        if (_instance == null) { 
            _instance = this;
            SetPosition();
        }
        else if (_instance != this && destroyDuplicates) {
            Debug.LogWarning("Instance of CameraController already exists. Destroying duplicate.");
            GameObjectUtils.Destroy(gameObject);
        }
    }

    void Start () {
        SetPosition();
        if (instance.defaultMusic != null && Application.isPlaying) AudioManager.CrossfadeMusic(instance.defaultMusic, 0.5f);
    }

    void LateUpdate () {
        if (!useFixedUpdate || !Application.isPlaying) UpdatePosition();
    }

    void FixedUpdate () {
        if (useFixedUpdate) UpdatePosition();
    }

    void Update () {
        if (this != instance) return;
        center = System.Array.ConvertAll(instances, (i) => i.unmergedPosition).Average();

        _splitBounds = new Bounds(instances[0].transform.position, Vector3.one);
        int len = instances.Length;
        for (int i = 1; i < len; i++) {
            Vector3 v = _instances[i].transform.position;
            _splitBounds.Encapsulate(v);
        }
        _splitBounds.Expand(1);
    }

    void SetPosition () {
        if (target == null) {
            GameObject go = GameObject.FindWithTag(targetTag);
            if (go == null) return;
            target = go.transform;
        }
        Vector3 off = offset;
        if (instances.Length > 1) off = splitOffset;
        transform.position = target.position + off;
        unmergedPosition = transform.position;
        
        Collider2D[] collider2Ds = Physics2D.OverlapPointAll(target.position);
        foreach (Collider2D collider2D in collider2Ds) {
            CameraZone zone = collider2D.gameObject.GetComponent<CameraZone>();
            if (zone != null) {
                transform.position = zone.position.Lerp3(transform.position, zone.axisLock);
                unmergedPosition = transform.position;
                return;
            }
        }


        Collider[] colliders = Physics.OverlapSphere(target.position, 1);
        foreach (Collider collider in colliders) {
            CameraZone zone = collider.gameObject.GetComponent<CameraZone>();
            if (zone != null) {
                transform.position = zone.position.Lerp3(transform.position, zone.axisLock);
                unmergedPosition = transform.position;
                return;
            }
        }
        if (bounded) {
            if (boundsObject) bounds = boundsObject.RendererBounds();
            transform.position = camera.GetBoundedPos(bounds);
            unmergedPosition = transform.position;
        }
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
                if (!transitioning) {
                    unmergedPosition = LerpToTarget();
                
                    Vector3 diff = center - unmergedPosition;
                    float dist = diff.magnitude;
                    if (dist < mergeDistance) {
                        _merge = 1;
                        transform.position = center;
                    }
                    else if (dist < mergeStartDistance) {
                        _merge = (mergeStartDistance - dist) / (mergeStartDistance - mergeDistance);
                        transform.position = Vector3.Lerp(unmergedPosition, center, _merge);
                    }
                    else {
                        _merge = 0;
                        transform.position = unmergedPosition;
                    }
                }

                if (bounded) {
                    if (boundsObject) bounds = boundsObject.RendererBounds();
                    transform.position = camera.GetBoundedPos(bounds);
                }
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

    Vector3 LerpToTarget () {
        Vector3 desiredPosition = unmergedPosition;
        CameraZone zone = (cameraZones.Count > 0) ? cameraZones[cameraZones.Count - 1] : null;
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
        transform.position = target.position + off;

        float d = Vector3.Distance(target.position, unmergedPosition + off);
        if (d > moveStartDist) {
            Vector3 targetPosition = Vector3.Lerp(
                unmergedPosition,
                target.position + off + v,
                Time.deltaTime * speed
            );
            if (zone == null) desiredPosition = targetPosition;
            else desiredPosition = zone.position.Lerp3(targetPosition, zone.axisLock);
        }

        return desiredPosition;
    }

    public static void AddZone(CameraZone zone) {
        if (instance.cameraZones.Contains(zone)) return;
        instance.cameraZones.Add(zone);
        instance.StopCoroutine("TransitionToZoneCoroutine");
        instance.StartCoroutine("TransitionToZoneCoroutine");
    }

    public static void RemoveZone(CameraZone zone) {
        instance.cameraZones.Remove(zone);
        instance.transitioning = false;
        instance.StopCoroutine("TransitionToZoneCoroutine");
        if (instance.cameraZones.Count > 0) instance.StartCoroutine("TransitionToZoneCoroutine");
        else if (instance.defaultMusic != null) AudioManager.CrossfadeMusic(instance.defaultMusic, 0.5f);
        
    }

    IEnumerator TransitionToZoneCoroutine () {
        transitioning = true;
        CameraZone zone = cameraZones[cameraZones.Count - 1];
        if (zone.music != null) AudioManager.CrossfadeMusic(zone.music, zone.transitionTime);
        Vector3 startPos = transform.position;
        float t = 0;
        while (t < zone.transitionTime) {
            t += Time.deltaTime;
            float frac = t / zone.transitionTime;
            transform.position = Vector3.Lerp(startPos, zone.position.Lerp3(transform.position, zone.axisLock), frac);
            yield return new WaitForEndOfFrame();
        }
        transform.position = zone.position.Lerp3(LerpToTarget(), zone.axisLock);
        transitioning = false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
}