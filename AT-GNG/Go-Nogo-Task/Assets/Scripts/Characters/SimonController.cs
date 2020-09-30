/******************************************************************************
Class: SimonController
Purpose: This class is meant to control the animation behaviour of the target character (aka Simon). It will receive the target type from the Experiment Manager and return the correct animation.
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using EnumTypes;
using ManagerSystems;

public class SimonController : MonoBehaviour
{
	private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
    	//Debug.Log("Simon Start Up");
    	anim = GetComponent<Animator>();
        if (!anim)
        {
            //Debug.LogError("No animator registered.");
        }
        anim.SetTrigger("StartUp");
    }

    /**************************************************************************
    	The purpose of this function is to take in the trial type and play the matching animation. AnimatorController labelled SimonAnimator has the animation clips for the different views.

    	Animations were chosen to denote trial types instead of colour changes because they more closely emulate what would be seen in a game (character models doing different actions that they need to react to). The Win and Lose poses were chosen because they are sufficiently different from each other - with the Win pose moving the character up in the screen, and Lose pose moving them down.
    **************************************************************************/
    
    void OnEnable()
    {
        EventManager.StartListening("Target", AnimTarget);
        EventManager.StartListening("Nontarget", AnimNontarget);
    }

    void OnDisable()
    {
        EventManager.StopListening("Target", AnimTarget);
        EventManager.StopListening("Nontarget", AnimNontarget);
    }


    void AnimTarget()
    {
        //Debug.Log("Simon target animation.");
        anim.SetTrigger("Target");
    }

    void AnimNontarget()
    {
        //Debug.Log("Simon non-target animation.");
        anim.SetTrigger("Nontarget");
    }

    public bool AnimatorIsPlaying()
    {
    	return anim.GetCurrentAnimatorStateInfo(0).length > anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
