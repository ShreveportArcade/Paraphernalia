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

	private float _z;
	public float z {
		get { return _z; }
	}

	private Rect _rect;
	public Rect rect {
		get { return _rect; }
	}
	
	private Vector2[] _path;
	public Vector2[] path {
		get { return _path; }
	}
	
	public Polygon (Vector3[] path) {
		SetPath(path);
	}

	public Polygon (Vector2[] path) {
		SetPath(path);
	}

	public void SetPath (Vector2[] path) {
		_path = path;
		UpdateRect();
	}

	public void SetPath (Vector3[] path) {
		_path = System.Array.ConvertAll(path, p => (Vector2)p);
		_z = path[0].z;
		UpdateRect();
	}

	public void AppendToPath (Vector2 point) {
		List<Vector2> newPath = new List<Vector2>(path);
		newPath.Add(point);
		_path = newPath.ToArray();
		if (!rect.Contains(point)) UpdateRect();
	}

	public void AppendToPath (Vector2[] points) {
		List<Vector2> newPath = new List<Vector2>(path);
		newPath.AddRange(points);
		SetPath(newPath.ToArray());
	}

	public void UpdateRect () {
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
		_rect = Rect.MinMaxRect(left, bottom, right, top);
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
		// nope, fails when the path crosses into a polygon that's already been split off

		int len = path.Length;
		List<Polygon> leftPolys = new List<Polygon>();
		List<Polygon> rightPolys = new List<Polygon>();
		List<Vector2> newPath = new List<Vector2>();
		
		List<Vector2> firstPath = new List<Vector2>();
		int firstSide = line.Side(path[0]);

		int lastSide = line.Side(path[len-1]);
		Vector2 lastPoint = path[len-1];

		for (int i = 0; i < len; i++) {
			int side = line.Side(path[i]);

			// if we've crossed the line
			if (Mathf.Abs(lastSide - side) == 2) {
				// TODO: look through current polygons on this side 
				// to see if this point is contained in it
				// if so, append to said polygon until you exit again

				Line2D l = new Line2D(lastPoint, path[i]);
				Vector2 intersect = l.Intersect(line);
				newPath.Add(intersect);
				if (firstPath.Count == 0) {
					firstPath.AddRange(newPath);
				}
				else {
					Polygon poly = new Polygon(newPath.ToArray());
					if (lastSide < 0) leftPolys.Add(poly);
					else rightPolys.Add(poly);
				}
				newPath.Clear();
				newPath.Add(intersect);
			}

			newPath.Add(path[i]);
			lastPoint = path[i];
			lastSide = side;
		}

		if (firstPath != null) {
			newPath.AddRange(firstPath);
		}
		Polygon lastPoly = new Polygon(newPath.ToArray());
		if (firstSide < 0) leftPolys.Add(lastPoly);
		else rightPolys.Add(lastPoly);

		Debug.Log(leftPolys.Count + " " + rightPolys.Count);
		return new Polygon[2][] {leftPolys.ToArray(), rightPolys.ToArray()};
	}

	public Polygon[] TestSplit () {
		float x = (rect.xMax + rect.xMin) * 0.5f;
		float y = (rect.yMax + rect.yMin) * 0.5f;

		Debug.Log(rect + " " + x + " " + y);
		Line2D l = new Line2D(new Vector2(0, y), new Vector2(1, y));
		Polygon[][] polys = this.Split(l);
		List<Polygon> list = new List<Polygon>(polys[0]);
		list.AddRange(polys[1]);

		List<Polygon> list2 = new List<Polygon>();

		l = new Line2D(new Vector2(x, 0), new Vector2(x, 1));
		foreach (Polygon p in list) {
			Polygon[][] split = p.Split(l);
			list2.AddRange(split[0]);
			list2.AddRange(split[1]);
		}

		return list2.ToArray();
		// return list.ToArray();
	}

	public Polygon[] Subdivide (Vector2 size) {
		List<Polygon> outputPolys = new List<Polygon>();
		List<Polygon> currentPolys = new List<Polygon>();
		currentPolys.Add(this);
		for (float x = rect.xMin + size.x; x < rect.xMax; x += size.x) {
			Line2D xLine = new Line2D(new Vector2(x, 0), new Vector2(x, 1));
			List<Polygon> leftPolys = new List<Polygon>();
			List<Polygon> rightPolys = new List<Polygon>();
			foreach (Polygon p in currentPolys) {
				Polygon[][] split = p.Split(xLine);
				leftPolys.AddRange(split[0]);
				rightPolys.AddRange(split[1]);
			}

			currentPolys = leftPolys;
			for (float y = rect.yMin + size.y; y < rect.yMax; y += size.y) {
				List<Polygon> topPolys = new List<Polygon>();
				List<Polygon> bottomPolys = new List<Polygon>();
				Line2D yLine = new Line2D(new Vector2(x, 0), new Vector2(x, 1));
				foreach (Polygon p in currentPolys) {
					Polygon[][] split = p.Split(yLine);
					bottomPolys.AddRange(split[0]);
					topPolys.AddRange(split[1]);
				}
				outputPolys.AddRange(bottomPolys.ToArray());
				currentPolys = topPolys;
			}

			currentPolys = rightPolys;
		}
		Debug.Log(currentPolys.Count);
		return outputPolys.ToArray();
	}
}
}