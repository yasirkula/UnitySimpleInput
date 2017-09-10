using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleInput : MonoBehaviour
{
	#region Helper Classes
	private class Axis
	{
		public readonly string name;

		public float value = 0f;
		public float valueRaw = 0f;

		public readonly List<AxisInput> inputs;

		public Axis( string axis )
		{
			name = axis;
			inputs = new List<AxisInput>();
		}
	}

	private class Button
	{
		public enum State { None, Pressed, Held, Released };

		public readonly string button;

		public State state = State.None;

		public readonly List<ButtonInput> inputs;

		public Button( string button )
		{
			this.button = button;
			inputs = new List<ButtonInput>();
		}
	}

	private class MouseButton
	{
		public enum State { None, Pressed, Held, Released };

		public readonly int button;

		public State state = State.None;

		public readonly List<MouseButtonInput> inputs;

		public MouseButton( int button )
		{
			this.button = button;
			inputs = new List<MouseButtonInput>();
		}
	}

	private class Key
	{
		public enum State { None, Pressed, Held, Released };

		public readonly KeyCode key;

		public State state = State.None;

		public readonly List<KeyInput> inputs;

		public Key( KeyCode key )
		{
			this.key = key;
			inputs = new List<KeyInput>();
		}
	}

	public class AxisInput
	{
		public readonly string axis;
		public float value = 0f;

		public AxisInput( string axis )
		{
			if( axis == null || axis.Length == 0 )
				throw new System.ArgumentException();

			this.axis = axis;
		}

		public void StartTracking() { RegisterAxis( this ); }
		public void StopTracking() { UnregisterAxis( this ); }
	}

	public class ButtonInput
	{
		public readonly string button;
		public bool isDown = false;

		public ButtonInput( string button )
		{
			if( button == null || button.Length == 0 )
				throw new System.ArgumentException();

			this.button = button;
		}

		public void StartTracking() { RegisterButton( button, this ); }
		public void StopTracking() { UnregisterButton( button, this ); }
	}

	public class MouseButtonInput
	{
		public readonly int button;
		public bool isDown = false;

		public MouseButtonInput( int button )
		{
			this.button = button;
		}

		public void StartTracking() { RegisterMouseButton( button, this ); }
		public void StopTracking() { UnregisterMouseButton( button, this ); }
	}

	public class KeyInput
	{
		public readonly KeyCode key;
		public bool isDown = false;

		public KeyInput( KeyCode key )
		{
			this.key = key;
		}

		public void StartTracking() { RegisterKey( key, this ); }
		public void StopTracking() { UnregisterKey( key, this ); }
	}
	#endregion

	private const float AXIS_LERP_MODIFIER = 20f;

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

		for( int i = 0; i < trackedTemporaryButtons.Count; i++ )
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
			return buttonInput.state == Button.State.Pressed;

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
			return buttonInput.state == Button.State.Held || buttonInput.state == Button.State.Pressed;

		TrackButton( button );
		return false;
	}

	public static bool GetButtonUp( string button )
	{
		Button buttonInput;
		if( buttons.TryGetValue( button, out buttonInput ) )
			return buttonInput.state == Button.State.Released;

		TrackButton( button );
		return false;
	}

	public static bool GetMouseButtonDown( int button )
	{
		MouseButton mouseButtonInput;
		if( mouseButtons.TryGetValue( button, out mouseButtonInput ) )
			return mouseButtonInput.state == MouseButton.State.Pressed;

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
			return mouseButtonInput.state == MouseButton.State.Held || mouseButtonInput.state == MouseButton.State.Pressed;

		TrackMouseButton( button );
		return false;
	}

	public static bool GetMouseButtonUp( int button )
	{
		MouseButton mouseButtonInput;
		if( mouseButtons.TryGetValue( button, out mouseButtonInput ) )
			return mouseButtonInput.state == MouseButton.State.Released;

		TrackMouseButton( button );
		return false;
	}

	public static bool GetKeyDown( KeyCode key )
	{
		if( Input.GetKeyDown( key ) )
			return true;

		Key keyInput;
		if( keys.TryGetValue( key, out keyInput ) )
			return keyInput.state == Key.State.Pressed;

		return false;
	}

	public static bool GetKey( KeyCode key )
	{
		if( Input.GetKey( key ) )
			return true;

		Key keyInput;
		if( keys.TryGetValue( key, out keyInput ) )
			return keyInput.state == Key.State.Held || keyInput.state == Key.State.Pressed;

		return false;
	}

	public static bool GetKeyUp( KeyCode key )
	{
		if( Input.GetKeyUp( key ) )
			return true;

		Key keyInput;
		if( keys.TryGetValue( key, out keyInput ) )
			return keyInput.state == Key.State.Released;

		return false;
	}

	private static void RegisterAxis( AxisInput input )
	{
		Axis axisObj;
		if( !axes.TryGetValue( input.axis, out axisObj ) )
		{
			axisObj = new Axis( input.axis );

			axes[input.axis] = axisObj;
			axesList.Add( axisObj );

			// Track corresponding Unity input as well, if exists
			TrackAxis( input.axis, true );
		}

		axisObj.inputs.Add( input );
	}

	private static void UnregisterAxis( AxisInput input )
	{
		Axis axisObj;
		if( axes.TryGetValue( input.axis, out axisObj ) )
		{
			if( axisObj.inputs.Remove( input ) && axisObj.inputs.Count == 0 )
			{
				axes.Remove( input.axis );
				axesList.Remove( axisObj );
			}
		}
	}

	private static void RegisterButton( string button, ButtonInput input )
	{
		Button buttonObj;
		if( !buttons.TryGetValue( button, out buttonObj ) )
		{
			buttonObj = new Button( button );

			buttons[button] = buttonObj;
			buttonsList.Add( buttonObj );

			// Track corresponding Unity input as well, if exists
			TrackButton( input.button, true );
		}

		buttonObj.inputs.Add( input );
	}

	private static void UnregisterButton( string button, ButtonInput input )
	{
		Button buttonObj;
		if( buttons.TryGetValue( button, out buttonObj ) )
		{
			if( buttonObj.inputs.Remove( input ) && buttonObj.inputs.Count == 0 )
			{
				buttons.Remove( button );
				buttonsList.Remove( buttonObj );
			}
		}
	}

	private static void RegisterMouseButton( int button, MouseButtonInput input )
	{
		MouseButton mouseButtonObj;
		if( !mouseButtons.TryGetValue( button, out mouseButtonObj ) )
		{
			mouseButtonObj = new MouseButton( button );

			mouseButtons[button] = mouseButtonObj;
			mouseButtonsList.Add( mouseButtonObj );

			// Track corresponding Unity input as well, if exists
			TrackMouseButton( input.button, true );
		}

		mouseButtonObj.inputs.Add( input );
	}

	private static void UnregisterMouseButton( int button, MouseButtonInput input )
	{
		MouseButton mouseButtonObj;
		if( mouseButtons.TryGetValue( button, out mouseButtonObj ) )
		{
			if( mouseButtonObj.inputs.Remove( input ) && mouseButtonObj.inputs.Count == 0 )
			{
				mouseButtons.Remove( button );
				mouseButtonsList.Remove( mouseButtonObj );
			}
		}
	}

	private static void RegisterKey( KeyCode key, KeyInput input )
	{
		Key keyObj;
		if( !keys.TryGetValue( key, out keyObj ) )
		{
			keyObj = new Key( key );

			keys[key] = keyObj;
			keysList.Add( keyObj );
		}

		keyObj.inputs.Add( input );
	}

	private static void UnregisterKey( KeyCode key, KeyInput input )
	{
		Key keyObj;
		if( keys.TryGetValue( key, out keyObj ) )
		{
			if( keyObj.inputs.Remove( input ) && keyObj.inputs.Count == 0 )
			{
				keys.Remove( key );
				keysList.Remove( keyObj );
			}
		}
	}

	private static void TrackAxis( string axis, bool trackUnityAxisOnly = false )
	{
		try
		{
			AxisInput unityAxis = new AxisInput( axis ) { value = Input.GetAxisRaw( axis ) };
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
			ButtonInput unityButton = new ButtonInput( button ) { isDown = Input.GetButton( button ) };
			trackedUnityButtons.Add( unityButton );
			unityButton.StartTracking();
		}
		catch
		{
			if( !trackUnityButtonOnly )
			{
				ButtonInput temporaryButton = new ButtonInput( button ) { isDown = false };
				trackedTemporaryButtons.Add( temporaryButton );
				temporaryButton.StartTracking();
			}
		}
	}

	private static void TrackMouseButton( int button, bool trackUnityMouseButtonOnly = false )
	{
		try
		{
			MouseButtonInput unityMouseButton = new MouseButtonInput( button ) { isDown = Input.GetMouseButton( button ) };
			trackedUnityMouseButtons.Add( unityMouseButton );
			unityMouseButton.StartTracking();
		}
		catch
		{
			if( !trackUnityMouseButtonOnly )
			{
				MouseButtonInput temporaryMouseButton = new MouseButtonInput( button ) { isDown = false };
				trackedTemporaryMouseButtons.Add( temporaryMouseButton );
				temporaryMouseButton.StartTracking();
			}
		}
	}

	void Update()
	{
		if( OnUpdate != null )
			OnUpdate();

		for( int i = 0; i < trackedUnityAxes.Count; i++ )
			trackedUnityAxes[i].value = Input.GetAxisRaw( trackedUnityAxes[i].axis );

		for( int i = 0; i < trackedUnityButtons.Count; i++ )
			trackedUnityButtons[i].isDown = Input.GetButton( trackedUnityButtons[i].button );

		for( int i = 0; i < trackedUnityMouseButtons.Count; i++ )
			trackedUnityMouseButtons[i].isDown = Input.GetMouseButton( trackedUnityMouseButtons[i].button );

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

			axis.value = Mathf.Lerp( axis.value, axis.valueRaw, AXIS_LERP_MODIFIER * Time.deltaTime );

			if( axis.valueRaw == 0f && axis.value != 0f )
			{
				if( Mathf.Abs( axis.value ) < 0.025f )
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
				if( input.isDown )
				{
					isDown = true;
					break;
				}
			}

			if( isDown )
			{
				if( button.state == Button.State.None || button.state == Button.State.Released )
					button.state = Button.State.Pressed;
				else
					button.state = Button.State.Held;
			}
			else
			{
				if( button.state == Button.State.Pressed || button.state == Button.State.Held )
					button.state = Button.State.Released;
				else
					button.state = Button.State.None;
			}
		}

		for( int i = 0; i < mouseButtonsList.Count; i++ )
		{
			MouseButton mouseButton = mouseButtonsList[i];

			bool isDown = false;
			for( int j = mouseButton.inputs.Count - 1; j >= 0; j-- )
			{
				MouseButtonInput input = mouseButton.inputs[j];
				if( input.isDown )
				{
					isDown = true;
					break;
				}
			}

			if( isDown )
			{
				if( mouseButton.state == MouseButton.State.None || mouseButton.state == MouseButton.State.Released )
					mouseButton.state = MouseButton.State.Pressed;
				else
					mouseButton.state = MouseButton.State.Held;
			}
			else
			{
				if( mouseButton.state == MouseButton.State.Pressed || mouseButton.state == MouseButton.State.Held )
					mouseButton.state = MouseButton.State.Released;
				else
					mouseButton.state = MouseButton.State.None;
			}
		}

		for( int i = 0; i < keysList.Count; i++ )
		{
			Key key = keysList[i];

			bool isDown = false;
			for( int j = key.inputs.Count - 1; j >= 0; j-- )
			{
				KeyInput input = key.inputs[j];
				if( input.isDown )
				{
					isDown = true;
					break;
				}
			}

			if( isDown )
			{
				if( key.state == Key.State.None || key.state == Key.State.Released )
					key.state = Key.State.Pressed;
				else
					key.state = Key.State.Held;
			}
			else
			{
				if( key.state == Key.State.Pressed || key.state == Key.State.Held )
					key.state = Key.State.Released;
				else
					key.state = Key.State.None;
			}
		}
	}
}