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
using Paraphernalia.Extensions;

namespace Paraphernalia.Math {
[System.Serializable]
public class Polygon {

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

    public Polygon (Bounds bounds) {
        SetPath(new Vector2[] {
            bounds.min,
            new Vector2(bounds.min.x, bounds.max.y),
            bounds.max, 
            new Vector2(bounds.max.x, bounds.min.y)
        });
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
        if (path.Length == 0) return;
        float left = path[0].x;
        float right = path[0].x;
        float bottom = path[0].y;
        float top = path[0].y;
        for (int i = 0; i < path.Length; i++) {
            Vector2 p = path[i];
            if (p.x > right) right = p.x;
            else if (p.x < left) left = p.x;
            if (p.y > top) top = p.y;
            else if (p.y < bottom) bottom = p.y;
        }
        _rect = Rect.MinMaxRect(left, bottom, right, top);
    }

    public bool Contains (Vector2 point) {
        return GetWindingNumber(point) != 0;
    }

    public int GetWindingNumber (Vector2 point) {
        float winding = 0;
        int len = path.Length;
        for (int i = 0; i < len; i++) {
            Vector2 p1 = (path[(i + len - 1) % len] - point).normalized;
            Vector2 p2 = (path[i] - point).normalized;
            winding += Mathf.Atan2(
                p1.x * p2.y - p1.y * p2.x, 
                p1.x * p2.x + p1.y * p2.y
            );
        }
        return Mathf.RoundToInt(winding / (2 * Mathf.PI));
    }

    public int GetWindingNumber () {
        float winding = 0;
        int len = path.Length;
        for (int i = 0; i < len; i++) {
            Vector2 p = path[i];
            Vector2 p1 = (path[(i + len - 1) % len] - p).normalized;
            Vector2 p2 = (path[(i + len + 1) % len] - p).normalized;
            winding += Mathf.PI - Mathf.Atan2(
                p1.x * p2.y - p1.y * p2.x, 
                p1.x * p2.x + p1.y * p2.y
            );
        }
        return Mathf.RoundToInt(winding / (2 * Mathf.PI));
    }

    public Polygon GetOffset (float offset) {
        return new Polygon(GetOffsetPath(offset));
    }

    public Vector2[] GetOffsetPath (float offset) {
        int len = path.Length;
        List<Vector2> newPath = new List<Vector2>();

        Vector2 prev = path[len-1];
        Vector2 curr = path[0];
        Vector2 next = path[1];

        
        Vector2 currDir = (prev - curr).normalized;
        Vector2 nextDir = (next - curr).normalized;

        Vector2 prevOut = currDir.GetPerpendicular();
        Vector2 currOut = nextDir.GetPerpendicular();
        
        for (int i = 1; i < len + 1; i++) {
            prev = curr;
            curr = next;
            next = path[(i+1)%len];

            currDir = nextDir;
            nextDir = (next - curr).normalized;
            
            prevOut = currOut;
            currOut = nextDir.GetPerpendicular();

            Line2D lineA = new Line2D(prevOut, prevOut + currDir);
            Line2D lineB = new Line2D(currOut, currOut + nextDir);
            newPath.Add(curr + lineA.Intersect(lineB) * offset);
        }

        return newPath.ToArray();
    }

    public void Cut (Vector2 point, Vector2 normal) {
        if (path.Length <= 1) return;
        normal.Normalize();
        List<Vector2> newPath = new List<Vector2>();
        Line2D cutLine = new Line2D(point, point + normal.GetPerpendicular());
        int len = path.Length;
        Vector2 lastPoint = path[len-1];
        float lastDot = Vector2.Dot((lastPoint-point).normalized, normal);
        for (int i = 0; i < len; i++) {
            float dot = Vector2.Dot((path[i]-point).normalized, normal);
            if (lastDot * dot < 0) {
                Line2D l = new Line2D(lastPoint, path[i]);
                Vector2 intersect = l.Intersect(cutLine);
                newPath.Add(intersect);
            }
            if (dot <= 0) newPath.Add(path[i]);
            lastPoint = path[i];
            lastDot = dot;
        }
        SetPath(newPath.ToArray());
    }

    public Polygon[][] Split (Line2D line) {
        // if (GetWindingNumber() > 0) _path = path.Reverse();

        int len = path.Length;
        List<Polygon> negPolys = new List<Polygon>();
        List<Polygon> posPolys = new List<Polygon>();
        List<Vector2> newPath = new List<Vector2>();
        
        List<Vector2> firstPath = new List<Vector2>();
        int firstSide = line.Side(path[0]);

        int lastSide = line.Side(path[len-1]);
        Vector2 lastPoint = path[len-1];
        Polygon polyToRevisit = null;

        for (int i = 0; i < len; i++) {
            int side = line.Side(path[i]);

            // if we've crossed the line
            if (Mathf.Abs(lastSide - side) == 2) {

                Line2D l = new Line2D(lastPoint, path[i]);
                Vector2 intersect = l.Intersect(line);
                newPath.Add(intersect);
                if (firstPath.Count == 0) {
                    firstPath.AddRange(newPath);
                }
                else if (polyToRevisit != null) {
                    polyToRevisit.AppendToPath(newPath.ToArray());
                    polyToRevisit = null;
                }
                else {
                    Polygon poly = new Polygon(newPath.ToArray());
                    if (lastSide < 0) negPolys.Add(poly);
                    else posPolys.Add(poly);
                }
                newPath.Clear();
                newPath.Add(intersect);
                
                // look through current polygons on this side 
                // to see if this point is contained in it
                // if so, append to said polygon until you exit again
                if (polyToRevisit == null) {
                    Polygon[] polyCheckList = new Polygon[0];
                    if (side > 0) polyCheckList = posPolys.ToArray();
                    else if (side < 0) polyCheckList = negPolys.ToArray();
                    for (int j = 0; j < polyCheckList.Length; j++) {
                        if (polyCheckList[j].Contains(path[i])) {
                            polyToRevisit = polyCheckList[j];
                            break;
                        }
                    }
                }
            }

            newPath.Add(path[i]);
            lastPoint = path[i];
            lastSide = side;
        }

        if (firstPath != null) {
            newPath.AddRange(firstPath);
        }
        Polygon lastPoly = new Polygon(newPath.ToArray());
        if (firstSide < 0) negPolys.Add(lastPoly);
        else posPolys.Add(lastPoly);

        // Debug.Log(negPolys.Count + " " + posPolys.Count);
        return new Polygon[2][] {negPolys.ToArray(), posPolys.ToArray()};
    }

    public Polygon[] TestSplit () {
        float x = rect.x + rect.width * 0.5f;
        float y = rect.y + rect.height * 0.5f;

        // Debug.Log("rect:" + rect + " x:" + x + " y:" + y);
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

        // return list2.ToArray();
        return list.ToArray();
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
        // Debug.Log(currentPolys.Count);
        return outputPolys.ToArray();
    }

    public override string ToString() {
        string s = "";
        foreach (Vector2 point in path) {
            s += string.Format("({0}, {1}) ", point.x, point.y);
        }
        return s;
    }
}
}