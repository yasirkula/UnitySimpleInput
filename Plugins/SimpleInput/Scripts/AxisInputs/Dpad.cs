using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class Dpad : MonoBehaviour, ISimpleInputDraggable
	{
		[SerializeField]
		private string xAxis = "Horizontal";
		[SerializeField]
		private string yAxis = "Vertical";

		private SimpleInput.AxisInput xInput = null;
		private SimpleInput.AxisInput yInput = null;

		private RectTransform rectTransform;
		private Graphic background;

		private Vector2 m_value = Vector2.zero;
		public Vector2 Value { get { return m_value; } }

		void Awake()
		{
			if( xAxis != null && xAxis.Length > 0 )
				xInput = new SimpleInput.AxisInput( xAxis );

			if( yAxis != null && yAxis.Length > 0 )
				yInput = new SimpleInput.AxisInput( yAxis );

			rectTransform = (RectTransform) transform;
			background = GetComponent<Graphic>();
			if( background != null )
				background.raycastTarget = true;

			SimpleInputDragListener eventReceiver = gameObject.AddComponent<SimpleInputDragListener>();
			eventReceiver.Listener = this;
		}

		void OnEnable()
		{
			if( xInput != null )
				xInput.StartTracking();

			if( yInput != null )
				yInput.StartTracking();
		}

		void OnDisable()
		{
			if( xInput != null )
				xInput.StopTracking();

			if( yInput != null )
				yInput.StopTracking();
		}

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

			if( xInput != null )
				xInput.value = 0f;

			if( yInput != null )
				yInput.value = 0f;
		}

		private void CalculateInput( PointerEventData eventData )
		{
			Vector2 pointerPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( rectTransform, eventData.position, background.canvas.worldCamera, out pointerPos );

			if( pointerPos.sqrMagnitude <= 400f )
				return;

			float angle = Vector2.Angle( pointerPos, Vector2.right );
			if( pointerPos.y < 0f )
				angle = 360f - angle;

			if( angle >= 25f && angle <= 155f )
				m_value.y = 1f;
			else if( angle >= 205f && angle <= 335f )
				m_value.y = -1f;
			else
				m_value.y = 0f;

			if( angle <= 65f || angle >= 295f )
				m_value.x = 1f;
			else if( angle >= 115f && angle <= 245f )
				m_value.x = -1f;
			else
				m_value.x = 0f;

			if( xInput != null )
				xInput.value = m_value.x;

			if( yInput != null )
				yInput.value = m_value.y;
		}
	}
}