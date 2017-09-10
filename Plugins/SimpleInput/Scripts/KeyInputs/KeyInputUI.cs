using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class KeyInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private KeyCode key;

		private SimpleInput.KeyInput input = null;

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;

			input = new SimpleInput.KeyInput( key );
		}

		void OnEnable()
		{
			if( input != null )
				input.StartTracking();
		}

		void OnDisable()
		{
			if( input != null )
				input.StopTracking();
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			input.isDown = true;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			input.isDown = false;
		}
	}
}