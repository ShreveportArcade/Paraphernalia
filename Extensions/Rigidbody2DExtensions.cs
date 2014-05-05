using UnityEngine;
using System.Collections;

namespace Paraphernalia.Extensions {
public static class Rigidbody2DExtensions {

	// From: http://forum.unity3d.com/threads/213397-Rigidbody2D-ForceMode-Impulse
	//       http://answers.unity3d.com/questions/574864/rigidbody2d-and-forcemodeaddvelocity.html
	// Author: gfoot 
	// made into an extension by Kencho
	public static void AddForce(this Rigidbody2D rigidbody2D, Vector2 force, ForceMode mode = ForceMode.Force) {
		switch (mode) {
		case ForceMode.Force:
			rigidbody2D.AddForce(force);
			break;
		case ForceMode.Impulse:
			rigidbody2D.AddForce(force / Time.fixedDeltaTime);
			break;
		case ForceMode.Acceleration:
			rigidbody2D.AddForce(force * rigidbody2D.mass);
			break;
		case ForceMode.VelocityChange:
			rigidbody2D.AddForce(force * rigidbody2D.mass / Time.fixedDeltaTime);
			break;
		}
	}
}
}