using UnityEngine;

namespace SimpleInputNamespace
{
	public class AxisInputMouse : MonoBehaviour
	{
		public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput();
		public SimpleInput.AxisInput yAxis = new SimpleInput.AxisInput();

//#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL || UNITY_FACEBOOK || UNITY_WSA || UNITY_WSA_10_0
		private void OnEnable()
		{
			xAxis.StartTracking();
			yAxis.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		private void OnDisable()
		{
			xAxis.StopTracking();
			yAxis.StopTracking();

			SimpleInput.OnUpdate -= OnUpdate;
		}

		private void OnUpdate()
		{
			if( Input.touchCount == 0 )
			{
				xAxis.value = Input.GetAxisRaw( "Mouse X" );
				yAxis.value = Input.GetAxisRaw( "Mouse Y" );
			}
			else
			{
				xAxis.value = 0f;
				yAxis.value = 0f;
			}
		}
//#endif
	}
}