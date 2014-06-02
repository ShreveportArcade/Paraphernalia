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

namespace Paraphernalia.Math {
[System.Serializable]
class Polygon {

	private Vector2[] path;
	private Rect rect;

	public Polygon (Vector2[] path) {
		this.path = path;
		
		float left = Mathf.Infinity;
		float right = Mathf.NegativeInfinity;
		float bottom = Mathf.Infinity;
		float top = Mathf.NegativeInfinity;
		for (int i = 0; i < path.Length; i++) {
			Vector2 p = path[i];
			if (p.x > right) right = p.x;
			else if (p.x < left) left = p.x;
			else if (p.y > top) top = p.y;
			else if (p.y < bottom) bottom = p.y;
		}
		rect = Rect.MinMaxRect(left, top, right, bottom);
	}

	public bool ContainsPoint (Vector2 point) {
		return GetWindingNumber(point) != 0;
	}

	public int GetWindingNumber (Vector2 point) {
		float winding = 0;
		int len = path.Length;
		for (int i = 0; i < len; i++) {
			Vector2 p1 = path[(i + len - 1) % len] - point;
			Vector2 p2 = path[i] - point;
			winding += Vector2.Angle(p1, p2);
		}

		return Mathf.RoundToInt(winding / 360f);
	}

	public Polygon[][] Split (Line2D line) {
		List<Polygon> leftPolys = new List<Polygon>();
		List<Polygon> rightPolys = new List<Polygon>();
		List<Vector2> newPath = new List<Vector2>();

		int len = path.Length;
		int lastSide = line.Side(path[len-1]);
		Vector2 lastPoint = path[len-1];
		for (int i = 0; i < len; i++) {
			int side = line.Side(path[i]);

			// if we've crossed the line
			if (Mathf.Abs(lastSide - side) == 2) {
				Line2D l = new Line2D(lastPoint, path[i]);
				Vector2 intersect = l.Intersect(line);
				newPath.Add(intersect);
				Polygon poly = new Polygon(newPath.ToArray());
				if (side < 0) leftPolys.Add(poly);
				else rightPolys.Add(poly);

				newPath.Clear();
			}

			newPath.Add(path[i]);
			lastPoint = path[i];
			lastSide = side;
		}
		return new Polygon[2][] {leftPolys.ToArray(), rightPolys.ToArray()};
	}

	public Polygon[] Subdivide (Vector2 size) {
		List<Polygon> outputPolys = new List<Polygon>();
		List<Polygon> currentPolys = new List<Polygon>();
		currentPolys.Add(this);
		for (float x = rect.xMin + size.x; x < rect.xMax; x += size.x) {
			Line2D line = new Line(new Vector2(x, 0), new Vector2(x, 1));
			List<Polygon> leftPolys = new List<Polygon>();
			List<Polygon> rightPolys = new List<Polygon>();
			foreach (Polygon p in currentPolys) {
				Polygon[][] split = p.Split(line);
				leftPolys.AddRange(split[0]);
				rightPolys.AddRange(split[1]);
			}
			currentPolys = rightPolys;
			for (float y = rect.yMin + size.y; y < rect.yMax; y += size.y) {
				List<Polygon> topPolys = new List<Polygon>();
				List<Polygon> bottomPolys = new List<Polygon>();
				Line2D line = new Line(new Vector2(x, 0), new Vector2(x, 1));
				foreach (Polygon p in leftPolys) {
					Polygon[][] split = p.Split(line);
					leftPolys.AddRange(split[0]);
					rightPolys.AddRange(split[1]);
				}
				outputPolys.AddRange()
			}
		}

		return null;
	}
}
}