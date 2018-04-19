using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class MouseButtonInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public SimpleInput.MouseButtonInput mouseButton = new SimpleInput.MouseButtonInput();

		private void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;
		}

		private void OnEnable()
		{
			mouseButton.StartTracking();
		}

		private void OnDisable()
		{
			mouseButton.StopTracking();
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			mouseButton.value = true;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			mouseButton.value = false;
		}
	}
}