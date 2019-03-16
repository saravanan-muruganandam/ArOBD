//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// ManagerMenuCircular.cs (13/06/2017)											\\
// Autor: Antonio Mateo (Moon Antonio) 	antoniomt.moon@gmail.com				\\
// Descripcion:		Manager del menu circular									\\
// Fecha Mod:		16/06/2017													\\
// Ultima Mod:		Immplementacion de titulo									\\
//******************************************************************************\\


using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoonAntonio.UI
{

	public class MenuManager : MonoBehaviour 
	{
		public ApplicationManager applicationManager;
		public GameObject InitialMenu;
		public static MenuManager instance;             
		private GameObject currentMenu;
		[SerializeField]
		private UnityEngine.Object GraphPrefab;
		[SerializeField]
		private int TotalGraphInstanceAllowed;
		private int totalActiveGraphCounter;
		private void Awake()
		{
			instance = this;
		}

		private void OnEnable()
		{
			instance.OpenPanel(InitialMenu);
		}

		public void Update()
		{
			//Debug.Log(EventSystem.current.currentSelectedGameObject.name);
			if (currentMenu!=null){
				//Debug.Log(currentMenu.transform.localScale);
			}
			//Debug.Log(getCurrentPannel());
			if (Input.GetKeyDown(KeyCode.C))
			{
				CloseCurrentPanel();
			}
			if (Input.GetKeyDown(KeyCode.B))
			{
				if (!InitialMenu.activeSelf)
				{
					//Debug.Log("open");
					instance.CloseCurrentPanel();
					instance.OpenPanel(InitialMenu);
				}

			}
		}

		public  GameObject getCurrentPannel()
		{
			return this.currentMenu;
		}

		public void OpenPanel(GameObject PanelToOpen)// Abre el menu circular.
		{
			//PanelToOpen.SetActive(true);
			if (this.currentMenu == PanelToOpen)
				return;
			SetSelected(FindFirstEnabledSelectable(PanelToOpen));
			PanelToOpen.gameObject.SetActive(true);
			currentMenu = PanelToOpen;
			SetSelected(FindFirstEnabledSelectable(PanelToOpen));
			StartCoroutine(EnableAnimation(PanelToOpen));
			
		}

		public void ToogleGraphInitiate(bool value)
		{
			
			BtnMenuCircular currentGameObject = EventSystem.current.currentSelectedGameObject.GetComponent<BtnMenuCircular>();
			if (value)
			{
				if (totalActiveGraphCounter <= TotalGraphInstanceAllowed)
				{
					currentGameObject.IntiateReadingTask(GraphPrefab);
					totalActiveGraphCounter+= 1;
					Debug.Log(totalActiveGraphCounter+"   "+ TotalGraphInstanceAllowed);
				}

			}
			else if(currentGameObject.getGraphInstance()!=null)
			{
				currentGameObject.DestroyTheGraphInstance();
				applicationManager.StopTaskfromFeeding(currentGameObject.TagName);
				totalActiveGraphCounter-=1;
				Debug.Log(totalActiveGraphCounter);
			}
		}

		public void CloseCurrentPanel()
		{
			SetSelected(null);
			if ((currentMenu == null))
				return;
			
			GameObject toClose = currentMenu;
			currentMenu = null;
			StartCoroutine(DisableAnimation(toClose));
			//toClose.SetActive(false);
		}
		private IEnumerator EnableAnimation(GameObject panelToOpen)
		{
			var buttons = panelToOpen.GetComponentsInChildren<BtnMenuCircular>(true);
			foreach (BtnMenuCircular button in buttons)
				{
					button.AnimacionON();
					yield return null;
				}

		}

		private IEnumerator DisableAnimation(GameObject panelToClose)
		{
			var buttons = panelToClose.GetComponentsInChildren<BtnMenuCircular>(true);
			foreach (BtnMenuCircular button in buttons)
			{

				button.AnimationOUT();
				yield return null;
			}
			//panelToClose.gameObject.SetActive(false);
			
		}

		public void RotateAnimation(GameObject objectToRotate)
		{
			StartCoroutine(RotateAnimicorotine(objectToRotate,Vector3.forward, 360f/5f, 0.5f));
		}

		IEnumerator RotateAnimicorotine(GameObject objectToRotate, Vector3 axis, float angle, float duration = 2.0f)
		{
			Quaternion from = objectToRotate.transform.rotation;
			Quaternion to = objectToRotate.transform.rotation;
			to *= Quaternion.Euler(axis * angle);

			float elapsed = 0.0f;
			while (elapsed < duration)
			{
				objectToRotate.transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
				elapsed += Time.deltaTime;
				yield return null;
			}
			objectToRotate.transform.rotation = to;
		}
		private void SetSelected(GameObject go)
		{
			EventSystem.current.SetSelectedGameObject(go);
		}

		static GameObject FindFirstEnabledSelectable(GameObject gameObject)
		{
			GameObject go = null;
			var selectables = gameObject.GetComponentsInChildren<Selectable>(true);
			foreach (var selectable in selectables)
			{
				if (selectable.IsActive() && selectable.IsInteractable())
				{
					go = selectable.gameObject;
					break;
				}
			}
			return go;
		}



	}
}
