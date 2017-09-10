using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInputNamespace
{
	public class Touchpad : MonoBehaviour
	{
		[SerializeField]
		private string xAxis = "Mouse X";
		[SerializeField]
		private string yAxis = "Mouse Y";

		private SimpleInput.AxisInput xInput = null;
		private SimpleInput.AxisInput yInput = null;

		private RectTransform rectTransform;

		public float sensitivity = 1f;

		[Tooltip( "Should touchpad allow mouse inputs as well, or only touch inputs" )]
		public bool allowMouseInput = false;
		
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
			if( xAxis != null && xAxis.Length > 0 )
				xInput = new SimpleInput.AxisInput( xAxis );

			if( yAxis != null && yAxis.Length > 0 )
				yInput = new SimpleInput.AxisInput( yAxis );

			rectTransform = transform as RectTransform;
			resolutionMultiplier = 100f / ( Screen.width + Screen.height );
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

		void OnUpdate()
		{
			EventSystem eventSystem = EventSystem.current;

			m_value.Set( 0f, 0f );

			if( allowMouseInput && Input.touchCount == 0 )
			{
				if( Input.GetMouseButtonDown( 0 ) )
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
				else if( trackMouseInput && Input.GetMouseButton( 0 ) )
				{
					Vector2 mousePos = Input.mousePosition;
					m_value = ( mousePos - prevMouseInputPos ) * resolutionMultiplier * sensitivity;
					prevMouseInputPos = mousePos;
				}
			}
			else
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

			xInput.value = m_value.x;
			yInput.value = m_value.y;
		}
	}
}