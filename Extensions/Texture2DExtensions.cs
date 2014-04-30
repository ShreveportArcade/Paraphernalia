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
using System.IO;

namespace Paraphernalia.Extensions {
public static class Texture2DExtensions {

	public static Texture2D RotateClockwise90 (this Texture2D texture2D) {
		int height = texture2D.height;
		int width = texture2D.width;
		Texture2D rotatedTexture2D = new Texture2D(height, width, texture2D.format, false);
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				rotatedTexture2D.SetPixel(height - j - 1, width - i - 1, texture2D.GetPixel(i,j));
			}
		}

		rotatedTexture2D.Apply();
		return rotatedTexture2D;
	}

	public static Texture2D RotateCounterclockwise90 (this Texture2D texture2D) {
		int height = texture2D.height;
		int width = texture2D.width;
		Texture2D rotatedTexture2D = new Texture2D(height, width, texture2D.format, false);
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				rotatedTexture2D.SetPixel(j, i, texture2D.GetPixel(i,j));
			}
		}

		rotatedTexture2D.Apply();
		return rotatedTexture2D;
	}

	public static Texture2D Rotate180 (this Texture2D texture2D) {
		int height = texture2D.height;
		int width = texture2D.width;
		Texture2D rotatedTexture2D = new Texture2D(width, height, texture2D.format, false);
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				rotatedTexture2D.SetPixel(width - i - 1, height - j - 1, texture2D.GetPixel(i,j));
			}
		}

		rotatedTexture2D.Apply();
		return rotatedTexture2D;
	}

	public static void SaveToPNG (this Texture2D texture2D, string path) {
		byte[] bytes = texture2D.EncodeToPNG();

		#if UNITY_WEBPLAYER
		FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(bytes);
		binaryWriter.Close();
		fileStream.Close();

		#else
		File.WriteAllBytes(path, bytes);

		#endif
	}
}
}