
#region Librerias
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using GoogleARCore.Examples.HelloAR;
#endregion

namespace MoonAntonio.UI
{

	public class BtnMenuCircular : MonoBehaviour
	{


		public float AnimatioTimer = 0.8f;
		public String TagName;
		public GameObject valueObject;
		[SerializeField]
		private bool ISInitiateGraphOnCLick;
		private GameObject GraphObject;
		private Button ObjectButton;
		private Color ButtonBckgrndInitialColor;
		private bool isFeeding;

		public void Start()
		{
			ObjectButton = transform.gameObject.GetComponent<Button>();
			

		}
		private IEnumerator AnimacionBotonesEnable()
		{
			// Asignacion de variables
			
			transform.localScale = Vector3.zero;
			float timer = 0.0f;

			// Bucle
			while (timer < (1 / AnimatioTimer))
			{
				// Animacion
				timer += Time.deltaTime;
				transform.localScale = Vector3.one * timer * AnimatioTimer;

				//Debug.Log("ON SCALE IN " + transform.gameObject.name + "  " + transform.localScale.ToString());
				yield return null;
			}
			// Reseteo
			
			transform.localScale = Vector3.one;
		}


			private IEnumerator AnimacionBotonesout()// Inicio de animacion de los botones
		{
			// Asignacion de variables
			
			transform.localScale = Vector3.one;
			float timer = 0.0f;

			
			while (timer < (1 / AnimatioTimer))
			{
				
				timer += Time.deltaTime;
				transform.localScale = Vector3.one - (Vector3.one * timer * AnimatioTimer);
				//Debug.Log("ON SCALE OUT"+transform.gameObject.name + "  " + transform.localScale.ToString());
				yield return null;
			}
			
			transform.localScale = Vector3.zero;
		}

		public void AnimacionON()// LLama a la animacion de los botones
		{
			StartCoroutine(AnimacionBotonesEnable());
		}

		public void AnimationOUT()
		{
			StartCoroutine(AnimacionBotonesout());
		}

		public GameObject getGraphInstance()
		{
			return this.GraphObject;
		}

		public bool IsActiveFeeding()
		{
			return isFeeding;
		}
		public void IntiateReadingTask(UnityEngine.Object GraphPrefab)
		{
			if (this.GraphObject == null) {
				if (TagName != null )
				{
					isFeeding = true;
					ButtonBckgrndInitialColor = transform.Find("ActiveFlagImage").GetComponent<Image>().color;
					SetColorAlpha(transform.Find("ActiveFlagImage").gameObject, new Color(0f, 0.5678f, 1f, 1f));

					UpdateValueObjectValueOnButton(TagName);
					//
					if (ISInitiateGraphOnCLick) {
						this.GraphObject = Instantiate(GraphPrefab, transform.position, Camera.main.transform.rotation) as GameObject;
						this.GraphObject.gameObject.SetActive(true);
						this.GraphObject.GetComponentInChildren<graph>().UpdateValueObjectTag(TagName);
						this.GraphObject.GetComponentInChildren<graph>().UpdateTitleName(TagName);
						this.GraphObject.name = TagName + "_Graph_" + GameObject.FindGameObjectsWithTag(TagName).Length;
						StartCoroutine(GraphInitiationAnimation(this.GraphObject));
					}

					

				}
			}

		}

		private void  SetColorAlpha(GameObject ObjectColorToChange,Color Value)
		{
			ObjectColorToChange.GetComponent<Image>().color = Value;
		}

		public void DestroyTheGraphInstance()
		{
			if (this.GraphObject != null)
			{
				if (ButtonBckgrndInitialColor!=null) {
					SetColorAlpha(transform.Find("ActiveFlagImage").gameObject, ButtonBckgrndInitialColor);
				}
				valueObject.SetActive(false);
				Destroy(GraphObject);
				this.GraphObject = null;
				isFeeding = true;

			}
		}


		private void UpdateValueObjectValueOnButton(String TagName) {
			if (valueObject != null)
			{
				valueObject.SetActive(true);
				valueObject.tag = TagName;
			}
			
		}

		private IEnumerator GraphInitiationAnimation(GameObject newGraph)
		{
			Vector3 initialScale = newGraph.transform.localScale;
			Vector3 initialPosition = newGraph.transform.position;
			//Vector3 toPosition = Camera.main.transform.position + Camera.main.transform.forward * 1f;
			Vector3 toPosition = Camera.main.transform.position + new Vector3(-initialPosition.x, initialPosition.y, 3.0f);
			newGraph.transform.localScale = Vector3.zero;
			float timer = 0.0f;

			while (timer < 3f && newGraph)
			{
				newGraph.transform.localScale = Vector3.Lerp(newGraph.transform.localScale, initialScale, timer / 5f);
				newGraph.transform.position = Vector3.Lerp(initialPosition, toPosition, timer);
				newGraph.transform.rotation = Quaternion.LookRotation(newGraph.transform.position - Camera.main.transform.position);
				timer += Time.deltaTime;
				yield return null;
			}

			newGraph.transform.parent = ApplicationManager.GetAnchorForFloatingObjects().transform;
			Debug.Log("anchr added");
		}
	}

	/*
	[CustomEditor(typeof(BtnMenuCircular))]
	public class MyScriptEditor : Editor
	{
		override public void OnInspectorGUI()
		{
			var myScript = target as BtnMenuCircular;

			myScript.ISInitiateGraphOnCLick = GUILayout.Toggle(myScript.ISInitiateGraphOnCLick, "Initiate Graph");
			if (myScript.ISInitiateGraphOnCLick)
			{
				myScript.TagName = EditorGUILayout.TextField("TagName", myScript.TagName);
				myScript.valueObject = (GameObject)EditorGUILayout.ObjectField("ValueObject",myScript.valueObject, typeof(GameObject),true);
			}
		}
	}
	*/
}
