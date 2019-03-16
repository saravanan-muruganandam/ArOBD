using obd2NET.OBDJobSchedular;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour {

	public Text messageText;

	private Color currentColor;
	
	// Use this for initialization
	void Start () {
		currentColor = messageText.color;

		//StartCoroutine("StartFadeInFadeOutOnDisconnect");
	}
	
	// Update is called once per frame
	void Update () {


	}

	public void ChangeTextColorForConnected()
	{
		StopCoroutine("StartFadeInFadeOutOnDisconnect");
		messageText.color = Color.green;
		messageText.text = "CONNECTED";
	}

	public void ChangeTextColorForDisconnected()
	{
		StartCoroutine("StartFadeInFadeOutOnDisconnect");
	}

	private  IEnumerator StartFadeInFadeOutOnDisconnect()
	{
		messageText.text = "DISCONNECTED";
		messageText.color = Color.red;
		while (true)
		{
			messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, (Mathf.Sin(Time.time * 3) + 1) / 2);
			yield return null;
		}
	}
}
