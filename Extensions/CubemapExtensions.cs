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
using System.Collections.Generic;
using System.IO;
using Paraphernalia.Utils;

namespace Paraphernalia.Extensions {
public static class CubemapExtensions {

	public static Texture2D FaceToTexture2D (this Cubemap cubemap, CubemapFace face) {
		Texture2D texture2D = new Texture2D(cubemap.width, cubemap.height, cubemap.format, false);
		texture2D.SetPixels(cubemap.GetPixels(face));
		return texture2D;
	}

	public static Color GetColorInDirection (this Cubemap cubemap, Vector3 dir) {
		// this could be faster, save some of the calculations to be reused
		float x = 0;
		float y = 0;
		Ray ray = new Ray(Vector3.zero, dir);
		float d;
		Plane p;
		Vector3 pos;
		CubemapFace face = CubemapUtils.GetFace(dir);
		switch (face) {
			case CubemapFace.PositiveX:
				p = new Plane(-Vector3.right, 1);
				p.Raycast(ray, out d);
				pos = ray.GetPoint(d);
				x = -0.5f * pos.z + 0.5f;
				y = -0.5f * pos.y + 0.5f;
				break;
			case CubemapFace.NegativeX:
				p = new Plane(Vector3.right, 1);
				p.Raycast(ray, out d);
				pos = ray.GetPoint(d);
				x = 0.5f * pos.z + 0.5f;
				y = -0.5f * pos.y + 0.5f;
				break;
			case CubemapFace.PositiveY:
				p = new Plane(-Vector3.up, 1);
				p.Raycast(ray, out d);
				pos = ray.GetPoint(d);
				x = 0.5f * pos.x + 0.5f;
				y = 0.5f * pos.z + 0.5f;
				break;
			case CubemapFace.NegativeY:
				p = new Plane(Vector3.up, 1);
				p.Raycast(ray, out d);
				pos = ray.GetPoint(d);
				x = 0.5f * pos.x + 0.5f;
				y = -0.5f * pos.z + 0.5f;
				break;
			case CubemapFace.PositiveZ:
				p = new Plane(-Vector3.forward, 1);
				p.Raycast(ray, out d);
				pos = ray.GetPoint(d);
				x = 0.5f * pos.x + 0.5f;
				y = -0.5f * pos.y + 0.5f;
				break;
			case CubemapFace.NegativeZ:
				p = new Plane(Vector3.forward, 1);
				p.Raycast(ray, out d);
				pos = ray.GetPoint(d);
				x = -0.5f * pos.x + 0.5f;
				y = -0.5f * pos.y + 0.5f;
				break;
		}
		return cubemap.GetPixel(face, (int)(x * cubemap.width), (int)(y * cubemap.height));
	}

	public static void SaveToPNG (this Cubemap cubemap, string path, CubeMappingType mapping = CubeMappingType.Faces4x3) {
		switch (mapping) {
			case CubeMappingType.Cylindrical:
				cubemap.SaveToCylindricalPNG(path);
				break;
			case CubeMappingType.Spherical:
				cubemap.SaveToSphericalPNG(path);
				break;
			case CubeMappingType.Faces4x3:
				cubemap.SaveTo4x3PNG(path);
				break;
			case CubeMappingType.Faces3x4:
				cubemap.SaveTo3x4PNG(path);
				break;
			case CubeMappingType.Faces6x1:
				cubemap.SaveTo6x1PNG(path);
				break;
			case CubeMappingType.Faces1x6:
				cubemap.SaveTo1x6PNG(path);
				break;
		}
	}

	public static void SaveTo4x3PNG (this Cubemap cubemap, string path) {
		Texture2D texture2D = cubemap.Get4x3Texture2D();
		texture2D.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(texture2D);
		else GameObject.DestroyImmediate(texture2D);
	}


	public static void SaveTo3x4PNG (this Cubemap cubemap, string path) {
		Texture2D texture2D = cubemap.Get3x4Texture2D();
		texture2D.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(texture2D);
		else GameObject.DestroyImmediate(texture2D);
	}

	public static void SaveTo6x1PNG (this Cubemap cubemap, string path) {
		Texture2D texture2D = cubemap.Get6x1Texture2D();
		texture2D.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(texture2D);
		else GameObject.DestroyImmediate(texture2D);
	}

	public static void SaveTo1x6PNG (this Cubemap cubemap, string path) {
		Texture2D texture2D = cubemap.Get1x6Texture2D();
		texture2D.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(texture2D);
		else GameObject.DestroyImmediate(texture2D);
	}

	public static void SaveToCylindricalPNG (this Cubemap cubemap, string path) {
		Texture2D texture2D = cubemap.GetCylindricalTexture2D();
		texture2D.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(texture2D);
		else GameObject.DestroyImmediate(texture2D);
	}

	public static void SaveToSphericalPNG (this Cubemap cubemap, string path) {
		Texture2D texture2D = cubemap.GetSphericalTexture2D();
		texture2D.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(texture2D);
		else GameObject.DestroyImmediate(texture2D);
	}

	public static Texture2D Get4x3Texture2D (this Cubemap cubemap) {
		int w = cubemap.width;
		int h = cubemap.height;
		Texture2D texture2D = new Texture2D(w * 4, h * 3, cubemap.format, false);
		
		texture2D.SetPixels(w * 2, 0,     w, h, cubemap.GetPixels(CubemapFace.PositiveY));
		texture2D.SetPixels(w,     h,     w, h, cubemap.GetPixels(CubemapFace.NegativeX));
		texture2D.SetPixels(w * 2, h,     w, h, cubemap.GetPixels(CubemapFace.PositiveZ));
		texture2D.SetPixels(w * 3, h,     w, h, cubemap.GetPixels(CubemapFace.PositiveX));
		texture2D.SetPixels(0,     h,     w, h, cubemap.GetPixels(CubemapFace.NegativeZ));
		texture2D.SetPixels(w * 2, h * 2, w, h, cubemap.GetPixels(CubemapFace.NegativeY));
		texture2D.Apply();

		texture2D = texture2D.Rotate180();
		return texture2D;
	}

	public static Texture2D Get3x4Texture2D (this Cubemap cubemap) {
		int w = cubemap.width;
		int h = cubemap.height;
		Texture2D texture2D = new Texture2D(w * 3, h * 4, cubemap.format, false);
		
		texture2D.SetPixels(w,     0,     w, h, cubemap.GetPixels(CubemapFace.PositiveY));
		texture2D.SetPixels(0,     h,     w, h, cubemap.GetPixels(CubemapFace.NegativeX));
		texture2D.SetPixels(w,     h,     w, h, cubemap.GetPixels(CubemapFace.PositiveZ));
		texture2D.SetPixels(w * 2, h,     w, h, cubemap.GetPixels(CubemapFace.PositiveX));
		texture2D.SetPixels(w    , h * 2, w, h, cubemap.GetPixels(CubemapFace.NegativeY));
		texture2D.SetPixels(w,     h * 3, w, h, cubemap.GetPixels(CubemapFace.NegativeZ).Reverse());
		texture2D.Apply();

		texture2D = texture2D.Rotate180();
		return texture2D;
	}

	public static Texture2D Get6x1Texture2D (this Cubemap cubemap) {
		int w = cubemap.width;
		int h = cubemap.height;
		Texture2D texture2D = new Texture2D(w * 6, h, cubemap.format, false);
		
		texture2D.SetPixels(0,     0, w, h, cubemap.GetPixels(CubemapFace.PositiveZ));
		texture2D.SetPixels(w,     0, w, h, cubemap.GetPixels(CubemapFace.NegativeZ));
		texture2D.SetPixels(w * 2, 0, w, h, cubemap.GetPixels(CubemapFace.NegativeY).Reverse());
		texture2D.SetPixels(w * 3, 0, w, h, cubemap.GetPixels(CubemapFace.PositiveY).Reverse());
		texture2D.SetPixels(w * 4, 0, w, h, cubemap.GetPixels(CubemapFace.NegativeX));
		texture2D.SetPixels(w * 5, 0, w, h, cubemap.GetPixels(CubemapFace.PositiveX));
		texture2D.Apply();

		texture2D = texture2D.Rotate180();
		return texture2D;
	}

	public static Texture2D Get1x6Texture2D (this Cubemap cubemap) {
		int w = cubemap.width;
		int h = cubemap.height;
		Texture2D texture2D = new Texture2D(w, h * 6, cubemap.format, false);
		
		texture2D.SetPixels(0, 0,     w, h, cubemap.GetPixels(CubemapFace.PositiveX));
		texture2D.SetPixels(0, h,     w, h, cubemap.GetPixels(CubemapFace.NegativeX));
		texture2D.SetPixels(0, h * 2, w, h, cubemap.GetPixels(CubemapFace.PositiveY).Reverse());
		texture2D.SetPixels(0, h * 3, w, h, cubemap.GetPixels(CubemapFace.NegativeY).Reverse());
		texture2D.SetPixels(0, h * 4, w, h, cubemap.GetPixels(CubemapFace.NegativeZ));
		texture2D.SetPixels(0, h * 5, w, h, cubemap.GetPixels(CubemapFace.PositiveZ));
		texture2D.Apply();

		texture2D = texture2D.Rotate180();
		return texture2D;
	}

	public static Texture2D GetCylindricalTexture2D (this Cubemap cubemap) {
		int w = cubemap.width * 4;
		int h = cubemap.height * 2;
		Texture2D texture2D = new Texture2D(w, h, cubemap.format, false);

		for (int j = 0; j < h; j++) {
			for (int i = 0; i < w; i++) {
				float a = (((float)i / (float)w) - 0.25f) * Mathf.PI * 2;
				float x = -Mathf.Cos(a);
				float y = Mathf.Sin(a);
				Vector3 dir = new Vector3 (x, 0, y);
				float b = ((float)j / (float)h);
				
				if (b > 0.5f) dir = Vector3.Slerp(dir, Vector3.up, (b - 0.5f) * 2);
				else dir = Vector3.Slerp(dir, -Vector3.up, 1 - b * 2);

				texture2D.SetPixel(i, j, cubemap.GetColorInDirection(dir));
			}
		}

		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D GetSphericalTexture2D (this Cubemap cubemap) {
		int w = cubemap.width * 2;
		int h = cubemap.height * 2;
		Texture2D texture2D = new Texture2D(w, h, cubemap.format, false);
		Color backColor = cubemap.GetColorInDirection(-Vector3.forward);

		for (int j = 0; j < h; j++) {
			for (int i = 0; i < w; i++) {
				float x = (float)i / (float)w;
				float y = (float)j / (float)h;
				float d = Vector2.Distance(Vector2.one * 0.5f, new Vector2(x,y));
				if (d <= 0.5f) {
					float r = 2 * Mathf.Sqrt(-4*x*x + 4*x - 1 - 4*y*y + 4*y);
					Vector3 dir = new Vector3(
						r * (2 * x - 1),
						r * (2 * y - 1),
						-8*x*x + 8*x - 8*y*y + 8*y - 3
					);
					texture2D.SetPixel(i, j, cubemap.GetColorInDirection(dir));
				}
				else {
					texture2D.SetPixel(i, j, backColor);
				}
			}
		}

		texture2D.Apply();
		return texture2D;
	}

	public static void FastSaveToCylindricalPNG (this Cubemap cubemap, string path) {
		int w = cubemap.width * 4;
		int h = cubemap.height * 2;
		Texture2D cylindricalMapping = TextureGenerator.CylindricalMapping(w, h);
		cubemap.SaveToPNGWithDirectionalMapping(path, cylindricalMapping);
		if (Application.isPlaying) GameObject.Destroy(cylindricalMapping);
		else GameObject.DestroyImmediate(cylindricalMapping);
	}

	public static void FastSaveToSphericalPNG (this Cubemap cubemap, string path) {
		int w = cubemap.width * 2;
		int h = cubemap.height * 2;
		Texture2D sphericalMapping = TextureGenerator.SphericalMapping(w, h);
		cubemap.SaveToPNGWithDirectionalMapping(path, sphericalMapping);
		if (Application.isPlaying) GameObject.Destroy(sphericalMapping);
		else GameObject.DestroyImmediate(sphericalMapping);
	}

	public static void SaveToPNGWithDirectionalMapping (this Cubemap cubemap, string path, Texture2D mapping) {
		Material material = new Material(Shader.Find("Sphere Sketch/Directional Mapping"));
		material.SetTexture("_DirMap", mapping);
		material.SetTexture("_Cube", cubemap);
		
		RenderTexture renderTexture = new RenderTexture(mapping.width, mapping.height, 0, RenderTextureFormat.ARGB32);
		renderTexture.Create();
		Graphics.Blit(renderTexture, renderTexture, material);
		renderTexture.SaveToPNG(path);
		if (Application.isPlaying) GameObject.Destroy(renderTexture);
		else GameObject.DestroyImmediate(renderTexture);
	}
}
}