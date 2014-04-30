// From: http://wiki.unity3d.com/index.php/Expose_properties_in_inspector
// Author: Mift (mift)
// licensed under Creative Commons Attribution Share Alike

using UnityEngine;
using System;
using System.Collections;
 
[AttributeUsage( AttributeTargets.Property )]
public class ExposePropertyAttribute : Attribute {
 
}