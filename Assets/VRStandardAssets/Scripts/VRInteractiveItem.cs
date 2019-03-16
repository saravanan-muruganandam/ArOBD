using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRStandardAssets.Utils
{
	// This class should be added to any gameobject in the scene
	// that should react to input based on the user's gaze.
	// It contains events that can be subscribed to by classes that
	// need to know about input specifics to this gameobject.
	public class VRInteractiveItem : MonoBehaviour
	{

		// public event Action OnOver;             // Called when the gaze moves over this object
		// public event Action OnOut;              // Called when the gaze leaves this object
		// public event Action OnClick;            // Called when click input is detected whilst the gaze is over this object.
		// public event Action OnDoubleClick;      // Called when double click input is detected whilst the gaze is over this object.
		// public event Action OnUp;               // Called when Fire1 is released whilst the gaze is over this object.
		// public event Action OnDown;             // Called when Fire1 is pressed whilst the gaze is over this object.

		private float ObjectInitialDistance;
		private bool InitialDistanceUpdated = false;
			
		[Serializable]
		public class OnOverEvent : UnityEvent { }
		[SerializeField]
		private OnOverEvent onOverEvent = new OnOverEvent();
		public OnOverEvent OnOver { get { return onOverEvent; } set { onOverEvent = value; } }

		[Serializable]
		public class OnOutEvent : UnityEvent { }
		[SerializeField]
		private OnOutEvent onOutEvent = new OnOutEvent();
		public OnOutEvent OnOut { get { return onOutEvent; } set { onOutEvent = value; } }

		[Serializable]
		public class OnDoubleClickEvent : UnityEvent { }
		[SerializeField]
		private OnDoubleClickEvent onDoubleClickEvent = new OnDoubleClickEvent();
		public OnDoubleClickEvent OnDoubleClick { get { return onDoubleClickEvent; } set { onDoubleClickEvent = value; } }

		[Serializable]
		public class OnClickEvent : UnityEvent { }
		[SerializeField]
		private OnClickEvent onClickEvent = new OnClickEvent();
		public OnClickEvent OnClick { get { return onClickEvent; } set { onClickEvent = value; } }

		[Serializable]
		public class OnUpEvent : UnityEvent { }
		[SerializeField]
		private OnUpEvent onUpEvent = new OnUpEvent();
		public OnUpEvent OnUp { get { return onUpEvent; } set { onUpEvent = value; } }

		[Serializable]
		public class OnDownEvent : UnityEvent { }
		[SerializeField]
		private OnDownEvent onDownEvent = new OnDownEvent();
		public OnDownEvent OnDown { get { return onDownEvent; } set { onDownEvent = value; } }


		public bool autoClick = false;
		private float autoClickTime = 1f;
		private float clickTimerState = 0f;
		private bool clicked = false;

        protected bool m_IsOver;

		private void Update () {
			if (autoClick && m_IsOver && !clicked) {
				clickTimerState += Time.deltaTime;
				if (clickTimerState >= autoClickTime) {
					clicked = true;
					Click ();
				}
			}
		}

        public bool IsOver
        {
            get { return m_IsOver; }              // Is the gaze currently over this object?
        }

		public void setAutoClickTime(float time) {
			autoClickTime = time;
		}

        // The below functions are called by the VREyeRaycaster when the appropriate input is detected.
        // They in turn call the appropriate events should they have subscribers.
        public void Over()
        {
            m_IsOver = true;

            if (OnOver != null)
				OnOver.Invoke();
		}


        public void Out()
        {
            m_IsOver = false;
			clicked = false;
			clickTimerState = 0f;

            if (OnOut != null)
                OnOut.Invoke();
        }


        public void Click()
        {
            if (OnClick != null)
                OnClick.Invoke();
        }


        public void DoubleClick()
        {
            if (OnDoubleClick != null)
                OnDoubleClick.Invoke();
        }


        public void Up()
        {
            if (OnUp != null)
                OnUp.Invoke();
        }


        public void Down()
        {
            if (OnDown != null)
				OnDown.Invoke();
        }

		public void LerpToNewDragPosition(Vector3 RayOrigin, Vector3 direction)
		{

		}
    }
}