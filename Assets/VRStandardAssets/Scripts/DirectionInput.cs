using MoonAntonio.UI;
using UnityEngine;
namespace VRStandardAssets.Utils
{
	public class DirectionInput : MonoBehaviour
	{

		public float moveSpeed;
		private Vector3 lastVisibleLocalPos;
		

		private void Start()
		{
			//transform.position = Camera.main.transform.forward * 1f;
			transform.position = transform.parent.forward * 6f;
		}
		void Update()
		{
			if (MenuManager.instance.getCurrentPannel()==null) {
				//Debug.Log(ManagerMenuCircular.instance.getCurrentPannel());
				//Debug.Log((Mathf.Round(Input.GetAxis("Vertical") * 10f) / 10f));
				Vector3 moveVector = (Vector3.right * (Mathf.Round(Input.GetAxis("Horizontal") * 10f) / 10f ) + Vector3.down * (Mathf.Round(-Input.GetAxis("Vertical") * 10f) / 10f) );
				BringInsideViewport(moveVector);
			}

	}

		public bool BringInsideViewport(Vector3 newPosition)
		{
			Vector3 toMovePositionLocal = transform.position + (newPosition * moveSpeed * Time.deltaTime);
			Vector3 pos = Camera.main.WorldToViewportPoint(toMovePositionLocal);

			if ((pos.x > 0.0000000f && pos.x < 0.7000000f) && (pos.y > 0.1000000f && pos.y < 0.9000000f) )
			{
				//Debug.Log(pos.x + "         " + pos.y);
				lastVisibleLocalPos = transform.localPosition;
				transform.localPosition = transform.localPosition + (newPosition * moveSpeed * Time.deltaTime);

				return true;
			}
			else
			{
				//transform.localPosition = lastVisibleLocalPos;
			}
			return false;
		}
	}
}