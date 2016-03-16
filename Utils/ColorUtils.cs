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
using System;
using System.Globalization;
using System.Collections;

namespace Paraphernalia.Utils {
public static class ColorUtils {

	public static Color Random () {
		float r = UnityEngine.Random.value;
		float g = UnityEngine.Random.value;
		float b = UnityEngine.Random.value;
		float a = UnityEngine.Random.value;
		return new Color(r, g, b, a);
	}

	public static Color Random (float alpha) {
		float r = UnityEngine.Random.value;
		float g = UnityEngine.Random.value;
		float b = UnityEngine.Random.value;
		return new Color(r, g, b, alpha);
	}

	public static Color Random (Color target, float weight) {
		return Color.Lerp(ColorUtils.Random(), target, weight);
	}

	public static Color Random (Color target, float weight, float alpha) {
		Color c = Color.Lerp(ColorUtils.Random(), target, weight);
		c.a = alpha;
		return c;
	}

	public static Color HSVtoRGB (Vector4 hsv) {
		float chroma = hsv.z * hsv.y;
		float h = 6 * hsv.x;
		float x = chroma * (1 - Mathf.Abs((h % 2) - 1));
		
		Color color;
		if (h < 1 || h == 6)	color = new Color (chroma, x, 0, hsv.w);
		else if (h < 2)			color = new Color (x, chroma, 0, hsv.w);
		else if (h < 3)			color = new Color (0, chroma, x, hsv.w);
		else if (h < 4)			color = new Color (0, x, chroma, hsv.w);
		else if (h < 5)			color = new Color (x, 0, chroma, hsv.w);
		else					color = new Color (chroma, 0, x, hsv.w);

		float m = hsv.z - chroma;
		return color + new Color(m, m, m, 0);
	}

	public static Vector4 RGBtoHSV (Color c) {
		return RGBtoHSV(c, Vector4.one);
	}

	public static Vector4 RGBtoHSV (Color c, Vector4 defaultHSV) {
		float m = Mathf.Min(c.r, c.g, c.b);
		float val = Mathf.Max(c.r, c.g, c.b);
		float chroma = val - m;
		float sat = ((val==0)?defaultHSV.y:chroma / val);

		float hue = defaultHSV.x * 6f;
		if (chroma != 0) {
			if (val == c.r)			hue = (c.g - c.b) / chroma;
			else if (val == c.g)	hue = 2 + (c.b - c.r) / chroma;
			else if (val == c.b)	hue = 4 + (c.r - c.g) / chroma;
		}

		hue /= 6f;

		return new Vector4(hue, sat, val, c.a);
	}

	public static Color HexToRGB (string hexValue) {
		return HexToRGB(hexValue, Color.white);
	}

	public static Color HexToRGB (string hexValue, Color defaultColor) {
		int len = hexValue.Length;
		if (len != 6 && len != 8) {
			Debug.LogError("Expected a 6 (RRGGBB) or 8 (RRGGBBAA) char string, got " + len);
			return defaultColor;
		}

		CultureInfo provider = CultureInfo.InvariantCulture;
		NumberStyles style = NumberStyles.HexNumber;
		byte r = 255;
		byte g = 255;
		byte b = 255;
		byte a = 255;
	
		if (Byte.TryParse(hexValue.Substring(0,2), style, provider, out r) && 
			Byte.TryParse(hexValue.Substring(2,2), style, provider, out g) && 
			Byte.TryParse(hexValue.Substring(4,2), style, provider, out b) && 
			(len == 6 || Byte.TryParse(hexValue.Substring(6,2), style, provider, out a))) {

			return (Color)(new Color32(r,g,b,a));
		}
		else {
			return defaultColor;
		}
	}

	public static string RGBtoHex (Color color) {
		Color32 c = (Color32)color;
		byte[] bytes = new byte[]{c.r, c.g, c.b, c.a};
		return BitConverter.ToString(bytes).Replace("-", "");
	}
}
}