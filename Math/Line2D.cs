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

namespace Paraphernalia.Math {
[System.Serializable]
public class Line2D {

	private float slope;
	private float intercept;
	private bool vertical;
	private Vector2 point;

	public Line2D (Vector2 p1, Vector2 p2) {
		this.point = p1;
		vertical = (p1.x == p2.x);

		if (vertical) this.slope = Mathf.Infinity;
		else this.slope = (p2.y - p1.y) / (p2.x - p1.x);

		this.intercept = p1.y - this.slope * p1.x;
	}

	public Vector2 PointWithX (float x) {
		if (vertical) return new Vector2(x, point.y);
		return new Vector2(x, slope * x + intercept);
	}

	public Vector2 PointWithY (float y) {
		if (vertical) return new Vector2(point.x, y);
		return new Vector2((y - intercept) / slope, y);
	}

	public float Distance (Vector2 point) {
		if (vertical) return Mathf.Abs(point.x - this.point.x);
		return Mathf.Abs(slope * point.x - point.y + intercept) / Mathf.Sqrt(slope * slope + 1);
	}

	public int Side (Vector2 point) {
		if (PointWithX(point.x) == PointWithY(point.y)) return 0;
		if (vertical) return (int)Mathf.Sign(point.x - this.point.x);
		return (int)Mathf.Sign(point.x - PointWithY(point.y).x);
	}

	public Vector2 Intersect (Line2D line) {
		if (this.slope == line.slope || (line.vertical && this.vertical)) return Vector2.zero;
		if (this.vertical) return line.PointWithX(this.point.x);
		if (line.vertical) return this.PointWithX(line.point.x);

		float x = (line.intercept - this.intercept) / (this.slope - line.slope);
		return new Vector2(x, this.slope * x + this.intercept);
	}

    public override string ToString() {
        if (vertical) return "vertical, x = " + point.x;
        return string.Format("y = {0}x + {1}", slope, intercept);
    }
}
}