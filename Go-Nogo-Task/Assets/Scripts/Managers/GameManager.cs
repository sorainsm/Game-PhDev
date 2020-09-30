using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using ManagerSystems;
using EnumTypes;
using CustomDataTypes;

public class GameManager: MonoBehaviour
{
	private static TrialManager tm;
    private static InputManager im;
    private static OutputManager om;

    void Start()
    {
    	if (tm == null)
    	{
    		//Debug.Log("New trial manager made");
    		tm = new TrialManager();
            if (tm.IsTrialSetFilled())
            {
                //Do nothing
            } else {
                Debug.LogError("Stack is empty");
                //I really should throw an exception here if the stack is not filled.
            }
    	}

        if (im == null)
        {
            //Debug.Log("New input manager made");
            im = GameObject.FindWithTag("GameManager").GetComponent<InputManager>();
        }

        if (om == null)
        {
            //Debug.Log("New output manager made");
            om = GameObject.FindWithTag("GameManager").GetComponent<OutputManager>();
        }
    }

    void OnEnable()
    {
    	EventManager.StartListening("TrialStart", ChecksForOutput);
        EventManager.StartListening("InputGiven", OutputInputData);
        EventManager.StartListening("NoInput", OutputNoInputData);
    }

    void OnDisable()
    {
    	EventManager.StopListening("TrialStart", ChecksForOutput);
        EventManager.StopListening("InputGiven", OutputInputData);        
        EventManager.StopListening("NoInput", OutputNoInputData);
    }

    void OutputInputData()
    {
        TrialData t;
        if (tm.GetCurrentTrial() <= 0)
        {
            t = new TrialData();
        } else {
            t = tm.GetTrial();
        }
        OutputData o = new OutputData(t.startTime, t.trialType, im.actionTime, im.currentAction, t.trialNumber, im.currentControl);
        om.SendData(o);
    }

    void OutputNoInputData()
    {
    	TrialData t;
    	if (tm.GetCurrentTrial() <= 0)
        {
            t = new TrialData();
        } else {
            t = tm.GetTrial();
        }
    	OutputData o = new OutputData(t.startTime, t.trialType, t.trialNumber);
    	om.SendData(o);
    }

    void ChecksForOutput()
    {
    	if (tm.GetCurrentTrial() < 0)
    	{
    		Debug.Log("CurrentTrial is nothing");
    	}
    	else
    	{
    		Debug.Log("CurrentTrial is 0 or more");
    		float startCheck = Time.realtimeSinceStartup;
	    	float endCheck = startCheck + tm.intertrialTime;

	    	Debug.Log("Start check is " + startCheck);
	    	Debug.Log("End Check is " + endCheck);

	    	Debug.Log("Real time from startup is " + Time.realtimeSinceStartup);
	    	while ((float) Time.realtimeSinceStartup < endCheck)
	    	{
	    		Debug.Log("Trial is still going on");
	    		if (im.currentAction != null)
	    		{
	    			EventManager.TriggerEvent("InputGiven");
	    			Debug.Log("InputGiven");
	    		} else {
	    			EventManager.TriggerEvent("NoInput");
	    			Debug.Log("NoInput");
	    		}
	    	}
    	}
    }

    // Update is called once per frame
    void Update()
    {
    	tm.TrialTime();
    }
}
