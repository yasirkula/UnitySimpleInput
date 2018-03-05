using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class MouseButtonInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public SimpleInput.MouseButtonInput mouseButton = new SimpleInput.MouseButtonInput();

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;
		}

		void OnEnable()
		{
			mouseButton.StartTracking();
		}

		void OnDisable()
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