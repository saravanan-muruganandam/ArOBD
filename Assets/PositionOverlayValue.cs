using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PositionOverlayValue : MonoBehaviour {

	[SerializeField]
	private GameObject DistanceTextObject;
	[SerializeField]
	private Collider SizeCollider;
	[SerializeField]
	private GameObject heightValueObject;
	[SerializeField]
	private GameObject WidthvalueObject;

	// Use this for initialization
	void Start () {
		


	}
	
	// Update is called once per frame
	void Update () {
		float height = SizeCollider.bounds.size.y;
		float width = SizeCollider.bounds.size.x;
		DistanceTextObject.GetComponent<TextMeshPro>().text= "Distance: "+ (Math.Round(Vector3.Distance(Camera.main.transform.position, transform.parent.position),1)).ToString()+" m";
		heightValueObject.GetComponent<TextMeshPro>().text = Math.Round(height, 1).ToString()+"m";
		WidthvalueObject.GetComponent<TextMeshPro>().text = Math.Round(width, 1).ToString()+"m";
	}


}
