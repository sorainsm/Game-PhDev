using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnumTypes;

namespace CustomDataTypes
{
/******************************************************************************
Class: Trial Data
Purpose: To contain all relevant information to the trials in the experiment.
Job: Simplifies passing of information to output.
******************************************************************************/
	public class TrialData
	{
		public float startTime { get; set; }
		public GNGTrialTypes trialType { get; set; }
		public int trialNumber { get; set; }

		public TrialData()
		{
			startTime = Time.realtimeSinceStartup;
			trialType = GNGTrialTypes.none;
			trialNumber = -1;
		}

		public TrialData(GNGTrialTypes t, int tn)
		{
			startTime = Time.realtimeSinceStartup;
			trialType = t;
			trialNumber = tn;
		}
	}
}
