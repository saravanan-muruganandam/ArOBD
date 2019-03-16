using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
namespace MoonAntonio.UI
{
	public static class ScrollRectExtensions
	{
		public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
		{
			Canvas.ForceUpdateCanvases();
			Vector2 viewportLocalPosition = instance.viewport.localPosition;
			Vector2 childLocalPosition = child.localPosition;
			Vector2 result = new Vector2(
				//0 - (viewportLocalPosition.x + childLocalPosition.x),
				0 - (viewportLocalPosition.x),
				0 - (viewportLocalPosition.y + childLocalPosition.y)
			);
			return result;
		}
	}

	public class RepositionScroll : MonoBehaviour
	{
		private ScrollRect scrollRect;
		private void Start()
		{
			scrollRect = transform.gameObject.GetComponent<ScrollRect>();
		}
		private void Update()
		{
			Debug.Log(EventSystem.current.currentSelectedGameObject);
			if ((transform.gameObject.activeSelf) && EventSystem.current.currentSelectedGameObject!=null)
			{
				Vector3 newPosition = scrollRect.GetSnapToPositionToBringChildIntoView(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
				scrollRect.content.localPosition = Vector3.Lerp(scrollRect.content.localPosition, newPosition, Time.deltaTime * 2.0f);

			}
		}

	}
}