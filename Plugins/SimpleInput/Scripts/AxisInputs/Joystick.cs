using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class Joystick : MonoBehaviour, ISimpleInputDraggable
	{
		public enum MovementAxes { XandY, X, Y };

		public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput( "Horizontal" );
		public SimpleInput.AxisInput yAxis = new SimpleInput.AxisInput( "Vertical" );

		private RectTransform joystickTR;
		private Image background;

		[SerializeField]
		private Image thumb;
		private RectTransform thumbTR;

		public MovementAxes movementAxes = MovementAxes.XandY;
		public float valueMultiplier = 1f;

		[SerializeField]
		private float movementAreaRadius = 75f;

		[SerializeField]
		private bool isDynamicJoystick = false;

		[SerializeField]
		private RectTransform dynamicJoystickMovementArea;

		private bool joystickHeld = false;
		private Vector2 pointerInitialPos;

		private float _1OverMovementAreaRadius;
		private float movementAreaRadiusSqr;

		private float opacity = 1f;

		private Vector2 m_value = Vector2.zero;
		public Vector2 Value { get { return m_value; } }

		private void Awake()
		{
			joystickTR = (RectTransform) transform;
			thumbTR = thumb.rectTransform;

			Image bgImage = GetComponent<Image>();
			if( bgImage != null )
			{
				background = bgImage;
				background.raycastTarget = false;
			}

			if( isDynamicJoystick )
			{
				opacity = 0f;
				thumb.raycastTarget = false;

				OnUpdate();
			}
			else
				thumb.raycastTarget = true;

			_1OverMovementAreaRadius = 1f / movementAreaRadius;
			movementAreaRadiusSqr = movementAreaRadius * movementAreaRadius;
		}

		private void Start()
		{
			SimpleInputDragListener eventReceiver;
			if( !isDynamicJoystick )
				eventReceiver = thumbTR.gameObject.AddComponent<SimpleInputDragListener>();
			else
			{
				if( dynamicJoystickMovementArea == null )
				{
					Transform canvasTransform = thumb.canvas.transform;
					dynamicJoystickMovementArea = new GameObject( "Dynamic Joystick Movement Area", typeof( RectTransform ), typeof( Image ) ).GetComponent<RectTransform>();

					dynamicJoystickMovementArea.SetParent( canvasTransform, false );
					dynamicJoystickMovementArea.SetAsFirstSibling();

					dynamicJoystickMovementArea.anchorMin = Vector2.zero;
					dynamicJoystickMovementArea.anchorMax = Vector2.one;
					dynamicJoystickMovementArea.sizeDelta = Vector2.zero;
					dynamicJoystickMovementArea.anchoredPosition = Vector2.zero;
				}

				Image dynamicJoystickMovementAreaRaycastTarget = dynamicJoystickMovementArea.GetComponent<Image>();
				if( dynamicJoystickMovementAreaRaycastTarget == null )
					dynamicJoystickMovementAreaRaycastTarget = dynamicJoystickMovementArea.gameObject.AddComponent<Image>();

				dynamicJoystickMovementAreaRaycastTarget.sprite = thumb.sprite;
				dynamicJoystickMovementAreaRaycastTarget.color = Color.clear;
				dynamicJoystickMovementAreaRaycastTarget.raycastTarget = true;

				eventReceiver = dynamicJoystickMovementArea.gameObject.AddComponent<SimpleInputDragListener>();
			}

			eventReceiver.Listener = this;
		}

		private void OnEnable()
		{
			xAxis.StartTracking();
			yAxis.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			xAxis.StopTracking();
			yAxis.StopTracking();

			SimpleInput.OnUpdate -= OnUpdate;
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			joystickHeld = true;

			if( isDynamicJoystick )
			{
				pointerInitialPos = Vector2.zero;
				joystickTR.position = eventData.position;
			}
			else
				RectTransformUtility.ScreenPointToLocalPointInRectangle( joystickTR, eventData.position, eventData.pressEventCamera, out pointerInitialPos );
		}

		public void OnDrag( PointerEventData eventData )
		{
			Vector2 pointerPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( joystickTR, eventData.position, eventData.pressEventCamera, out pointerPos );

			Vector2 direction = pointerPos - pointerInitialPos;
			if( movementAxes == MovementAxes.X )
				direction.y = 0f;
			else if( movementAxes == MovementAxes.Y )
				direction.x = 0f;

			if( direction.sqrMagnitude > movementAreaRadiusSqr )
				direction = direction.normalized * movementAreaRadius;

			m_value = direction * _1OverMovementAreaRadius * valueMultiplier;

			thumbTR.localPosition = direction;

			xAxis.value = m_value.x;
			yAxis.value = m_value.y;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			joystickHeld = false;
			thumbTR.localPosition = Vector3.zero;

			m_value = Vector2.zero;

			xAxis.value = 0f;
			yAxis.value = 0f;
		}

		private void OnUpdate()
		{
			if( !isDynamicJoystick )
				return;

			if( joystickHeld )
				opacity = Mathf.Min( 1f, opacity + Time.unscaledDeltaTime * 4f );
			else
				opacity = Mathf.Max( 0f, opacity - Time.unscaledDeltaTime * 4f );

			Color c = thumb.color;
			c.a = opacity;
			thumb.color = c;

			if( background != null )
			{
				c = background.color;
				c.a = opacity;
				background.color = c;
			}
		}
	}
}