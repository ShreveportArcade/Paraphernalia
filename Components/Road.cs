using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Components;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Road : MonoBehaviour {

	[Range(2, 63)] public int segments = 5;
	public float width = 1;
	public float influence = 0.5f;
	public bool matchTarget = true;
	public Transform target;
	public Vector3 targetOffset;

	[SortingLayer] public string sortingLayerName = "Default";
	public int sortingOrder = 0;

	private Mesh _mesh;
	private Mesh mesh {
		get {
			if (_mesh == null) {
				_mesh = new Mesh();
			}
			return _mesh;
		}
	}

	[ContextMenu("Setup")]
	void Setup () {
		if (segments < 1 || segments > 63) return;

		UpdateRenderer();
		UpdateMesh();
	}

	void UpdateRenderer () {
		Renderer r = GetComponent<Renderer>();
		r.sortingLayerName = sortingLayerName;
		r.sortingOrder = sortingOrder;
	}

	void UpdateMesh () {		
		Vector3[] vertices = new Vector3[segments * 6];
		Vector3[] normals = new Vector3[segments * 6];
		Vector2[] uv = new Vector2[segments * 6];
		int[] triangles = new int[segments * 12];

		Road otherRoad = null;
		Vector3 endPos = targetOffset;
		Vector3 endRight = Vector3.right;
		Vector3 endUp = Vector3.up;
		float dist = targetOffset.magnitude;
		float widthB = width;
		float influenceB = influence;

		if (target != null) {
			otherRoad = target.gameObject.GetComponent<Road>();
			endPos = transform.InverseTransformPoint(target.position) + targetOffset;
			endRight = transform.InverseTransformDirection(target.right);
			endUp = transform.InverseTransformDirection(target.up);
			dist = Vector3.Distance(transform.position, target.position);
		}

		if (otherRoad != null) {
			widthB = otherRoad.width;
			influenceB = otherRoad.influence;
		}
		Vector3 cpA = Vector3.forward * dist * influence;
		Vector3 cpB = endPos - Vector3.forward * dist * influence;
		if (matchTarget && target != null) {
			cpB = endPos - transform.InverseTransformDirection(target.forward) * dist * influenceB;			
		}

		Vector3 prevRight = Vector3.right * width;

		int triIndex = 0;
		for (int i = 0; i < segments; i++) {
			float fracA = (float)i / (float)segments;
			float fracB = (float)(i+1) / (float)segments;
			Vector3 posA = Interpolate.CubicBezier(Vector3.zero, cpA, cpB, endPos, fracA);
			Vector3 posB = Interpolate.CubicBezier(Vector3.zero, cpA, cpB, endPos, fracB);
			Vector3 right = Vector3.right * width;

			if (matchTarget) {
				Vector3 upA = Vector3.Lerp(Vector3.up, endUp, fracA).normalized;
				Vector3 upB = Vector3.Lerp(Vector3.up, endUp, fracB).normalized;
				
				right = (i == segments - 1) ? endRight : Vector3.Cross(upB, (posB - posA).normalized).normalized;
				right = Vector3.Lerp(right * width, right * widthB, fracB);

				normals.SetRange(i * 6, new Vector3[] {upA, upB, upB, upA, upB, upA});
			}
			else {
				normals.SetRange(i * 6, new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up});
			}

			vertices.SetRange(
				i * 6, 
				new Vector3[] {
					posA - prevRight,
					posB - right,
					posB,
					posA,
					posB + right,
					posA + prevRight
				});

			uv.SetRange(i * 6, new Vector2[]{
				Vector2.zero, 
				Vector2.up, 
				new Vector2(0.5f, 1), 
				new Vector2(0.5f, 0),
				Vector2.one, 
				Vector2.right
				});

			triangles.SetRange(i * 12, new int[] {
				triIndex, triIndex+2, triIndex+3, 
				triIndex, triIndex+1, triIndex+2,
				triIndex+3, triIndex+4, triIndex+5,
				triIndex+3, triIndex+2, triIndex+4
				});

			triIndex += 6;
			prevRight = right;
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	void Update () {
		Setup();
	}

}
