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
public static class IntExtensions {

	public static string ToRoman(this int i) {
        if (i > 999) return "M" + ToRoman(i - 1000);
        if (i > 899) return "CM" + ToRoman(i - 900);
        if (i > 499) return "D" + ToRoman(i - 500);
        if (i > 399) return "CD" + ToRoman(i - 400);
        if (i > 99) return "C" + ToRoman(i - 100);            
        if (i > 89) return "XC" + ToRoman(i - 90);
        if (i > 49) return "L" + ToRoman(i - 50);
        if (i > 39) return "XL" + ToRoman(i - 40);
        if (i > 9) return "X" + ToRoman(i - 10);
        if (i > 8) return "IX" + ToRoman(i - 9);
        if (i > 4) return "V" + ToRoman(i - 5);
        if (i > 3) return "IV" + ToRoman(i - 4);
        if (i > 0) return "I" + ToRoman(i - 1);
        return "";
    }
}
}