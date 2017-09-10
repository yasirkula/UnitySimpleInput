using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class AxisInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private string axis;

		[SerializeField]
		private float value;

		private SimpleInput.AxisInput input = null;

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;

			if( axis != null && axis.Length > 0 )
				input = new SimpleInput.AxisInput( axis );
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
			input.value = value;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			input.value = 0f;
		}
	}
}