using UnityEngine;

namespace SimpleInputNamespace
{
	public class AxisInputKeyboard : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private KeyCode key;
#pragma warning restore 0649

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