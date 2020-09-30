using System;
using System.Collections.Generic;
using UnityEngine;

using EnumTypes;
using CustomDataTypes;


namespace ManagerSystems 
{
/******************************************************************************
Class: Trial Manager
Purpose: To contain information about the nature of the experiment (i.e. how many trials or sets of trials, how long between trials, how many types of targets/non-targets).
Job: Trial Manager tells the Game Manager what type of trial is happening and when all the trials are done. The data structure for the trials themselves are found in TrialData in the CustomDataTypes namespace.
******************************************************************************/
	public class TrialManager
	{	
		//private ExperimentTypes exp = ExperimentTypes.gonogo;
		/*I eventually want the trial manager to recognize the type of experiment that is supposed to be happening and perhaps change elements like the percent target or the algorithm for assigning trial types.*/

		//=====================================================================
		/*---- Trial Administration Information ----*/
		/**----------- Variables -----------**/
		private int totalTrials;
		private int currentTrial;
		public float intertrialTime {get; set;} //Time for Unity is measured in seconds; this is the delta between trials as measured in seconds
		private float percentTargets; //Number between 0 and 1
		private List<TrialData> trials;
		
		/*---- Helper Information ----*/
		/**----------- Variables -----------**/
		private System.Random rand;
		public float lastTrialTime;
		private Stack<GNGTrialTypes> trialSet; //For filling list of trials

		//=====================================================================
		/*---- Trial Administration Information ----*/
		/**----------- Methods -----------**/

		public TrialManager()
		{
			totalTrials = 7;
			currentTrial = 0;
			intertrialTime = 2.0f; //Time for Unity is measured in seconds; this is the delta between trials as measured in seconds
			percentTargets = 0.5f; //Number between 0 and 1
			trials = new List<TrialData>();
			rand = new System.Random();
			lastTrialTime = 0.0f;
			trialSet = new Stack<GNGTrialTypes>(); //For filling list of trials

			FillTrialSet();
		}

		public TrialManager(int tt, float itt, float pt)
		{
			totalTrials = tt;
			currentTrial = 0;
			intertrialTime = itt;
			percentTargets = pt;
			trials = new List<TrialData>();
			rand = new System.Random();
			lastTrialTime = 0.0f;
			trialSet = new Stack<GNGTrialTypes>();

			FillTrialSet();
		}

		public int GetCurrentTrial()
		{
			return currentTrial;
		}

		public TrialData GetTrial()
		{
			return trials[currentTrial-1];
		}

		public TrialData GetSpecificTrial(int n)
		{
			return trials[n];
		}

		bool IsItTrialTime()
		{
			if (Time.time >= lastTrialTime + intertrialTime)
			{
				return true;
			} else {
				return false;
			}
		}

		public void TrialTime()
		{
			if (IsItTrialTime())
			{
				//It is trial time
				lastTrialTime = (float) Time.realtimeSinceStartup;
				if (currentTrial < totalTrials)
				{
					TrialData t = new TrialData(trialSet.Pop(), currentTrial);
					trials.Add(t);
					if (t.trialType == GNGTrialTypes.target)
					{
						EventManager.TriggerEvent("Target");
					}
					else if (t.trialType == GNGTrialTypes.nontarget)
					{
						EventManager.TriggerEvent("Nontarget");
					}
					else
					{
						//Do nothing
					}
					//EventManager.TriggerEvent("TrialStart");
					currentTrial++;
				}
			}
		}
		
		/*---- Helper Information ----*/
		/**----------- Methods -----------**/	
		/**********************************************************************
		Function: FillTrialSet
		Inputs: -
		Outputs: -
		Pre-condition: Empty stack.
		Post-condition: Filled stack (size of total number of trials) with a random ordering of targets and non-targets that never exceed max 		numbers of each type.
		Purpose:  
		**********************************************************************/
		public void FillTrialSet()
		{
			int maxTarget = (int)Math.Round(totalTrials*percentTargets, MidpointRounding.AwayFromZero);
			
			int maxNontarget = (int)Math.Round(totalTrials*(1-percentTargets),MidpointRounding.AwayFromZero);
			
			if (totalTrials == (maxTarget + maxNontarget))
			{
				//Do nothing the numbers are right
			} else 
			{
				if (percentTargets < 0.5f) //Priority to non-target
				{
					if (maxTarget > 1)
					{
						maxTarget--;
					}
				}
				else {
					if (maxNontarget > 1) 
					{
						maxNontarget--;
					} 
				}
			}
			StackFiller(maxTarget, maxNontarget);
		}

		/**********************************************************************
		Function: StackFiller
		Inputs: maxTarget (the total number of target trials in this), maxNontarget (total number of nontarget trials in this)
		Outputs: -
		Pre-condition: Empty stack.
		Post-condition: Filled stack (size of total number of trials) with a random ordering of targets and non-targets that never exceed max 		numbers of each type.
		Purpose: We want the ordering of the trial types (target, nontarget) to be randomized between tests, but we want the same number of each type in each test so that we can compare the results between participants. This way we can accurately measure inhibition and response (because they can't just learn the patterns), while still making sure there's a way it's consistent.
		**********************************************************************/
		void StackFiller(int maxTarget, int maxNontarget)
		{
			int currentTarget = 0;
			int currentNontarget = 0;
			GNGTrialTypes temp;

			for (int i = 0; i <= totalTrials; i++)
			{
				temp = GoNoGo.ThisTrialType(percentTargets,rand);
				switch(temp)
				{
					case GNGTrialTypes.target:
						if (currentTarget < maxTarget)
						{
							trialSet.Push(temp);
							currentTarget++;
						} else
						{
							if (currentNontarget < maxNontarget)
							{
								trialSet.Push(GNGTrialTypes.nontarget);
								currentNontarget++;
							} else 
							{
								//Debug.LogError("Something screwy happened when setting trial types");
							}
						}
						break;

					case GNGTrialTypes.nontarget:
						if (currentNontarget < maxNontarget)
						{
							trialSet.Push(temp);
							currentNontarget++;
						} else
						{
							if (currentTarget < maxTarget)
							{
								trialSet.Push(GNGTrialTypes.target);
								currentTarget++;
							} else 
							{
								//Debug.LogError("Something screwy happened when setting trial types");
							}
						}
						break;
				}
			}
		}

		public bool IsTrialSetFilled()
		{
			if (trialSet.Count == 0)
			{
				return false;
			} else if (trialSet.Count == totalTrials)
			{
				return true;
			} else {
				return false;
			}
		}
	}
}