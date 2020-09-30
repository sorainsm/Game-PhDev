using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ManagerSystems
{
	/**************************************************************************
	Name: InputManager
	Job: Define the functionality of the different inputs. Input hiding is handled by the Unity Input System to keep the input device separate from the action functionality. For the purposes of this work, the input from the player will be written to an output log.
	**************************************************************************/
	public class InputManager : MonoBehaviour
	{
		private Inputs_GNG _controls;
		public InputAction currentAction { get; set; }
		public InputControl currentControl { get; set; }
		public float actionTime { get; set; }

		private void Awake()
		{
			_controls = new Inputs_GNG();
			_controls.Player.Correct.performed += PlayerResponse;
			_controls.Player.Incorrect.performed += PlayerResponse;						
		}

		private void OnEnable()
		{
			_controls.Player.Correct.Enable();
			_controls.Player.Incorrect.Enable();
		}

		private void OnDisable()
		{
			_controls.Player.Correct.performed -= PlayerResponse;
			_controls.Player.Correct.Disable();
			_controls.Player.Incorrect.performed -= PlayerResponse;
			_controls.Player.Incorrect.Disable();
		}

		/**********************************************************************
		Function: PlayerResponse
		Input: context
		Output: -
		Purpose: Because we're just logging the input we only need one function. We store the (logical) action [i.e. correct or incorrect], control pressed, and the time the action was started. This set up allows for any amount of logical actions to be added to the ActionMap in the editor (not via code), and any controls to be added to the controls asset in the editor (not via code) - so we shouldn't have to change this function for different types of challenges, just add the different actions and inputs.

		We then trigger the input event so the output manager will know to log this information immediately.
		**********************************************************************/
		private void PlayerResponse(InputAction.CallbackContext context)
		{
			currentAction = context.action;
			currentControl = context.control;
			actionTime = (float) context.startTime;
			EventManager.TriggerEvent("InputGiven");
		}

		/**********************************************************************
		Function: WhichButton
		Input: context
		Output:
		Purpose: This button figures out which button was pressed regardless of controller type. 

		The big caveat is that the "any key" input for keyboard is a synthetic key - so it just outputs any key not the specific key pressed. This is an issue with the current implementation of the Unity Input System trying to (properly) abstract the controller. We'll need to work around it manually for now (I don't want to write a custom controller for the sake of future proofing - identifying which key is pressed from the any key will eventually be a feature).
		**********************************************************************/
		private InputControl WhichButton(InputAction.CallbackContext context)
		{
			foreach (InputControl k in context.control.device.allControls)
			{				
				if (context.control == k)
				{
					Debug.Log("Key pressed is " + k.ToString());
					return k;
				}
			}
			Debug.LogError("Key not pressed");
			return context.control;
		}

	}
}


