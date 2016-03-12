// from http://gamedesigntheory.blogspot.ie/2010/09/controlling-aspect-ratio-in-unity.html
// originally by Adrian Lopez
// adapted by Nolan Baker

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraAspect : MonoBehaviour {

	public float aspectRatio = 16f/9f;
	
	void Start () {
		SetAspectRatio();
	}

	public void SetAspectRatio () {
		Camera camera = GetComponent<Camera>();
		Rect rect = new Rect(0, 0, 1, 1);

		float screenAspect = (float)Screen.width / (float)Screen.height;
	    float scale = screenAspect / aspectRatio;

	    if (scale < 1.0f) {  
	        rect.height = scale;
	        rect.y = (1.0f - scale) / 2.0f;
	    }
	    else {
	        scale = 1.0f / scale;
	        rect.width = scale;
	        rect.x = (1.0f - scale) / 2.0f;
	    }
        camera.rect = rect;
	}
}
