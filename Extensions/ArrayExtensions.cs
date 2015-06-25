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

namespace Paraphernalia.Extensions {
public static class ArrayExtensions {

	public static T[] Reverse<T> (this T[] array) {
		for (int i = 0; i < array.Length / 2; i++) {
			T temp = array[i];
			array[i] = array[array.Length - i - 1];
			array[array.Length - i - 1] = temp;
		}
		return array;
	}

	public static T[] Subarray<T> (this T[] array, int start, int length) {
		T[] result = new T[length];
		Array.Copy(array, start, result, 0, length);
		return result;
	}

	public static T[] SetRange<T> (this T[] array, int start, T[] range) {
		for (int i = 0; i < range.Length; i++) {
			array[start + i] = range[i];
		}
		return array;
	}

	// this is fast but memory hungry, maybe this instead? -> http://stackoverflow.com/a/876410
	public static T[] Shift<T> (this T[] array, int amount) {
		int len = array.Length;
		amount =  -len + (amount) % len;
		T[] result = new T[len];
		for (int i = 0; i < len; i++) {
			int shift = (i - amount) % len;
			result[i] = array[shift];
		}
		return result;
	}
}
}