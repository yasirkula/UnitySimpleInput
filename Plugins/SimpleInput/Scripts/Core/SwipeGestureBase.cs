using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInputNamespace
{
	[RequireComponent( typeof( SimpleInputMultiDragListener ) )]
	public abstract class SwipeGestureBase<K, V> : SelectivePointerInput, ISimpleInputDraggableMultiTouch
	{
		public Vector2 swipeAmount = new Vector2( 0f, 25f );

		private SimpleInputMultiDragListener eventReceiver;

		protected abstract BaseInput<K, V> Input { get; }
		protected abstract V Value { get; }

		public abstract int Priority { get; }

		private void Awake()
		{
			eventReceiver = GetComponent<SimpleInputMultiDragListener>();
		}

		private void OnEnable()
		{
			eventReceiver.AddListener( this );
			Input.StartTracking();
		}

		private void OnDisable()
		{
			eventReceiver.RemoveListener( this );
			Input.StopTracking();
		}

		public bool OnUpdate( List<PointerEventData> mousePointers, List<PointerEventData> touchPointers, ISimpleInputDraggableMultiTouch activeListener )
		{
			Input.ResetValue();

			if( activeListener != null && activeListener.Priority > Priority )
				return false;

			PointerEventData pointer = GetSatisfyingPointer( mousePointers, touchPointers );
			if( pointer == null )
				return false;

			if( !IsSwipeSatisfied( pointer ) )
				return ReferenceEquals( activeListener, this );

			Input.value = Value;
			return true;
		}

		private bool IsSwipeSatisfied( PointerEventData eventData )
		{
			Vector2 deltaPosition = eventData.position - eventData.pressPosition;
			if( swipeAmount.x > 0f )
			{
				if( deltaPosition.x < swipeAmount.x )
					return false;
			}
			else if( swipeAmount.x < 0f )
			{
				if( deltaPosition.x > swipeAmount.x )
					return false;
			}

			if( swipeAmount.y > 0f )
			{
				if( deltaPosition.y < swipeAmount.y )
					return false;
			}
			else if( swipeAmount.y < 0f )
			{
				if( deltaPosition.y > swipeAmount.y )
					return false;
			}

			return true;
		}
	}
}