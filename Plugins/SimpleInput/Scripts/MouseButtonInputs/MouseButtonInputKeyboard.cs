using UnityEngine;

namespace SimpleInputNamespace
{
	public class MouseButtonInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private int button;

		[SerializeField]
		private KeyCode key;

		private SimpleInput.MouseButtonInput input = null;

		void Awake()
		{
			input = new SimpleInput.MouseButtonInput( button );
		}

		void OnEnable()
		{
			if( input != null )
				input.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			if( input != null )
				input.StopTracking();

			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			input.isDown = Input.GetKey( key );
		}
	}
}