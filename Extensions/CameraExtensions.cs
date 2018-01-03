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
using System.Collections;

namespace Paraphernalia.Extensions {
public static class CameraExtensions {

	public static Vector3 GetBoundedPos (this Camera camera, Bounds bounds) {
		Vector3 pos = camera.transform.position;		
		Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, -pos.z));
		Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, -pos.z));
		Vector3 topRightDiff = topRight - bounds.max;
		Vector3 bottomLeftDiff = bottomLeft - bounds.min;
		Vector3 size = topRight - bottomLeft;
		
		if (size.x > bounds.size.x) pos.x = bounds.center.x;
		else if (topRightDiff.x > 0) pos.x -= topRightDiff.x;
		else if (bottomLeftDiff.x < 0) pos.x -= bottomLeftDiff.x;

		if (size.y > bounds.size.y) pos.y = bounds.center.y;
		else if (topRightDiff.y > 0) pos.y -= topRightDiff.y;
		else if (bottomLeftDiff.y < 0) pos.y -= bottomLeftDiff.y;

        pos.z = camera.transform.position.z;
		if (pos.z > bounds.center.z + bounds.extents.z) pos.z = bounds.center.z + bounds.extents.z;
        else if (pos.z < bounds.center.z - bounds.extents.z) pos.z = bounds.center.z - bounds.extents.z;
		
        return pos;
	}
}
}