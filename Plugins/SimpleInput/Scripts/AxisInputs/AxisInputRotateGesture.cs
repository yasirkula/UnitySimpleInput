using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInputNamespace
{
	[RequireComponent( typeof( SimpleInputMultiDragListener ) )]
	public class AxisInputRotateGesture : MonoBehaviour, ISimpleInputDraggableMultiTouch
	{
		private const float MULTIPLIER = 180f / Mathf.PI;

		public SimpleInput.AxisInput axis = new SimpleInput.AxisInput();

		public float sensitivity = 0.25f;
		public bool clockwise = true;

		private SimpleInputMultiDragListener eventReceiver;
		
		public int Priority { get { return 2; } }

		private void Awake()
		{
			eventReceiver = GetComponent<SimpleInputMultiDragListener>();
		}

		private void OnEnable()
		{
			eventReceiver.AddListener( this );
			axis.StartTracking();
		}

		private void OnDisable()
		{
			eventReceiver.RemoveListener( this );
			axis.StopTracking();
		}

		public bool OnUpdate( List<PointerEventData> mousePointers, List<PointerEventData> touchPointers, ISimpleInputDraggableMultiTouch activeListener )
		{
			axis.value = 0f;

			if( activeListener != null && activeListener.Priority > Priority )
				return false;

			if( touchPointers.Count < 2 )
			{
#pragma warning disable 0252
				if( activeListener == this && touchPointers.Count == 1 )
					touchPointers[0].pressPosition = touchPointers[0].position;
#pragma warning restore 0252

				return false;
			}

			PointerEventData touch1 = touchPointers[touchPointers.Count - 1];
			PointerEventData touch2 = touchPointers[touchPointers.Count - 2];

			Vector2 deltaPosition = touch2.position - touch1.position;
			Vector2 prevDeltaPosition = deltaPosition - ( touch2.delta - touch1.delta );
			
			float deltaAngle = ( Mathf.Atan2( prevDeltaPosition.y, prevDeltaPosition.x ) - Mathf.Atan2( deltaPosition.y, deltaPosition.x ) ) * MULTIPLIER;
			if( deltaAngle > 180f )
				deltaAngle -= 360f;
			else if( deltaAngle < -180f )
				deltaAngle += 360f;

			axis.value = clockwise ? deltaAngle * sensitivity : -deltaAngle * sensitivity;
			return true;
		}
	}
}