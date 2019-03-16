using Aryzon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectOBD : MonoBehaviour {

	// Use this for initialization

	private bool ConnectionStatus = true;
	private Animation backgroundLoadingImage;
	private Text ButtonText;
	private AryzonTracking arytracking;
	void Start() {
		backgroundLoadingImage = transform.Find("Background").GetComponent<Animation>();
		ButtonText = transform.Find("Status Text").GetComponent<Text>();
		arytracking = GameObject.Find("Aryzon").GetComponent<AryzonTracking>();
	}

	// Update is called once per frame
	void Update() {

	}

	public void startConnection(){
		backgroundLoadingImage.Play();

		if (ConnectionStatus)
		{
			backgroundLoadingImage.Stop();
			ButtonText.text = "Connected!";
			
			arytracking.StartAryzonMode();
		}
		


		}
}
