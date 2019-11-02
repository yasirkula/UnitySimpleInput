using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	[DisallowMultipleComponent]
	[RequireComponent( typeof( RectTransform ) )]
	public class SimpleInputMultiDragListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private const float POINTERS_VALIDATION_INTERVAL = 5f;

		private List<ISimpleInputDraggableMultiTouch> listeners = new List<ISimpleInputDraggableMultiTouch>( 1 );
		private ISimpleInputDraggableMultiTouch activeListener;

		private List<PointerEventData> mousePointers = new List<PointerEventData>();
		private List<PointerEventData> touchPointers = new List<PointerEventData>();
		private List<PointerEventData> validPointers = new List<PointerEventData>();

		private float pointersNextValidation = POINTERS_VALIDATION_INTERVAL;

		private void Awake()
		{
			Graphic graphic = GetComponent<Graphic>();
			if( !graphic )
				graphic = gameObject.AddComponent<NonDrawingGraphic>();

			graphic.raycastTarget = true;
		}

		private void OnEnable()
		{
			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			SimpleInput.OnUpdate -= OnUpdate;
		}

		public void AddListener( ISimpleInputDraggableMultiTouch listener )
		{
			int priority = listener.Priority;
			int i = 0;
			while( i < listeners.Count && listeners[i].Priority < priority )
				i++;

			listeners.Insert( i, listener );
		}

		public void RemoveListener( ISimpleInputDraggableMultiTouch listener )
		{
			listeners.Remove( listener );

			if( activeListener == listener )
				activeListener = null;
		}

		private void OnUpdate()
		{
			pointersNextValidation -= Time.unscaledDeltaTime;
			if( pointersNextValidation <= 0f )
			{
				pointersNextValidation = POINTERS_VALIDATION_INTERVAL;
				ValidatePointers();
			}

			for( int i = listeners.Count - 1; i >= 0; i-- )
			{
				if( listeners[i].OnUpdate( mousePointers, touchPointers, activeListener ) )
				{
					if( activeListener == null || activeListener.Priority < listeners[i].Priority )
						activeListener = listeners[i];
				}
				else if( activeListener == listeners[i] )
					activeListener = null;
			}

			for( int i = 0; i < mousePointers.Count; i++ )
				mousePointers[i].delta = new Vector2( 0f, 0f );

			for( int i = 0; i < touchPointers.Count; i++ )
				touchPointers[i].delta = new Vector2( 0f, 0f );
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			List<PointerEventData> pointers = eventData.IsTouchInput() ? touchPointers : mousePointers;

			for( int i = 0; i < pointers.Count; i++ )
			{
				if( pointers[i].pointerId == eventData.pointerId )
				{
					pointers[i] = eventData;
					return;
				}
			}

			pointers.Add( eventData );
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			for( int i = 0; i < mousePointers.Count; i++ )
			{
				if( mousePointers[i].pointerId == eventData.pointerId )
				{
					mousePointers.RemoveAt( i );
					break;
				}
			}

			for( int i = 0; i < touchPointers.Count; i++ )
			{
				if( touchPointers[i].pointerId == eventData.pointerId )
				{
					touchPointers.RemoveAt( i );
					break;
				}
			}
		}

		private void ValidatePointers()
		{
			for( int i = mousePointers.Count - 1; i >= 0; i-- )
			{
				if( !Input.GetMouseButton( (int) mousePointers[i].button ) )
					mousePointers.RemoveAt( i );
			}

			for( int i = Input.touchCount - 1; i >= 0; i-- )
			{
				int fingerId = Input.GetTouch( i ).fingerId;
				for( int j = 0; j < touchPointers.Count; j++ )
				{
					if( touchPointers[j].pointerId == fingerId )
					{
						validPointers.Add( touchPointers[j] );
						break;
					}
				}
			}

			List<PointerEventData> temp = touchPointers;
			touchPointers = validPointers;
			validPointers = temp;
			validPointers.Clear();
		}
	}
}