using UnityEngine;

namespace SimpleInputNamespace
{
	public class KeyInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode realKey;

		public SimpleInput.KeyInput key = new SimpleInput.KeyInput();
		
		void OnEnable()
		{
			key.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			key.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			key.value = Input.GetKey( realKey );
		}
	}
}