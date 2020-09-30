using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnumTypes;

/******************************************************************************
Go/No-Go Experiments
-----------------------
Go/No-go (GNG) experiments are about inhibiting responses to non-target trials, and performing responses quickly to target trials.

GNG experiments only have two types of trials: target (good) and non-targer (bad).
******************************************************************************/
public class GoNoGo
{
	public static GNGTrialTypes ThisTrialType(float ratioTarget, System.Random r)
	{
		//Debug.Log("GNGTrialTypes running");
		double x = r.NextDouble();
		//Debug.Log("x is " + x);
		if (x <= ratioTarget)
		{
			//Debug.Log("GNGTrailType: target selected");
			return GNGTrialTypes.target;
		}
		else
		{
			//Debug.Log("GNGTrialType: nontarget selected");
			return GNGTrialTypes.nontarget;
		}
	}
}
