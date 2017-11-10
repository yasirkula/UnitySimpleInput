using UnityEngine;

namespace SimpleInputNamespace
{
	public class ButtonInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private string button;

		[SerializeField]
		private KeyCode key;

		private SimpleInput.ButtonInput input = null;

		void Awake()
		{
			if( button != null && button.Length > 0 )
				input = new SimpleInput.ButtonInput( button );
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
			if( input != null )
				input.isDown = Input.GetKey( key );
		}
	}
}