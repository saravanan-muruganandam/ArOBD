using MoonAntonio.UI;
using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // In order to interact with objects in the scene
    // this class casts a ray into the scene and if it finds
    // a VRInteractiveItem it exposes it for other classes to use.
    // This script should be generally be placed on the camera.
    public class VREyeRaycaster : MonoBehaviour
    {
        public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.

        [SerializeField] private Transform m_Camera;
        [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
        [SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
        [SerializeField] private VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
        [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
        [SerializeField] private float m_DebugRayLength = 50f;          // Debug ray length.
        [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
        [SerializeField] private float m_RayLength = 5000f;
		[SerializeField] private Transform directionObject;  // How far into the scene the ray is cast.
		[SerializeField] private MenuManager m_PanelManager;

		[SerializeField] private float autoClickTime = 1f;              //The time it takes to auto click when gazing at an object
        
        private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
        private VRInteractiveItem m_LastInteractible;                   //The last interactive item
		private VRInteractiveItem m_CurrentSelectedForDrag;

		private bool m_DragingFlag = false;
		private float ObjectInitialDistance;
		private bool InitialDistanceUpdated;

		//public Transform direction; 

		// Utility for other classes to get the current interactive item
		public VRInteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        
        private void OnEnable()
        {
            m_VrInput.OnClick += HandleClick;
            m_VrInput.OnDoubleClick += HandleDoubleClick;
            m_VrInput.OnUp += HandleUp;
            m_VrInput.OnDown += HandleDown;
			m_VrInput.OnDrag += HandleDrag;
			m_VrInput.OnStart += HandleStart;
			m_VrInput.OnCancel += HandleCancel;
			

		}


        private void OnDisable ()
        {
            m_VrInput.OnClick -= HandleClick;
            m_VrInput.OnDoubleClick -= HandleDoubleClick;
            m_VrInput.OnUp -= HandleUp;
            m_VrInput.OnDown -= HandleDown;
			m_VrInput.OnDrag -= HandleDrag;
			m_VrInput.OnStart -= HandleStart;
			m_VrInput.OnCancel -= HandleCancel;
			
		}

		private void Start()
		{
			Debug.Log(directionObject.name);
			 //direction.position = m_Camera.forward * 5f;

		}
		private void Update()
        {
            EyeRaycast();
        }

        private void EyeRaycast()
        {
			
			// Show the debug ray if required
			if (m_ShowDebugRay)
            {
				Debug.DrawRay(m_Camera.position, directionObject.position* m_DebugRayLength, Color.blue);
			}
			//Debug.Log(m_CurrentInteractible);
			// Create a ray that points forwards from the camera.
			// Ray ray = new Ray(m_Camera.position, directionObject);
			Ray ray = new Ray(m_Camera.position, directionObject.position* m_DebugRayLength);
			RaycastHit hit;
			// Do the raycast forweards to see if we hit an interactive item
			if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
            {
                VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
				
				if (!m_DragingFlag) {
					m_CurrentInteractible = interactible;
					// If we hit an interactive item and it's not the same as the last interactive item, then call Over
					if (interactible && interactible != m_LastInteractible)
					{
						interactible.setAutoClickTime(autoClickTime);
						interactible.Over();
						m_Reticle.fillInTime(autoClickTime);
					}
				}

                // Deactive the last interactive item 
                if (interactible != m_LastInteractible)
                    DeactiveLastInteractible();

                m_LastInteractible = interactible;

                // Something was hit, set at the hit position.
                if (m_Reticle)
                    m_Reticle.SetPosition(hit);
					

				if (OnRaycasthit != null)
                    OnRaycasthit(hit);
            }
            else
            {
                // Nothing was hit, deactive the last interactive item.
                DeactiveLastInteractible();
                m_CurrentInteractible = null;

				// Position the reticle at default distance.
				//Vector3 moveVector = (Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical"));				

				if (m_Reticle)
					m_Reticle.SetPosition(directionObject.position * m_DebugRayLength);
			}

		}

		public bool IsInsideScreen(Vector3 newPosition)
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
		private void DeactiveLastInteractible()
        {
            if (m_LastInteractible == null)
                return;

			m_Reticle.stopFilling ();
			if (!m_DragingFlag) {
				m_LastInteractible.Out();
			}
            m_LastInteractible = null;
        }


        private void HandleUp()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Up();
				m_DragingFlag = false;
				InitialDistanceUpdated = false;
				m_CurrentSelectedForDrag = null;
		}


        private void HandleDown()
        {
			//Debug.Log("DOWN");
			if (m_CurrentInteractible != null)
                m_CurrentInteractible.Down();
        }


        private void HandleClick()
        {
			//Debug.Log ("Clicked()");
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Click();
        }


        private void HandleDoubleClick()
        {
			//Debug.Log("DoubleClick()");
			if (m_CurrentInteractible != null)
                m_CurrentInteractible.DoubleClick();

        }

		private void HandleDrag()
		{
			m_DragingFlag = true;
			if (m_CurrentInteractible != null)
			{
				m_CurrentSelectedForDrag = m_CurrentInteractible;
			}
			if (m_CurrentSelectedForDrag != null)
			{
				if (!InitialDistanceUpdated)
				{
					ObjectInitialDistance = Vector3.Distance(m_Camera.position, m_CurrentSelectedForDrag.gameObject.transform.position);
				}
				InitialDistanceUpdated = true;
				Debug.Log(Input.GetAxis("ScaleAxis"));
				ObjectInitialDistance += Input.GetAxis("MoveFront&Back");
				m_CurrentSelectedForDrag.gameObject.transform.localScale = Vector3.Lerp(m_CurrentSelectedForDrag.gameObject.transform.localScale, m_CurrentSelectedForDrag.gameObject.transform.localScale+(Vector3.one*Input.GetAxis("ScaleAxis")), Time.deltaTime/2f );
				m_CurrentSelectedForDrag.gameObject.transform.rotation = Quaternion.LookRotation(m_CurrentSelectedForDrag.gameObject.transform.position - Camera.main.transform.position);
				m_CurrentSelectedForDrag.gameObject.transform.position = Vector3.Lerp(m_CurrentSelectedForDrag.gameObject.transform.position, m_Camera.position 
					+ (directionObject.position.normalized * ObjectInitialDistance ), Time.deltaTime * 10.0f);

			}
		}

		private void HandleStart() {

			m_PanelManager.CloseCurrentPanel();
			m_PanelManager.OpenPanel(m_PanelManager.InitialMenu);

		}


		private void HandleCancel() {
			m_PanelManager.CloseCurrentPanel();
		
		}



    }
}