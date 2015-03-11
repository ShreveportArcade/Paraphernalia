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

namespace Paraphernalia.Components {
public class ScaleToViewport : MonoBehaviour {

	public Camera cam;
	[Range(0,1)] public float left = 0;
	[Range(0,1)] public float right = 1;
	public bool update = false;
	
	void Start () {
		if (cam == null) cam = Camera.main;
		if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
		Scale();	
	}

	[ContextMenu("Scale")]
	void Scale () {
		Vector3 p = transform.position;
		float z = cam.transform.InverseTransformPoint(p).z;
		float l = cam.ViewportToWorldPoint(new Vector3(left, 0.5f, z)).x;
		float r = cam.ViewportToWorldPoint(new Vector3(right, 0.5f, z)).x;
		p.x = (l + r) * 0.5f;
		transform.position = p;

		float xScale = (r - l) / (GetComponent<Renderer>().bounds.max.x - GetComponent<Renderer>().bounds.min.x);
		Vector3 scale = transform.localScale;
		scale.x *= xScale;
		transform.localScale = scale;
	}

	void Update () {
		if (update) Scale();
	}
}
}