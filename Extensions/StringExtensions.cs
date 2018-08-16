/*
Copyright (C) 2018 Nolan Baker

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

namespace Paraphernalia.Extensions {
public static class StringExtensions {

    public static int LevenshteinDist(this string a, string b) {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))  return 0;

        int lenA = a.Length;
        int lenB = b.Length;
        int[,] dists = new int[lenA+1, lenB+1];

        for (int i = 0; i <= lenA; dists[i, 0] = i++);
        for (int j = 0; j <= lenB; dists[0, j] = j++);

        for (int i = 0; i < lenA; i++) {
            for (int j = 0; j < lenB; j++) {
                int cost = b[j] == a[i] ? 0 : 1;
                dists[i, j] = Mathf.Min(
                    Mathf.Min(dists[i, j+1]+1, dists[i+1, j]+1),
                    dists[i, j] + cost
                );
            }
        }
        return dists[lenA, lenB];
    }
}
}