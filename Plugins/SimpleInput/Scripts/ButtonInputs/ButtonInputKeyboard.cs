using UnityEngine;

namespace SimpleInputNamespace
{
	public class ButtonInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		public SimpleInput.ButtonInput button = new SimpleInput.ButtonInput();
		
		void OnEnable()
		{
			button.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			button.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			button.value = Input.GetKey( key );
		}
	}
}