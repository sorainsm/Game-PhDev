using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using CustomDataTypes;

namespace ManagerSystems
{
	public class OutputManager : MonoBehaviour
	{
		//We will need to change the target url when we actually post it.
		string targetURL = "http://localhost/unity/WebGL_Build/outputs.php";

	    public void SendData(OutputData o)
	    {
	    	StartCoroutine(Upload(o));
	    }

	/**************************************************************************
	Name: Upload
	Inputs: o - OutputData data structure
	Outputs: 
	Purpose: This function sends the data to the PHP file on our server which will write it into a text file. These text files will then be parsed to be put into an excel spreadsheet.
	**************************************************************************/
	    IEnumerator Upload(OutputData o)
	    {
	    	List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

	    	/*Add form fields. MultipartFormDataSection takes a string field name, and a string or byte array for data*/
	    	formData.Add(new MultipartFormDataSection("trial-number", o.trialNumber.ToString()));
	    	formData.Add(new MultipartFormDataSection("time-stimulus", o.stimStartTime.ToString()));
	    	formData.Add(new MultipartFormDataSection("trial-type", o.stimType.ToString()));
	    	formData.Add(new MultipartFormDataSection("time-response", o.respStartTime.ToString()));
	    	formData.Add(new MultipartFormDataSection("response", o.respAction.name));
			formData.Add(new MultipartFormDataSection("control", o.control.displayName));	    	    	

	    	/*Send form to PHP file*/
	    	UnityWebRequest www = UnityWebRequest.Post(targetURL, formData);
	    	yield return www.SendWebRequest();

	    	if (www.result != UnityWebRequest.Result.Success)
	    	{
	    		Debug.Log(www.error);
	    	}
	    	else
	    	{
	    		Debug.Log("Form upload complete.");
	    	}
	    }
	}	
}


