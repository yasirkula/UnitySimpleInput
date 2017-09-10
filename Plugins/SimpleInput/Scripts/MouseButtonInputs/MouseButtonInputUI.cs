using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class MouseButtonInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private int button;

		private SimpleInput.MouseButtonInput input = null;

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;

			input = new SimpleInput.MouseButtonInput( button );
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