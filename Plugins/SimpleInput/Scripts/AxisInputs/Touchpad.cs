using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
	public class Touchpad : MonoBehaviour
	{
		public enum MouseButton { Left = 0, Right = 1, Middle = 2 }
		
		public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput( "Mouse X" );
		public SimpleInput.AxisInput yAxis = new SimpleInput.AxisInput( "Mouse Y" );

		private RectTransform rectTransform;

		public float sensitivity = 1f;

		[Tooltip( "Should touchpad allow touch inputs on touchscreens, or mouse input only" )]
		public bool allowTouchInput = true;

		[Tooltip( "Valid mouse buttons that can register input through this touchpad" )]
		public MouseButton[] allowedMouseButtons;

		[Tooltip( "Should a touch on a UI element be considered valid" )]
		public bool ignoreUIElements = false;

		private float resolutionMultiplier;
		private int fingerId = -1;

		private Vector2 prevMouseInputPos;
		private bool trackMouseInput = false;

		private Vector2 m_value = Vector2.zero;
		public Vector2 Value { get { return m_value; } }

		void Awake()
		{
			rectTransform = transform as RectTransform;
			resolutionMultiplier = 100f / ( Screen.width + Screen.height );

			Graphic graphic = GetComponent<Graphic>();
			if( graphic != null )
				graphic.raycastTarget = false;
		}

		void OnEnable()
		{
			xAxis.StartTracking();
			yAxis.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			xAxis.StopTracking();
			yAxis.StopTracking();

			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			EventSystem eventSystem = EventSystem.current;

			m_value.Set( 0f, 0f );

			if( allowTouchInput && Input.touchCount > 0 )
			{
				for( int i = 0; i < Input.touchCount; i++ )
				{
					Touch touch = Input.GetTouch( i );
					if( fingerId == -1 )
					{
						if( touch.phase == TouchPhase.Began &&
							( rectTransform == null || RectTransformUtility.RectangleContainsScreenPoint( rectTransform, touch.position ) ) &&
							( ignoreUIElements || eventSystem == null || !eventSystem.IsPointerOverGameObject( touch.fingerId ) ) )
						{
							fingerId = touch.fingerId;
							break;
						}
					}
					else if( touch.fingerId == fingerId )
					{
						m_value = touch.deltaPosition * resolutionMultiplier * sensitivity;

						if( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled )
							fingerId = -1;

						break;
					}
				}
			}
			else
			{
				if( GetMouseButtonDown() )
				{
					Vector2 mousePos = Input.mousePosition;

					if( ( rectTransform == null || RectTransformUtility.RectangleContainsScreenPoint( rectTransform, mousePos ) ) &&
						( ignoreUIElements || eventSystem == null || !eventSystem.IsPointerOverGameObject() ) )
					{
						trackMouseInput = true;
						prevMouseInputPos = Input.mousePosition;
					}
					else
						trackMouseInput = false;
				}
				else if( trackMouseInput && GetMouseButton() )
				{
					Vector2 mousePos = Input.mousePosition;
					m_value = ( mousePos - prevMouseInputPos ) * resolutionMultiplier * sensitivity;
					prevMouseInputPos = mousePos;
				}
			}

			xAxis.value = m_value.x;
			yAxis.value = m_value.y;
		}

		private bool GetMouseButtonDown()
		{
			for( int i = 0; i < allowedMouseButtons.Length; i++ )
			{
				if( Input.GetMouseButtonDown( (int) allowedMouseButtons[i] ) )
					return true;
			}

			return false;
		}

		private bool GetMouseButton()
		{
			for( int i = 0; i < allowedMouseButtons.Length; i++ )
			{
				if( Input.GetMouseButtonDown( (int) allowedMouseButtons[i] ) )
					return true;
			}

			return false;
		}
	}
}