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
public enum CubeMappingType {
	Spherical,
	Cylindrical,
	Faces4x3,
	Faces3x4,
	Faces6x1,
	Faces1x6,
}

public static class CubemapUtils {

	public static CubeMappingType GetCubeMappingType (this Texture2D texture2D) {
		return GetCubeMappingType(texture2D.width, texture2D.height);
	}

	public static CubeMappingType GetCubeMappingType (int width, int height) {
		float ratio = (float)width / (float)height;
		if (ratio == 1f) return CubeMappingType.Spherical;
		else if (ratio == 2f) return CubeMappingType.Cylindrical;
		else if (ratio == 4f/3f) return CubeMappingType.Faces4x3;
		else if (ratio == 3f/4f) return CubeMappingType.Faces3x4;
		else if (ratio == 6f) return CubeMappingType.Faces6x1;
		else if (ratio == 1f/6f) return CubeMappingType.Faces1x6;
		else {
			Debug.LogError("Ratio " + ratio + " does not match any standard cubemap import type");
			return CubeMappingType.Cylindrical;
		}
	}

	public static CubemapFace GetFace (Vector3 dir) {
		if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) {
			if (dir.x > 0) return CubemapFace.PositiveX;
			else return CubemapFace.NegativeX;
		}
		else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) {
			if (dir.y > 0) return CubemapFace.PositiveY;
			else return CubemapFace.NegativeY;
		}
		else {
			if (dir.z > 0)  return CubemapFace.PositiveZ;
			else return CubemapFace.NegativeZ;
		}
	}

	public static Plane GetPlane (CubemapFace face) {
		switch (face) {
			case CubemapFace.PositiveX: return new Plane(-Vector3.right, 1);
			case CubemapFace.NegativeX: return new Plane(Vector3.right, 1);
			case CubemapFace.PositiveY: return new Plane(-Vector3.up, 1);
			case CubemapFace.NegativeY: return new Plane(Vector3.up, 1);
			case CubemapFace.PositiveZ: return new Plane(-Vector3.forward, 1);
			default: return new Plane(Vector3.forward, 1);
		}
	}

	public static Vector3 GetIntersectionPoint (CubemapFace face, Vector3 dir) {
		float dist;
		Ray ray = new Ray(Vector3.zero, dir);
		Plane plane = GetPlane(face);
		plane.Raycast(ray, out dist);
		return ray.GetPoint(dist);
	}

	public static Vector3 GetIntersectionPoint (Vector3 dir) {
		return GetIntersectionPoint(GetFace(dir), dir);
	}
}
}