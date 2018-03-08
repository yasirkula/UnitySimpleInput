using UnityEngine;

namespace SimpleInputNamespace
{
	public class AxisInputMouse : MonoBehaviour
	{
		public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput();
		public SimpleInput.AxisInput yAxis = new SimpleInput.AxisInput();

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL || UNITY_FACEBOOK || UNITY_WSA || UNITY_WSA_10_0
		void OnEnable()
		{
			xAxis.StartTracking();
			yAxis.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			xAxis.StopTracking();
			yAxis.StopTracking();

			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			xAxis.value = Input.GetAxisRaw( "Mouse X" );
			yAxis.value = Input.GetAxisRaw( "Mouse Y" );
		}
#endif
	}
}