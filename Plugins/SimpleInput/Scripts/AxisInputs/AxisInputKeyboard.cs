using UnityEngine;

namespace SimpleInputNamespace
{
	public class AxisInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		public SimpleInput.AxisInput axis = new SimpleInput.AxisInput();
		public float value = 1f;

		private void OnEnable()
		{
			axis.StartTracking();
			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			axis.StopTracking();
			SimpleInput.OnUpdate -= OnUpdate;
		}

		private void OnUpdate()
		{
			axis.value = Input.GetKey( key ) ? value : 0f;
		}
	}
}