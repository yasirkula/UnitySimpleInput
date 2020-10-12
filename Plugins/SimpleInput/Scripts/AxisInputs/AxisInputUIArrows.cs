using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInputNamespace
{
	public class AxisInputUIArrows : MonoBehaviour, ISimpleInputDraggable
	{
		public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput( "Horizontal" );
		public SimpleInput.AxisInput yAxis = new SimpleInput.AxisInput( "Vertical" );

		public float valueMultiplier = 1f;

#pragma warning disable 0649
		[Tooltip( "Radius of the deadzone at the center of the arrows that will yield no input" )]
		[SerializeField]
		private float deadzoneRadius;
		private float deadzoneRadiusSqr;
#pragma warning restore 0649

		private RectTransform rectTransform;

		private Vector2 m_value = Vector2.zero;
		public Vector2 Value { get { return m_value; } }

		private void Awake()
		{
			rectTransform = (RectTransform) transform;
			gameObject.AddComponent<SimpleInputDragListener>().Listener = this;

			deadzoneRadiusSqr = deadzoneRadius * deadzoneRadius;
		}

		private void OnEnable()
		{
			xAxis.StartTracking();
			yAxis.StartTracking();
		}

		private void OnDisable()
		{
			xAxis.StopTracking();
			yAxis.StopTracking();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			deadzoneRadiusSqr = deadzoneRadius * deadzoneRadius;
		}
#endif

		public void OnPointerDown( PointerEventData eventData )
		{
			CalculateInput( eventData );
		}

		public void OnDrag( PointerEventData eventData )
		{
			CalculateInput( eventData );
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			m_value = Vector2.zero;

			xAxis.value = 0f;
			yAxis.value = 0f;
		}

		private void CalculateInput( PointerEventData eventData )
		{
			Vector2 pointerPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( rectTransform, eventData.position, eventData.pressEventCamera, out pointerPos );

			if( !yAxis.IsKeyValid() )
			{
				if( pointerPos.x * pointerPos.x <= deadzoneRadiusSqr )
					m_value.Set( 0f, 0f );
				else
					m_value.Set( pointerPos.x >= 0f ? valueMultiplier : -valueMultiplier, 0f );
			}
			else if( !xAxis.IsKeyValid() )
			{
				if( pointerPos.y * pointerPos.y <= deadzoneRadiusSqr )
					m_value.Set( 0f, 0f );
				else
					m_value.Set( 0f, pointerPos.y >= 0f ? valueMultiplier : -valueMultiplier );
			}
			else
			{
				if( pointerPos.sqrMagnitude <= deadzoneRadiusSqr )
					m_value.Set( 0f, 0f );
				else
				{
					float angle = Vector2.Angle( pointerPos, Vector2.right );
					if( pointerPos.y < 0f )
						angle = 360f - angle;

					if( angle >= 45f && angle <= 135f )
						m_value.Set( 0f, valueMultiplier );
					else if( angle >= 135f && angle <= 225f )
						m_value.Set( -valueMultiplier, 0f );
					else if( angle >= 225f && angle <= 315f )
						m_value.Set( 0f, -valueMultiplier );
					else
						m_value.Set( valueMultiplier, 0f );
				}
			}

			xAxis.value = m_value.x;
			yAxis.value = m_value.y;
		}
	}
}