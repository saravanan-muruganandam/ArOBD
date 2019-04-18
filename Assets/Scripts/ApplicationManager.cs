using UnityEngine;
using System.Collections;
using System.Collections.Concurrent;
using obd2NET.OBDJobSchedular;
using System;
using obd2NET;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using GoogleARCore;

public class ApplicationManager : MonoBehaviour {
	
	private  BlockingCollection<OBDCommand> m_queue;
	private  bool PreviousConnectionStatus = true;
	public   static bool CurrentStatus = false;
	public  String IpAddress { get; set; }
	public  String Port { get; set; }
	private static Anchor m_FloatingGraphAnchor;

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	void Start()
	{
		StartCoroutine("UpdateConnectionStatus");
		m_queue = OBDJobService.Instance()._getObdResponseQueue();
		StartCoroutine("ProcessQueueCommands");
	}

	private IEnumerator UpdateConnectionStatus()
	{
		while (true)
		{
			
			CurrentStatus = OBDJobService.Instance().GetVehicleConnectionStatus();
			if (CurrentStatus && !PreviousConnectionStatus) {
				//StartCoroutine("StartConnectionProcess");
				OnConnectedEventTriggered();
				Debug.Log(CurrentStatus);
			}
			else if (!CurrentStatus && PreviousConnectionStatus) {
				//StartCoroutine("StartDISConnectionProcess");
				OnDisConnectedEventTriggered();
				Debug.Log(CurrentStatus);
			}
			PreviousConnectionStatus = CurrentStatus;
			yield return new WaitForSeconds(1.0f);
		}

	}

	IEnumerator ProcessQueueCommands()
	{
		while (true)
		{
			//Debug.Log("RESPONSE COUNT"+m_queue.Count);
			if (m_queue.Count > 0)
			{
				OBDCommand command = m_queue.Take();
				//Debug.Log(vehicle.GetCurrentData(command._obdCommandName));
				//Debug.Log(m_queue.Count);
				List<GameObject> activeSimilarObject = new List<GameObject>(GameObject.FindGameObjectsWithTag(command._obdCommandName));
				//Debug.Log(activeSimilarObject.Count);
				if (activeSimilarObject.Count > 0)
				{
					activeSimilarObject.ForEach(c => UpdateReadingValue(c, command._responseValue));
				}
				else
				{
					Debug.Log("LIST EMPTY");
				}
				
			}
			
			yield return new WaitForSeconds(0.2f);
		}
	}


	private void UpdateReadingValue(GameObject objectToUpdate, String ReaddingValue)
	{
		if (objectToUpdate.GetComponent<TextMeshPro>()!=null) {
			objectToUpdate.GetComponent<TextMeshPro>().text = ReaddingValue;
		}
		else if (objectToUpdate.GetComponent<TextMeshProUGUI>()!=null) {
			objectToUpdate.GetComponent<TextMeshProUGUI>().text = ReaddingValue;
		}

	}

	public void ConnectToVehicle()
	{
		
		OBDJobService.Instance().ConnectToVehicleOBD(IpAddress,Int32.Parse(Port));
		//StartCoroutine("StartConnectionProcess");
	}
	public void CloseConnectionToVehicle()
	{
		OBDJobService.Instance().CloseConnectionToVehicle();
	}

	public void ToogleConnection(bool value)
	{
		Debug.Log(value);
		if (value)
		{
			ConnectToVehicle();
		}
		else {
			CloseConnectionToVehicle();
		}
	}

	public void StopTaskfromFeeding(String TagName)
	{
		
		if (new List<GameObject>(GameObject.FindGameObjectsWithTag(TagName)).Count < 1)
		{
			OBDJobService.Instance().StopAndRemoveTaskFromJobList(TagName);
		}
	}
	/// <summary>
	/// Create common anchor to link the floating objects
	/// </summary>
	public static GameObject GetAnchorForFloatingObjects()
	{
		if (m_FloatingGraphAnchor == null)
		{
			Pose pose;
			pose.position = Camera.main.transform.position + Camera.main.transform.forward * 1f;
			pose.rotation = Quaternion.LookRotation(pose.position - Camera.main.transform.position);
			m_FloatingGraphAnchor = Session.CreateAnchor(pose);
		}
		return m_FloatingGraphAnchor.gameObject;
	}
	IEnumerator StartConnectionProcess()
	{
		while (true)
		{
			if (CurrentStatus)
			{
				OnConnectedEventTriggered();
				Debug.Log(" Connected to the vehicle");
				yield break;
			}
			Debug.Log("Not Connected");
			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator StartDISConnectionProcess()
	{
		while (true)
		{
			if (!CurrentStatus)
			{
				OnDisConnectedEventTriggered();
				Debug.Log(" DisConnected from the vehicle");
				yield break;
			}
			Debug.Log("Still Connected");
			yield return new WaitForSeconds(0.2f);
		}
	}



	public void OnApplicationQuit()
	{
		CloseConnectionToVehicle();
		OBDJobService.Instance()._dispose();
	}

	//my event
	[Serializable]
	public class ConnectEvent : UnityEvent { }
	[Serializable]
	public class DisconnectEvent: UnityEvent { }

	[SerializeField]
	private ConnectEvent OnConnected = new ConnectEvent();
	public ConnectEvent OnConnectedEvent { get { return OnConnected; } set { OnConnected = value; } }
	public void OnConnectedEventTriggered()
	{
		OnConnectedEvent.Invoke();

	}

	[SerializeField]
	private DisconnectEvent OnDisConnected = new DisconnectEvent();
	

	public DisconnectEvent OnDisConnectedEvent { get { return OnDisConnected; } set { OnDisConnected = value; } }
	public void OnDisConnectedEventTriggered()
	{
		OnDisConnectedEvent.Invoke();

	}

}
