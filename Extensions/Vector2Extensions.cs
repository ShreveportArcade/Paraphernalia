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
public static class Vector2Extensions {

    public static Vector2 GetPerpendicular (this Vector2 p) {
        return new Vector2(-p.y, p.x);
    }

    public static Vector2 RotatedByDeg (this Vector2 v, float degrees) {
        return v.RotatedByRad(degrees * Mathf.Deg2Rad);
    }

    public static Vector2 RotatedByRad (this Vector2 v, float radians) {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
    
    public static Vector2 Average (this Vector2[] points) {
        Vector2 average = points[0];
        for (int i = 1; i < points.Length; i++) {
            average += points[i];
        }
        average /= (float)points.Length;
        return average;
    }

    public static float Winding (this Vector2[] path) {
        float ang = 0;
        for (int i = 1; i < path.Length - 1; i++) {
            ang += Vector3.Cross(path[i-1] - path[i], path[i+1] - path[i]).z;
        }

        return ang;
    }

    public static float ClosedWinding (this Vector2[] path) {
        float ang = 0;
        for (int i = 0; i < path.Length; i++) {
            ang += Vector3.Cross(
                path[(i-1+path.Length)%path.Length] - path[i], 
                path[(i+1)%path.Length] - path[i]).z;
        }

        return ang;
    }
}
}
