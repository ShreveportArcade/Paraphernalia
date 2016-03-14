using UnityEngine;
using System.Collections;

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