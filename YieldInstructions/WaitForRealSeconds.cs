using UnityEngine;
using System.Collections;

namespace Paraphernalia.YieldInstructions {
public class WaitForRealSeconds : CustomYieldInstruction {

	private float startTime;
	private float duration;

	public override bool keepWaiting { 
		get { return (Time.realtimeSinceStartup - this.startTime) < this.duration; } 
	}

	public WaitForRealSeconds (float seconds) { 
		this.startTime = Time.realtimeSinceStartup;
		this.duration = seconds;
	}
}
}