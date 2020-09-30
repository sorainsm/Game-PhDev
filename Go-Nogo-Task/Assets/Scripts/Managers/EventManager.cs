using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ManagerSystems
{
	public class EventManager : MonoBehaviour
	{
		private Dictionary <string, UnityEvent> eventDictionary; //List of all events in our game

		private static EventManager eventManager;

		public static EventManager instance
		{
			get
			{
				if (!eventManager)
				{
					eventManager = GameObject.FindObjectOfType<EventManager>();

					if (!eventManager)
					{
						//Debug.LogError("Needs to be one active EventManager script on a GameObject in your scene.");
					} else {
						eventManager.InitializeEventManager();
					}
				}
				return eventManager;
			}
		}

		void InitializeEventManager()
		{
			if (eventDictionary == null)
			{
				eventDictionary = new Dictionary<string, UnityEvent>();
			}
		}

		public static void StartListening(string eventName, UnityAction listener)
		{
			UnityEvent thisEvent = null;

			if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.AddListener(listener);	
			} else {
				thisEvent = new UnityEvent();
				thisEvent.AddListener(listener);
				instance.eventDictionary.Add(eventName,thisEvent);
			}
		}

		public static void StopListening(string eventName, UnityAction listener)
		{
			if (eventManager == null) return;

			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.RemoveListener(listener);
			}
		}

		public static void TriggerEvent(string eventName)
		{
			//Debug.Log("TriggerEvent is started");
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				//Debug.Log("Event is found. Trying to Invoke.");
				thisEvent.Invoke();
			}
		}

	}
}