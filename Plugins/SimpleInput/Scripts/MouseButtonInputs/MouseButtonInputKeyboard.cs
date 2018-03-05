using UnityEngine;

namespace SimpleInputNamespace
{
	public class MouseButtonInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		public SimpleInput.MouseButtonInput mouseButton = new SimpleInput.MouseButtonInput();
		
		void OnEnable()
		{
			mouseButton.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			mouseButton.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			mouseButton.value = Input.GetKey( key );
		}
	}
}