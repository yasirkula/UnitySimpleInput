//#define GET_AXIS_USE_MOVE_TOWARDS

using SimpleInputNamespace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleInput : MonoBehaviour
{
	private enum InputState { None, Pressed, Held, Released };

	#region Helper Classes
	private class Axis
	{
		public readonly string name;
		public readonly List<AxisInput> inputs;

		public float value = 0f;
		public float valueRaw = 0f;

		public Axis( string axis )
		{
			name = axis;
			inputs = new List<AxisInput>();
		}
	}

	private class Button
	{
		public readonly string button;
		public readonly List<ButtonInput> inputs;

		public InputState state = InputState.None;

		public Button( string button )
		{
			this.button = button;
			inputs = new List<ButtonInput>();
		}
	}

	private class MouseButton
	{
		public readonly int button;
		public readonly List<MouseButtonInput> inputs;

		public InputState state = InputState.None;

		public MouseButton( int button )
		{
			this.button = button;
			inputs = new List<MouseButtonInput>();
		}
	}

	private class Key
	{
		public readonly KeyCode key;
		public readonly List<KeyInput> inputs;

		public InputState state = InputState.None;

		public Key( KeyCode key )
		{
			this.key = key;
			inputs = new List<KeyInput>();
		}
	}

	[System.Serializable]
	public class AxisInput : BaseInput<string, float>
	{
		public AxisInput() : base() { }
		public AxisInput( string key ) : base( key ) { }

		public override bool IsKeyValid() { return !string.IsNullOrEmpty( Key ); }

		protected override bool KeysEqual( string key1, string key2 ) { return key1 == key2; }
		protected override void RegisterInput() { RegisterAxis( this ); }
		protected override void UnregisterInput() { UnregisterAxis( this ); }
	}

	[System.Serializable]
	public class ButtonInput : BaseInput<string, bool>
	{
		public ButtonInput() : base() { }
		public ButtonInput( string key ) : base( key ) { }

		public override bool IsKeyValid() { return !string.IsNullOrEmpty( Key ); }

		protected override bool KeysEqual( string key1, string key2 ) { return key1 == key2; }
		protected override void RegisterInput() { RegisterButton( this ); }
		protected override void UnregisterInput() { UnregisterButton( this ); }
	}

	[System.Serializable]
	public class MouseButtonInput : BaseInput<int, bool>
	{
		public MouseButtonInput() : base() { }
		public MouseButtonInput( int key ) : base( key ) { }

		protected override bool KeysEqual( int key1, int key2 ) { return key1 == key2; }
		protected override void RegisterInput() { RegisterMouseButton( this ); }
		protected override void UnregisterInput() { UnregisterMouseButton( this ); }
	}

	[System.Serializable]
	public class KeyInput : BaseInput<KeyCode, bool>
	{
		public KeyInput() : base() { }
		public KeyInput( KeyCode key ) : base( key ) { }

		protected override bool KeysEqual( KeyCode key1, KeyCode key2 ) { return key1 == key2; }
		protected override void RegisterInput() { RegisterKey( this ); }
		protected override void UnregisterInput() { UnregisterKey( this ); }
	}
	#endregion

#if GET_AXIS_USE_MOVE_TOWARDS
	/// <summary>
	/// Speed to move SimpleInput.GetAxis' value towards SimpleInput.GetAxisRaw's value in units per second
	/// </summary>
	public static float GetAxisSensitivity = 3f;
#else
	/// <summary>
	/// Lerp modifier to move SimpleInput.GetAxis' value towards SimpleInput.GetAxisRaw's value
	/// </summary>
	public static float GetAxisSensitivity = 20f;
#endif
	/// <summary>
	/// When SimpleInput.GetAxisRaw's value is 0f and SimpleInput.GetAxis' value is within this range, SimpleInput.GetAxis' value will snap to 0f
	/// </summary>
	public static float GetAxisDeadZone = 0.025f;
	/// <summary>
	/// If set to true and the values of SimpleInput.GetAxis and SimpleInput.GetAxisRaw have different signs, SimpleInput.GetAxis will jump to 0f and continue from there
	/// </summary>
	public static bool GetAxisSnapValue = true;
	/// <summary>
	/// Sets whether or not Time.timeScale should affect SimpleInput.GetAxis' sensitivity
	/// </summary>
	public static bool GetAxisTimeScaleDependent = true;

	private static bool m_trackUnityInput = true;
	public static bool TrackUnityInput
	{
		get { return m_trackUnityInput; }
		set
		{
			if( m_trackUnityInput != value )
			{
				m_trackUnityInput = value;

				if( !m_trackUnityInput )
				{
					for( int i = 0; i < trackedUnityAxes.Count; i++ )
						trackedUnityAxes[i].ResetValue();

					for( int i = 0; i < trackedUnityButtons.Count; i++ )
						trackedUnityButtons[i].ResetValue();

					for( int i = 0; i < trackedUnityMouseButtons.Count; i++ )
						trackedUnityMouseButtons[i].ResetValue();
				}
			}
		}
	}

	// Singleton instance
	private static SimpleInput instance;

	private static Dictionary<string, Axis> axes = new Dictionary<string, Axis>();
	private static List<Axis> axesList = new List<Axis>();

	private static List<AxisInput> trackedUnityAxes = new List<AxisInput>();
	private static List<AxisInput> trackedTemporaryAxes = new List<AxisInput>();

	private static Dictionary<string, Button> buttons = new Dictionary<string, Button>();
	private static List<Button> buttonsList = new List<Button>();

	private static List<ButtonInput> trackedUnityButtons = new List<ButtonInput>();
	private static List<ButtonInput> trackedTemporaryButtons = new List<ButtonInput>();

	private static Dictionary<int, MouseButton> mouseButtons = new Dictionary<int, MouseButton>();
	private static List<MouseButton> mouseButtonsList = new List<MouseButton>();

	private static List<MouseButtonInput> trackedUnityMouseButtons = new List<MouseButtonInput>();
	private static List<MouseButtonInput> trackedTemporaryMouseButtons = new List<MouseButtonInput>();

	private static Dictionary<KeyCode, Key> keys = new Dictionary<KeyCode, Key>();
	private static List<Key> keysList = new List<Key>();

	public delegate void UpdateCallback();
	public static event UpdateCallback OnUpdate;

	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
	static void Init()
	{
		// Initialize singleton instance
		instance = new GameObject( "SimpleInput" ).AddComponent<SimpleInput>();
		DontDestroyOnLoad( instance.gameObject );
	}

#if UNITY_2019_3_OR_NEWER
	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )] // Configurable Enter Play Mode: https://docs.unity3d.com/Manual/DomainReloading.html
	private static void ResetStatics()
	{
		instance = null;
		OnUpdate = null;

		axes.Clear();
		axesList.Clear();
		trackedUnityAxes.Clear();
		trackedTemporaryAxes.Clear();
		buttons.Clear();
		buttonsList.Clear();
		trackedUnityButtons.Clear();
		trackedTemporaryButtons.Clear();
		mouseButtons.Clear();
		mouseButtonsList.Clear();
		trackedUnityMouseButtons.Clear();
		trackedTemporaryMouseButtons.Clear();
		keys.Clear();
		keysList.Clear();
	}
#endif

	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneChanged;
	}

	private void Start()
	{
		if( this != instance )
			Destroy( this );
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneChanged;
	}

	private static void OnSceneChanged( Scene scene, LoadSceneMode loadSceneMode )
	{
		for( int i = 0; i < trackedTemporaryAxes.Count; i++ )
			trackedTemporaryAxes[i].StopTracking();

		for( int i = 0; i < trackedTemporaryButtons.Count; i++ )
			trackedTemporaryButtons[i].StopTracking();

		for( int i = 0; i < trackedTemporaryMouseButtons.Count; i++ )
			trackedTemporaryMouseButtons[i].StopTracking();

		trackedTemporaryAxes.Clear();
		trackedTemporaryButtons.Clear();
		trackedTemporaryMouseButtons.Clear();
	}

	public static float GetAxis( string axis )
	{
		Axis axisInput;
		if( axes.TryGetValue( axis, out axisInput ) )
			return axisInput.value;

		// Try to increase the hit-rate of the above if statement
		// by tracking the corresponding Unity axis (if exists)
		// or using a dummy (temporary) AxisInput
		TrackAxis( axis );
		return 0f;
	}

	public static float GetAxisRaw( string axis )
	{
		Axis axisInput;
		if( axes.TryGetValue( axis, out axisInput ) )
			return axisInput.valueRaw;

		TrackAxis( axis );
		return 0f;
	}

	public static bool GetButtonDown( string button )
	{
		Button buttonInput;
		if( buttons.TryGetValue( button, out buttonInput ) )
			return buttonInput.state == InputState.Pressed;

		// Try to increase the hit-rate of the above if statement
		// by tracking the corresponding Unity button (if exists)
		// or using a dummy (temporary) ButtonInput
		TrackButton( button );
		return false;
	}

	public static bool GetButton( string button )
	{
		Button buttonInput;
		if( buttons.TryGetValue( button, out buttonInput ) )
			return buttonInput.state == InputState.Held || buttonInput.state == InputState.Pressed;

		TrackButton( button );
		return false;
	}

	public static bool GetButtonUp( string button )
	{
		Button buttonInput;
		if( buttons.TryGetValue( button, out buttonInput ) )
			return buttonInput.state == InputState.Released;

		TrackButton( button );
		return false;
	}

	public static bool GetMouseButtonDown( int button )
	{
		MouseButton mouseButtonInput;
		if( mouseButtons.TryGetValue( button, out mouseButtonInput ) )
			return mouseButtonInput.state == InputState.Pressed;

		// Try to increase the hit-rate of the above if statement
		// by tracking the corresponding Unity mouse button (if exists)
		// or using a dummy (temporary) MouseButtonInput
		TrackMouseButton( button );
		return false;
	}

	public static bool GetMouseButton( int button )
	{
		MouseButton mouseButtonInput;
		if( mouseButtons.TryGetValue( button, out mouseButtonInput ) )
			return mouseButtonInput.state == InputState.Held || mouseButtonInput.state == InputState.Pressed;

		TrackMouseButton( button );
		return false;
	}

	public static bool GetMouseButtonUp( int button )
	{
		MouseButton mouseButtonInput;
		if( mouseButtons.TryGetValue( button, out mouseButtonInput ) )
			return mouseButtonInput.state == InputState.Released;

		TrackMouseButton( button );
		return false;
	}

	public static bool GetKeyDown( KeyCode key )
	{
		if( TrackUnityInput && Input.GetKeyDown( key ) )
			return true;

		Key keyInput;
		if( keys.TryGetValue( key, out keyInput ) )
			return keyInput.state == InputState.Pressed;

		return false;
	}

	public static bool GetKey( KeyCode key )
	{
		if( TrackUnityInput && Input.GetKey( key ) )
			return true;

		Key keyInput;
		if( keys.TryGetValue( key, out keyInput ) )
			return keyInput.state == InputState.Held || keyInput.state == InputState.Pressed;

		return false;
	}

	public static bool GetKeyUp( KeyCode key )
	{
		if( TrackUnityInput && Input.GetKeyUp( key ) )
			return true;

		Key keyInput;
		if( keys.TryGetValue( key, out keyInput ) )
			return keyInput.state == InputState.Released;

		return false;
	}

	private static void RegisterAxis( AxisInput input )
	{
		Axis axisObj;
		if( !axes.TryGetValue( input.Key, out axisObj ) )
		{
			axisObj = new Axis( input.Key );

			axes[input.Key] = axisObj;
			axesList.Add( axisObj );

			// Track corresponding Unity input as well, if exists
			TrackAxis( input.Key, true );
		}

		axisObj.inputs.Add( input );
	}

	private static void UnregisterAxis( AxisInput input )
	{
		Axis axisObj;
		if( axes.TryGetValue( input.Key, out axisObj ) )
		{
			if( axisObj.inputs.Remove( input ) && axisObj.inputs.Count == 0 )
			{
				axes.Remove( input.Key );
				axesList.Remove( axisObj );
			}
		}
	}

	private static void RegisterButton( ButtonInput input )
	{
		Button buttonObj;
		if( !buttons.TryGetValue( input.Key, out buttonObj ) )
		{
			buttonObj = new Button( input.Key );

			buttons[input.Key] = buttonObj;
			buttonsList.Add( buttonObj );

			// Track corresponding Unity input as well, if exists
			TrackButton( input.Key, true );
		}

		buttonObj.inputs.Add( input );
	}

	private static void UnregisterButton( ButtonInput input )
	{
		Button buttonObj;
		if( buttons.TryGetValue( input.Key, out buttonObj ) )
		{
			if( buttonObj.inputs.Remove( input ) && buttonObj.inputs.Count == 0 )
			{
				buttons.Remove( input.Key );
				buttonsList.Remove( buttonObj );
			}
		}
	}

	private static void RegisterMouseButton( MouseButtonInput input )
	{
		MouseButton mouseButtonObj;
		if( !mouseButtons.TryGetValue( input.Key, out mouseButtonObj ) )
		{
			mouseButtonObj = new MouseButton( input.Key );

			mouseButtons[input.Key] = mouseButtonObj;
			mouseButtonsList.Add( mouseButtonObj );

			// Track corresponding Unity input as well, if exists
			TrackMouseButton( input.Key, true );
		}

		mouseButtonObj.inputs.Add( input );
	}

	private static void UnregisterMouseButton( MouseButtonInput input )
	{
		MouseButton mouseButtonObj;
		if( mouseButtons.TryGetValue( input.Key, out mouseButtonObj ) )
		{
			if( mouseButtonObj.inputs.Remove( input ) && mouseButtonObj.inputs.Count == 0 )
			{
				mouseButtons.Remove( input.Key );
				mouseButtonsList.Remove( mouseButtonObj );
			}
		}
	}

	private static void RegisterKey( KeyInput input )
	{
		Key keyObj;
		if( !keys.TryGetValue( input.Key, out keyObj ) )
		{
			keyObj = new Key( input.Key );

			keys[input.Key] = keyObj;
			keysList.Add( keyObj );
		}

		keyObj.inputs.Add( input );
	}

	private static void UnregisterKey( KeyInput input )
	{
		Key keyObj;
		if( keys.TryGetValue( input.Key, out keyObj ) )
		{
			if( keyObj.inputs.Remove( input ) && keyObj.inputs.Count == 0 )
			{
				keys.Remove( input.Key );
				keysList.Remove( keyObj );
			}
		}
	}

	private static void TrackAxis( string axis, bool trackUnityAxisOnly = false )
	{
		try
		{
			AxisInput unityAxis = new AxisInput( axis ) { value = Input.GetAxisRaw( axis ) };
			if( !m_trackUnityInput )
				unityAxis.ResetValue();

			trackedUnityAxes.Add( unityAxis );
			unityAxis.StartTracking();
		}
		catch
		{
			if( !trackUnityAxisOnly )
			{
				AxisInput temporaryAxis = new AxisInput( axis ) { value = 0f };
				trackedTemporaryAxes.Add( temporaryAxis );
				temporaryAxis.StartTracking();
			}
		}
	}

	private static void TrackButton( string button, bool trackUnityButtonOnly = false )
	{
		try
		{
			ButtonInput unityButton = new ButtonInput( button ) { value = Input.GetButton( button ) };
			if( !m_trackUnityInput )
				unityButton.ResetValue();

			trackedUnityButtons.Add( unityButton );
			unityButton.StartTracking();
		}
		catch
		{
			if( !trackUnityButtonOnly )
			{
				ButtonInput temporaryButton = new ButtonInput( button ) { value = false };
				trackedTemporaryButtons.Add( temporaryButton );
				temporaryButton.StartTracking();
			}
		}
	}

	private static void TrackMouseButton( int button, bool trackUnityMouseButtonOnly = false )
	{
		try
		{
			MouseButtonInput unityMouseButton = new MouseButtonInput( button ) { value = Input.GetMouseButton( button ) };
			if( !m_trackUnityInput )
				unityMouseButton.ResetValue();

			trackedUnityMouseButtons.Add( unityMouseButton );
			unityMouseButton.StartTracking();
		}
		catch
		{
			if( !trackUnityMouseButtonOnly )
			{
				MouseButtonInput temporaryMouseButton = new MouseButtonInput( button ) { value = false };
				trackedTemporaryMouseButtons.Add( temporaryMouseButton );
				temporaryMouseButton.StartTracking();
			}
		}
	}

	private void Update()
	{
		if( OnUpdate != null )
			OnUpdate();

		if( m_trackUnityInput )
		{
			for( int i = 0; i < trackedUnityAxes.Count; i++ )
				trackedUnityAxes[i].value = Input.GetAxisRaw( trackedUnityAxes[i].Key );

			for( int i = 0; i < trackedUnityButtons.Count; i++ )
				trackedUnityButtons[i].value = Input.GetButton( trackedUnityButtons[i].Key );

			for( int i = 0; i < trackedUnityMouseButtons.Count; i++ )
				trackedUnityMouseButtons[i].value = Input.GetMouseButton( trackedUnityMouseButtons[i].Key );
		}

		float sensitivity = GetAxisSensitivity * ( GetAxisTimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime );

		for( int i = 0; i < axesList.Count; i++ )
		{
			Axis axis = axesList[i];

			axis.valueRaw = 0f;
			for( int j = axis.inputs.Count - 1; j >= 0; j-- )
			{
				AxisInput input = axis.inputs[j];
				if( input.value != 0f )
				{
					axis.valueRaw = input.value;
					break;
				}
			}

#if GET_AXIS_USE_MOVE_TOWARDS
			if( axis.value >= 0 )
				axis.value = Mathf.MoveTowards( axis.valueRaw >= 0 || !GetAxisSnapValue ? axis.value : 0, axis.valueRaw, sensitivity );
			else
				axis.value = Mathf.MoveTowards( axis.valueRaw <= 0 || !GetAxisSnapValue ? axis.value : 0, axis.valueRaw, sensitivity );
#else
			if( axis.value >= 0 )
				axis.value = Mathf.Lerp( axis.valueRaw >= 0 || !GetAxisSnapValue ? axis.value : 0, axis.valueRaw, sensitivity );
			else
				axis.value = Mathf.Lerp( axis.valueRaw <= 0 || !GetAxisSnapValue ? axis.value : 0, axis.valueRaw, sensitivity );
#endif

			if( axis.valueRaw == 0f && axis.value != 0f )
			{
				if( Mathf.Abs( axis.value ) < GetAxisDeadZone )
					axis.value = 0f;
			}
		}

		for( int i = 0; i < buttonsList.Count; i++ )
		{
			Button button = buttonsList[i];

			bool isDown = false;
			for( int j = button.inputs.Count - 1; j >= 0; j-- )
			{
				ButtonInput input = button.inputs[j];
				if( input.value )
				{
					isDown = true;
					break;
				}
			}

			if( isDown )
			{
				if( button.state == InputState.None || button.state == InputState.Released )
					button.state = InputState.Pressed;
				else
					button.state = InputState.Held;
			}
			else
			{
				if( button.state == InputState.Pressed || button.state == InputState.Held )
					button.state = InputState.Released;
				else
					button.state = InputState.None;
			}
		}

		for( int i = 0; i < mouseButtonsList.Count; i++ )
		{
			MouseButton mouseButton = mouseButtonsList[i];

			bool isDown = false;
			for( int j = mouseButton.inputs.Count - 1; j >= 0; j-- )
			{
				MouseButtonInput input = mouseButton.inputs[j];
				if( input.value )
				{
					isDown = true;
					break;
				}
			}

			if( isDown )
			{
				if( mouseButton.state == InputState.None || mouseButton.state == InputState.Released )
					mouseButton.state = InputState.Pressed;
				else
					mouseButton.state = InputState.Held;
			}
			else
			{
				if( mouseButton.state == InputState.Pressed || mouseButton.state == InputState.Held )
					mouseButton.state = InputState.Released;
				else
					mouseButton.state = InputState.None;
			}
		}

		for( int i = 0; i < keysList.Count; i++ )
		{
			Key key = keysList[i];

			bool isDown = false;
			for( int j = key.inputs.Count - 1; j >= 0; j-- )
			{
				KeyInput input = key.inputs[j];
				if( input.value )
				{
					isDown = true;
					break;
				}
			}

			if( isDown )
			{
				if( key.state == InputState.None || key.state == InputState.Released )
					key.state = InputState.Pressed;
				else
					key.state = InputState.Held;
			}
			else
			{
				if( key.state == InputState.Pressed || key.state == InputState.Held )
					key.state = InputState.Released;
				else
					key.state = InputState.None;
			}
		}
	}
}