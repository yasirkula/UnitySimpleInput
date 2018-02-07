using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class Joystick : MonoBehaviour, ISimpleInputDraggable
	{
		public enum MovementAxes { XandY, X, Y };

		[SerializeField]
		private string xAxis = "Horizontal";
		[SerializeField]
		private string yAxis = "Vertical";

		private SimpleInput.AxisInput xInput = null;
		private SimpleInput.AxisInput yInput = null;

		private RectTransform joystickTR;
		private Image background;

		private RectTransform thumbTR;
		[SerializeField]
		private Image thumb;

		public MovementAxes movementAxes = MovementAxes.XandY;

		[SerializeField]
		private float movementAreaRadius = 75f;

		[SerializeField]
		private bool isDynamicJoystick = false;

		public RectTransform dynamicJoystickMovementArea;

		private bool joystickHeld = false;
		private Vector2 pointerInitialPos;

		private float _1OverMovementAreaRadius;
		private float movementAreaRadiusSqr;

		private float opacity = 1f;

		private Vector2 m_value = Vector2.zero;
		public Vector2 Value { get { return m_value; } }

		void Awake()
		{
			if( xAxis != null && xAxis.Length > 0 )
				xInput = new SimpleInput.AxisInput( xAxis );

			if( yAxis != null && yAxis.Length > 0 )
				yInput = new SimpleInput.AxisInput( yAxis );

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

		void Start()
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

		void OnEnable()
		{
			if( xInput != null )
				xInput.StartTracking();

			if( yInput != null )
				yInput.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			if( xInput != null )
				xInput.StopTracking();

			if( yInput != null )
				yInput.StopTracking();

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
				RectTransformUtility.ScreenPointToLocalPointInRectangle( joystickTR, eventData.position, thumb.canvas.worldCamera, out pointerInitialPos );
		}

		public void OnDrag( PointerEventData eventData )
		{
			Vector2 pointerPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( joystickTR, eventData.position, thumb.canvas.worldCamera, out pointerPos );

			Vector2 direction = pointerPos - pointerInitialPos;
			if( movementAxes == MovementAxes.X )
				direction.y = 0f;
			else if( movementAxes == MovementAxes.Y )
				direction.x = 0f;

			if( direction.sqrMagnitude > movementAreaRadiusSqr )
				direction = direction.normalized * movementAreaRadius;

			m_value = direction * _1OverMovementAreaRadius;

			thumbTR.localPosition = direction;

			if( xInput != null )
				xInput.value = m_value.x;

			if( yInput != null )
				yInput.value = m_value.y;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			joystickHeld = false;
			thumbTR.localPosition = Vector3.zero;

			m_value = Vector2.zero;

			if( xInput != null )
				xInput.value = 0f;

			if( yInput != null )
				yInput.value = 0f;
		}

		void OnUpdate()
		{
			if( !isDynamicJoystick )
				return;

			if( joystickHeld )
				opacity = Mathf.Min( 1f, opacity + Time.deltaTime * 4f );
			else
				opacity = Mathf.Max( 0f, opacity - Time.deltaTime * 4f );

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