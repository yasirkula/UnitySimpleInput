using UnityEngine;

namespace SimpleInputNamespace
{
	public class KeyInputKeyboard : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private KeyCode realKey;
#pragma warning restore 0649

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