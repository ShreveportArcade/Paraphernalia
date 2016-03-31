using UnityEngine;
using System.Collections;

namespace Paraphernalia.YieldInstructions {
public class WaitUntil : CustomYieldInstruction {

	private System.Func<bool> waitFunc;

	public override bool keepWaiting { 
		get { return waitFunc(); } 
	}

	public WaitUntil (System.Func<bool> waitFunc) { 
		this.waitFunc = waitFunc; 
	}
}
}