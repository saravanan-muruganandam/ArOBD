using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionErrorHandler : MonoBehaviour {

	[SerializeField]
	private InputField IPAddressInputFeild;
	[SerializeField]
	private InputField PortInputFeild;
	[SerializeField]
	private Text ErrorNotificationText;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
		{
			foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKey(kcode))
				{
					Debug.Log("KeyCode down: " + kcode);
					ErrorNotificationText.text = kcode.ToString();
				}

			}
		}


	}

	public bool ChaeckIPandPort()
	{
		if (string.IsNullOrEmpty(IPAddressInputFeild.text))
		{
			ErrorNotificationText.text = "IP Address box is Empty";
			Debug.Log(ErrorNotificationText.text);
			return false;
		}
		else if (string.IsNullOrEmpty(PortInputFeild.text))
		{
			ErrorNotificationText.text = "Port box is Empty";
			Debug.Log(ErrorNotificationText.text);
			return false;
		}
		return true;
	}

	public void ToogleError(bool value)
	{
		StopCoroutine("CheckCOnnectionCoroutine");
		if (ChaeckIPandPort()) {
			StartCoroutine("CheckCOnnectionCoroutine", value);
		}

	}

	IEnumerator CheckCOnnectionCoroutine(bool value)
	{
		float TimeToStop = 0.0f;
		while (true)
		{
			if (value==true)
			{
				if (ApplicationManager.CurrentStatus==true)
				{
					ErrorNotificationText.text = "Connected";
					Debug.Log(ErrorNotificationText.text);
					yield break;
				}
				else
				{
					if (TimeToStop > 10.0f)
					{
						ErrorNotificationText.text = "Not able to Connect. Check IP and Port";
						yield break;
					}
					ErrorNotificationText.text = "Connecting...";
					TimeToStop += 0.5f;
					Debug.Log(ErrorNotificationText.text);
				}
			}
			else
			{
				if (ApplicationManager.CurrentStatus==false)
				{
					ErrorNotificationText.text = "Disconnected"; ;
					Debug.Log(ErrorNotificationText.text);
					yield break;
				}
				else
				{
					if (TimeToStop > 10.0f)
					{
						ErrorNotificationText.text = "Not able to Disconnect.";
						yield break;
					}
					ErrorNotificationText.text = "Disconnecting...";
					Debug.Log(ErrorNotificationText.text);
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
