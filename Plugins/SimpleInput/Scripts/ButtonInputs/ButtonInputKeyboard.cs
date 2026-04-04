using UnityEngine;

namespace SimpleInputNamespace
{
	public class ButtonInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		public SimpleInput.ButtonInput button = new SimpleInput.ButtonInput();

		private void OnEnable()
		{
			button.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			button.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		private void OnUpdate()
		{
			button.value = Input.GetKey( key );
		}
	}
}