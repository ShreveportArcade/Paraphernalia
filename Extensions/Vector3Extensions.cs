/*
License and copyrights are as follows, except where noted below.

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

namespace Paraphernalia.Extensions {
public static class Vector3Extensions {

    public static Vector3 Inverse (this Vector3 v) {
        return new Vector3(1f/v.x, 1f/v.y, 1f/v.z);
    }

    public static Vector3 Average (this Vector3[] points) {
        Vector3 average = points[0];
        for (int i = 1; i < points.Length; i++) {
            average += points[i];
        }
        average /= (float)points.Length;
        return average;
    }

    // Resample is adapted from $1 recognizer
    // From: http://depts.washington.edu/aimgroup/proj/dollar/
    // For inquires about this fn or the $1 recognizer, please email wobbrock@uw.edu.
    // Copyright (c) 2007-2011, Jacob O. Wobbrock, Andrew D. Wilson and Yang Li.
    public static Vector3[] Resample (this Vector3[] path, int n, bool closed) {		
        float I = path.PathLength(closed) / (n - 1);
        float D = 0.0f;
        List<Vector3> srcPts = new List<Vector3>(path);
        List<Vector3> dstPts = new List<Vector3>(n);
        dstPts.Add(srcPts[0]);
        
        for (int i = 1; i < srcPts.Count; i++) {
            Vector3 pt1 = srcPts[i - 1];
            Vector3 pt2 = srcPts[i];
        
            float d = Vector3.Distance(pt1, pt2);
            if ((D + d) >= I) {
                float qx = pt1.x + ((I - D) / d) * (pt2.x - pt1.x);
                float qy = pt1.y + ((I - D) / d) * (pt2.y - pt1.y);
                Vector3 q = new Vector3(qx, qy);
                dstPts.Add(q); // append new point 'q'
                srcPts.Insert(i, q); // insert 'q' at position i in points s.t. 'q' will be the next i
                D = 0.0f;
            }
            else D += d;
        }
        
        // somtimes we fall a rounding-error short of adding the last point, so add it if so
        if (dstPts.Count == n - 1) dstPts.Add(srcPts[srcPts.Count - 1]);
        
        return dstPts.ToArray();
    }

    public static Vector3[] RemoveColinear (this Vector3[] path, float maxAng, bool closed) {
        List<Vector3> nonColinear = new List<Vector3>(path);
        float ang = 0;
        for (int i = 1; i < path.Length-1; i++) {
            ang = Vector3.Angle(path[i] - path[i-1], path[i+1] - path[i]);
            if (Mathf.Abs(ang) < maxAng) {
                nonColinear.Remove(path[i]);
            }
        }

        return nonColinear.ToArray();
    }

    public static Vector3[] RemoveOverlapping (this Vector3[] path, float minDist = 0.1f) {
        List<Vector3> nonOverlapping = new List<Vector3>(path);
        float distSq = 0;
        for (int i = 0; i < path.Length; i++) {
            distSq = (path[i] - path[(i+1)%path.Length]).sqrMagnitude;
            if (distSq < minDist) {
                nonOverlapping.Remove(path[i]);
            }
        }

        return nonOverlapping.ToArray();
    }

    public static float Winding (this Vector3[] path) {
        float ang = 0;
        for (int i = 1; i < path.Length - 1; i++) {
            ang += Vector3.Cross(path[i-1] - path[i], path[i+1] - path[i]).z;
        }

        return ang;
    }

    public static float ClosedWinding (this Vector3[] path) {
        float ang = 0;
        for (int i = 0; i < path.Length; i++) {
            ang += Vector3.Cross(
                path[(i-1+path.Length)%path.Length] - path[i], 
                path[(i+1)%path.Length] - path[i]).z;
        }

        return ang;
    }

    public static float PathLength (this Vector3[] path, bool closed) {
        float len = 0;
        for (int i = 1; i < path.Length; i++) {
            len += Vector3.Distance(path[i-1], path[i]);
        }
        if (closed) {
            len += Vector3.Distance(path[path.Length-1], path[0]);
        }

        return len;
    }

    public static Vector3[] MoveBy (this Vector3[] points, Vector3 delta) {
        for (int i = 0; i < points.Length; i++) {
            points[i] += delta;
        }
        return points;
    }

    public static Bounds GetBounds (this Vector3[] points) {
        Bounds b = new Bounds();
        Vector3 max = points[0];
        Vector3 min = points[0];
        for (int i = 1; i < points.Length; i++) {
            Vector3 p = points[i];
            if (p.x > max.x) max.x = p.x;
            if (p.x < min.x) min.x = p.x;
            if (p.y > max.y) max.y = p.y;
            if (p.y < min.y) min.y = p.y;
        }
        b.SetMinMax(min, max);
        return b;
    }

    public static Vector3 ClipToBounds (this Vector3 v, Bounds bounds) {
        if (v.x > bounds.center.x + bounds.extents.x) v.x = bounds.center.x + bounds.extents.x;
        else if (v.x < bounds.center.x - bounds.extents.x) v.x = bounds.center.x - bounds.extents.x;

        if (v.y > bounds.center.y + bounds.extents.y) v.y = bounds.center.y + bounds.extents.y;
        else if (v.y < bounds.center.y - bounds.extents.y) v.y = bounds.center.y - bounds.extents.y;

        if (v.z > bounds.center.z + bounds.extents.z) v.z = bounds.center.z + bounds.extents.z;
        else if (v.z < bounds.center.z - bounds.extents.z) v.z = bounds.center.z - bounds.extents.z;

        return v;
    }

    public static Vector3 ClipToExtents (this Vector3 v, Vector3 extents) {
        if (v.x > extents.x) v.x = extents.x;
        else if (v.x < -extents.x) v.x = -extents.x;

        if (v.y > extents.y) v.y = extents.y;
        else if (v.y < -extents.y) v.y = -extents.y;

        if (v.z > extents.z) v.z = extents.z;
        else if (v.z < -extents.z) v.z = -extents.z;

        return v;
    }

    public static Vector3 Lerp3 (this Vector3 a, Vector3 b, Vector3 t) {
        return new Vector3 (
                Mathf.Lerp(a.x, b.x, t.x),
                Mathf.Lerp(a.y, b.y, t.y),
                Mathf.Lerp(a.z, b.z, t.z)
            );
    }
}
}
