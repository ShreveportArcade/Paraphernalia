// From: http://wiki.unity3d.com/index.php?title=Triangulator
// Author: Runevision
// made static class, updated to use Vector3 - Nolan Baker

using UnityEngine;
using System.Collections.Generic;
 
namespace Paraphernalia.Utils {
public static class Triangulator {
	public static int[] Triangulate(Vector3[] points) {
        List<int> indices = new List<int>();
 
        int n = points.Length;
        if (n < 3)
            return indices.ToArray();
 
        int[] V = new int[n];
        if (Area(points) > 0) {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }
 
        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2; ) {
            if ((count--) <= 0)
                return indices.ToArray();
 
            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;
 
            if (Snip(points, u, v, w, nv, V)) {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }
 
        indices.Reverse();
        return indices.ToArray();
    }
 
    private static float Area (Vector3[] points) {
        int n = points.Length;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            Vector3 pval = points[p];
            Vector3 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }
 
    private static bool Snip (Vector3[] points, int u, int v, int w, int n, int[] V) {
        Vector3 A = points[V[u]];
        Vector3 B = points[V[v]];
        Vector3 C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (int p = 0; p < n; p++) {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }
 
    private static bool InsideTriangle (Vector3 A, Vector3 B, Vector3 C, Vector3 P) {
        float ax = C.x - B.x;
        float ay = C.y - B.y;
        float bx = A.x - C.x;
        float by = A.y - C.y;
        float cx = B.x - A.x;
        float cy = B.y - A.y;
        float apx = P.x - A.x;
        float apy = P.y - A.y;
        float bpx = P.x - B.x;
        float bpy = P.y - B.y;
        float cpx = P.x - C.x;
        float cpy = P.y - C.y;
 
        float aCROSSbp = ax * bpy - ay * bpx;
        float cCROSSap = cx * apy - cy * apx;
        float bCROSScp = bx * cpy - by * cpx;
 
        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}
}