using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInputNamespace
{
	public class SimpleInputDragListener : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
	{
		public ISimpleInputDraggable Listener { get; set; }

		private int pointerId = -99;

		public void OnPointerDown( PointerEventData eventData )
		{
			if( pointerId != -99 )
				return;

			Listener.OnPointerDown( eventData );
			pointerId = eventData.pointerId;
		}

		public void OnDrag( PointerEventData eventData )
		{
			if( pointerId != eventData.pointerId )
			{
				eventData.pointerDrag = null;
				return;
			}

			Listener.OnDrag( eventData );
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			if( pointerId != eventData.pointerId )
				return;

			Listener.OnPointerUp( eventData );
			pointerId = -99;
		}
	}
}