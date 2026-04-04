using UnityEngine;

namespace SimpleInputNamespace
{
	public class MouseButtonInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		public SimpleInput.MouseButtonInput mouseButton = new SimpleInput.MouseButtonInput();

		private void OnEnable()
		{
			mouseButton.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			mouseButton.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		private void OnUpdate()
		{
			mouseButton.value = Input.GetKey( key );
		}
	}
}