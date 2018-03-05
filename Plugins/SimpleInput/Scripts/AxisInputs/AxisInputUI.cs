using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class AxisInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private float value;

		public SimpleInput.AxisInput axis = new SimpleInput.AxisInput();

		void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = true;
		}

		void OnEnable()
		{
			axis.StartTracking();
		}

		void OnDisable()
		{
			axis.StopTracking();
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			axis.value = value;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			axis.value = 0f;
		}
	}
}