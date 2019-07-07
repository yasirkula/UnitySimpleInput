using UnityEngine;

namespace SimpleInputNamespace
{
	public class MouseButtonInputKeyboard : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private KeyCode key;
#pragma warning restore 0649

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