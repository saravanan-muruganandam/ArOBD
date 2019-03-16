using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keybordMovement : MonoBehaviour {

	public float speed = 0.1f;
	public UnityEngine.UI.Text outText ;
	public UnityEngine.UI.InputField textComponent;
	public void FixedUpdate()
	{
		int limit = 1;
		if (Input.GetKey("joystick button "+ textComponent.text)) {
			Debug.Log(textComponent.text+"suppored!!");
			outText.text = textComponent.text + "suppored!!";
		}

		Vector3 pointToMove = Vector3.zero;

		if (Input.GetKey(KeyCode.RightArrow))
		{
			pointToMove = Vector3.right * Time.deltaTime * speed;
			if (isInsideScreen(pointToMove)) transform.Translate(pointToMove);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			pointToMove =  Vector3.left * Time.deltaTime * speed;
			if (isInsideScreen(pointToMove)) transform.Translate(pointToMove);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			pointToMove =  Vector3.down * Time.deltaTime * speed;
			if (isInsideScreen(pointToMove)) transform.Translate(pointToMove);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			pointToMove = ( Vector3.up * Time.deltaTime * speed);
			if (isInsideScreen(pointToMove)) transform.Translate(pointToMove);
		}

	}

	public bool isInsideScreen(Vector3 newPosition)
	{
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position + newPosition);
		Debug.Log(pos.x + " *******  " + pos.y);
		//pos.x = Mathf.Clamp01(pos.x);
		//pos.y = Mathf.Clamp01(pos.y);
		if ((pos.x > 0f && pos.x < 1f) && (pos.y > 0f && pos.y < 1f))
		{
			return true;
		}
		return false;
	}
}
