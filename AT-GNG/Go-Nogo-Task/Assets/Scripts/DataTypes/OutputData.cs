using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using EnumTypes;

namespace CustomDataTypes
{
	public class OutputData
	{
		public float stimStartTime { get; set;}
		public GNGTrialTypes stimType { get; set;}
		public float respStartTime { get; set;}
		public InputAction respAction { get; set;}
		public int trialNumber { get; set; }
		public InputControl control { get; set; }

		public OutputData()
		{
			stimStartTime = 0.0f;
			stimType = GNGTrialTypes.none;
			respStartTime = 0.0f;
			respAction = new InputAction();
			trialNumber = 0;
			control = respAction.activeControl;
		}

		public OutputData(float sst, GNGTrialTypes st)
		{
			stimStartTime = sst;
			stimType = st;
			respStartTime = 0.0f;
			respAction = new InputAction();
			trialNumber = 0;
			control = respAction.activeControl;
		}

		public OutputData(float sst, GNGTrialTypes st, int tn)
		{
			stimStartTime = sst;
			stimType = st;
			respStartTime = 0.0f;
			respAction = new InputAction();
			trialNumber = tn;
			control = respAction.activeControl;
		}

		public OutputData(float sst, GNGTrialTypes st, float rst, InputAction ra, int tn, InputControl c)
		{
			stimStartTime = sst;
			stimType = st;
			respStartTime = rst;
			respAction = ra;
			trialNumber = tn;
			control = c;
		}

	}
}
