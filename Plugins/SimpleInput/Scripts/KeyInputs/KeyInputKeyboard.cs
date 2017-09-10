using UnityEngine;

namespace SimpleInputNamespace
{
	public class KeyInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode simulatedKey;

		[SerializeField]
		private KeyCode key;

		private SimpleInput.KeyInput input = null;

		void Awake()
		{
			input = new SimpleInput.KeyInput( simulatedKey );
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