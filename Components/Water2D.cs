using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Components;
using Paraphernalia.Extensions;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Water2D : MonoBehaviour {

    public Color color = Color.white;
    [Range(8,512)]public int segments = 8;
    public Rect rect = new Rect(-1, -1, 2, 2);
    [Range(0.001f,100)] public float springK = 0.02f;
    [Range(0,1)] public float damping = 0.04f;
    [Range(0,0.5f)] public float spread = 0.05f;
    public string splashParticles = "Splash";
    public string splashSound = "splash";

    [SortingLayer] public string sortingLayerName = "Default";
    public int sortingOrder = 100;

    float[] offsets;
    float[] velocities;
    float[] accelerations;
    float[] leftDeltas;
    float[] rightDeltas;
    Vector3[] vertices;
    Mesh mesh;
    Bounds bounds; 

    void Start() {
        Setup();
    }

    [ContextMenu("Setup")]
    void Setup () {
        if (mesh == null) mesh = new Mesh();
        if (bounds == null) bounds = new Bounds(rect.center, rect.size);
        offsets = new float[segments+1];
        velocities = new float[segments+1];
        accelerations = new float[segments+1];
        leftDeltas = new float[segments+1];
        rightDeltas = new float[segments+1];
        vertices = new Vector3[segments * 4];
        SetupMesh();
    }

    void SetupMesh () {
        Color[] colors = new Color[segments * 4];
        Vector3[] normals = new Vector3[segments * 4];
        Vector2[] uv = new Vector2[segments * 4];
        int[] triangles = new int[segments * 6];

        int triIndex = 0;
        float frac = rect.width / (rect.height * (float)segments);
        for (int i = 0; i < segments; i++) {
            float a = (float)i * frac;
            float b = (float)(i + 1) * frac;
            uv.SetRange(
                i * 4, 
                new Vector2[]{
                    new Vector2(a, 1),
                    new Vector2(b, 1),
                    new Vector2(b, 0),
                    new Vector2(a, 0)
                }
            );
            colors.SetRange(i * 4, new Color[] {color, color, color, color});
            normals.SetRange(i * 4, new Vector3[] {-Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward});
            triangles.SetRange(i * 6, new int[] {triIndex, triIndex+2, triIndex+3, triIndex, triIndex+1, triIndex+2});
            triIndex += 4;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;

        Renderer r = GetComponent<Renderer>();
        r.sortingLayerName = sortingLayerName;
        r.sortingOrder = sortingOrder;

        UpdateMesh();
    }

    void UpdateMesh () {
        float w = rect.width / (float)segments;
        Vector3 min = (Vector3)rect.min;
        Vector3 max = (Vector3)rect.max;
        float h = rect.height;
        for (int i = 0; i < segments; i++) {
            if (h+offsets[i]>max.y) max.y = h + offsets[i];
            vertices[i*4] = min;
            vertices[i*4].y += h + offsets[i];
            vertices[i*4+1] = min;
            vertices[i*4+1].x += w;
            vertices[i*4+1].y += h + offsets[i+1];
            vertices[i*4+2] = min;
            vertices[i*4+2].x += w;
            vertices[i*4+3] = min;
            min.x += w;
        }

        mesh.vertices = vertices;
        bounds.max = max;
        mesh.bounds = bounds;
    }

    void Update () {
        #if UNITY_EDITOR
        if (!Application.isPlaying) {
            if (mesh.vertices.Length == segments*4) UpdateMesh();
            else Setup();
            return;
        }
        #endif
        
        float dt = Time.deltaTime;
        for (int i = 0; i <= segments; i++) {
            accelerations[i] = -springK * offsets[i] - velocities[i] * damping;
            velocities[i] += accelerations[i] * dt;
            offsets[i] += velocities[i] * dt;
        }

        for (int j = 0; j < 8; j++) {
            for (int i = 0; i <= segments; i++) {
                if (i > 0) {
                    leftDeltas[i] = spread * (offsets[i] - offsets[i-1]);
                    velocities[i-1] += leftDeltas[i];
                }
                if (i < segments) {
                    rightDeltas[i] = spread * (offsets[i] - offsets[i+1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i <= segments; i++) {
                if (i > 0) {
                    offsets[i - 1] += leftDeltas[i];
                }
                if (i < offsets.Length - 1) {
                    offsets[i + 1] += rightDeltas[i];
                }
            }
        }

        UpdateMesh();
    }

    void OnTriggerEnter2D (Collider2D collider) {
        Rigidbody2D body = collider.gameObject.GetComponent<Rigidbody2D>();
        if (body) Splash(body);
    }

    void OnTriggerExit2D (Collider2D collider) {
        Rigidbody2D body = collider.gameObject.GetComponent<Rigidbody2D>();
        if (body) Splash(body);
    }

    void Splash (Rigidbody2D body) {
        Collider2D c = this.GetComponent<Collider2D>();
        float frac = (body.gameObject.transform.position.x - c.bounds.min.x) / c.bounds.size.x;
        velocities[(int)Mathf.Min(frac * segments, velocities.Length - 1)] = body.velocity.y;
        if (!string.IsNullOrEmpty(splashParticles)) {
            ParticleManager.Play(splashParticles, body.gameObject.transform.position);
            float volume = Mathf.Clamp01(body.velocity.y * body.velocity.y * 0.1f);
            float pitch = 1 + 0.05f * Mathf.Sign(body.velocity.y);
            AudioManager.PlayEffect(splashSound, pitch: pitch, volume: volume);
        }
    }
}
