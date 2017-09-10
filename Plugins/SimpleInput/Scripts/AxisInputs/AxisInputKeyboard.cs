using UnityEngine;

namespace SimpleInputNamespace
{
	public class AxisInputKeyboard : MonoBehaviour
	{
		[SerializeField]
		private string axis;

		[SerializeField]
		private KeyCode key;

		[SerializeField]
		private float value;

		private SimpleInput.AxisInput input = null;

		void Awake()
		{
			if( axis != null && axis.Length > 0 )
				input = new SimpleInput.AxisInput( axis );
		}

		void OnEnable()
		{
			if( input != null )
				input.StartTracking();

			SimpleInput.OnUpdate += OnUpdate;
		}

		void OnDisable()
		{
			if( input != null )
				input.StopTracking();

			SimpleInput.OnUpdate -= OnUpdate;
		}

		void OnUpdate()
		{
			if( Input.GetKey( key ) )
				input.value = value;
			else
				input.value = 0f;
		}
	}
}