using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInputNamespace
{
	[RequireComponent( typeof( SimpleInputMultiDragListener ) )]
	public class AxisInputMoveGesture : MonoBehaviour, ISimpleInputDraggableMultiTouch
	{
		public SimpleInput.AxisInput horizontal = new SimpleInput.AxisInput( "Mouse X" );
		public SimpleInput.AxisInput vertical = new SimpleInput.AxisInput( "Mouse Y" );

		public float sensitivity = 1f;
		public bool invertValue = true;

		private SimpleInputMultiDragListener eventReceiver;

		public int Priority { get { return 2; } }

		private void Awake()
		{
			eventReceiver = GetComponent<SimpleInputMultiDragListener>();
		}

		private void OnEnable()
		{
			eventReceiver.AddListener( this );

			horizontal.StartTracking();
			vertical.StartTracking();
		}

		private void OnDisable()
		{
			eventReceiver.RemoveListener( this );

			horizontal.StopTracking();
			vertical.StopTracking();
		}

		public bool OnUpdate( List<PointerEventData> mousePointers, List<PointerEventData> touchPointers, ISimpleInputDraggableMultiTouch activeListener )
		{
			horizontal.value = 0f;
			vertical.value = 0f;

			if( activeListener != null && activeListener.Priority > Priority )
				return false;

			if( touchPointers.Count < 2 )
			{
				if( ReferenceEquals( activeListener, this ) && touchPointers.Count == 1 )
					touchPointers[0].pressPosition = touchPointers[0].position;

				return false;
			}

			PointerEventData touch1 = touchPointers[touchPointers.Count - 1];
			PointerEventData touch2 = touchPointers[touchPointers.Count - 2];

			Vector2 pinchAmount = sensitivity * SimpleInputUtils.ResolutionMultiplier * ( touch1.delta + touch2.delta );
			if( invertValue )
				pinchAmount = -pinchAmount;

			horizontal.value = pinchAmount.x;
			vertical.value = pinchAmount.y;

			return true;
		}
	}
}