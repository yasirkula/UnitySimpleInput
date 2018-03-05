using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class KeyInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public SimpleInput.KeyInput key = new SimpleInput.KeyInput();

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;
		}

		void OnEnable()
		{
			key.StartTracking();
		}

		void OnDisable()
		{
			key.StopTracking();
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			key.value = true;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			key.value = false;
		}
	}
}