using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerExample : EventTrigger
{
	public void OnDisconnect() => Debug.Log("OnBeginOnDisconnect called.");


}