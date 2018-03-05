using UnityEngine;

namespace SimpleInputNamespace
{
	public class AxisInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		[SerializeField]
		private float value;
		
		public SimpleInput.AxisInput axis = new SimpleInput.AxisInput();

		void OnEnable()
		{
			axis.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			axis.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			axis.value = Input.GetKey( key ) ? value : 0f;
		}
	}
}