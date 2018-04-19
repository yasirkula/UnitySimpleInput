using UnityEngine;

namespace SimpleInputNamespace
{
	public class KeyInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode realKey;

		public SimpleInput.KeyInput key = new SimpleInput.KeyInput();

		private void OnEnable()
		{
			key.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			key.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		private void OnUpdate()
		{
			key.value = Input.GetKey( realKey );
		}
	}
}