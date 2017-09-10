using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class ButtonInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private string button;

		private SimpleInput.ButtonInput input = null;

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;

			if( button != null && button.Length > 0 )
				input = new SimpleInput.ButtonInput( button );
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