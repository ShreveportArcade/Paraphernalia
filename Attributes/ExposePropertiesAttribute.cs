// From: http://wiki.unity3d.com/index.php/Expose_properties_in_inspector
// Author: Mift (mift)

using UnityEngine;
using System;
using System.Collections;
 
[AttributeUsage( AttributeTargets.Property )]
public class ExposePropertyAttribute : Attribute {
 
}