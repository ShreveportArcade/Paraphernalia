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

namespace Paraphernalia.Utils {
public static class TextureGenerator {

	public static Texture2D SphericalMapping (int width = 1024, int height = 1024) {
		Texture2D texture2D = new Texture2D(width, height);

		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				float x = (float)i / (float)width;
				float y = (float)j / (float)height;
				float d = Vector2.Distance(Vector2.one * 0.5f, new Vector2(x,y));
				if (d <= 0.5f) {
					float r = 2 * Mathf.Sqrt(-4*x*x + 4*x - 1 - 4*y*y + 4*y);
					Vector3 dir = new Vector3(
						r * (2 * x - 1),
						r * (2 * y - 1),
						-8*x*x + 8*x - 8*y*y + 8*y - 3
					);
					
					dir = dir * 0.5f + Vector3.one * 0.5f;
					texture2D.SetPixel(i, j, new Color(dir.x, dir.y, dir.z, 1));
				}
				else {
					texture2D.SetPixel(i, j, new Color(0.5f, 0.5f, 0, 1));
				}
			}
		}

		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D CylindricalMapping (int width = 2048, int height = 1024) {
		Texture2D texture2D = new Texture2D(width, height);

		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				float a = (((float)i / (float)width) - 0.25f) * Mathf.PI * 2;
				float x = -Mathf.Cos(a);
				float y = Mathf.Sin(a);
				Vector3 dir = new Vector3 (x, 0, y);
				float b = ((float)j / (float)height);
				
				if (b > 0.5f) dir = Vector3.Slerp(dir, Vector3.up, (b - 0.5f) * 2);
				else dir = Vector3.Slerp(dir, -Vector3.up, 1 - b * 2);

				dir = dir * 0.5f + Vector3.one * 0.5f;
				texture2D.SetPixel(i, j, new Color(dir.x, dir.y, dir.z, 1));
			}
		}

		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D PerlinNoise (int width = 1024, int height = 1024) {
		Texture2D texture2D = new Texture2D(width, height);
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				float n = Mathf.PerlinNoise((float)i / (float)width, (float)j / (float)height);
				texture2D.SetPixel(i, j, new Color(n,n,n,1));
			}
		}

		texture2D.Apply();
		return texture2D;
	}
}
}