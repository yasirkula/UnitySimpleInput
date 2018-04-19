# Simple Input for Unity 3D
![screenshot](screenshot.png)

**Available on Asset Store:** https://www.assetstore.unity3d.com/en/#!/content/113033

**Forum Thread:** https://forum.unity.com/threads/simple-input-use-custom-input-providers-like-joysticks-ui-buttons-and-d-pads-open-source.520504/

**WebGL Demo:** http://yasirkula.net/SimpleInputDemo/

## ABOUT
SimpleInput is a replacement for Unity's standard **Input** system that allows you to use your own input providers for axes, buttons, mouse buttons and keys. In other words, it lets you simulate e.g. Input.GetAxis when a button is pressed or a joystick is dragged. It also supports using custom axes and buttons that don't necessarily exist in Edit-Project Settings-Input.

To use the SimpleInput system, simply replace Input with **SimpleInput** in your scripts; i.e:

- Input.GetAxis -> SimpleInput.GetAxis
- Input.GetAxisRaw -> SimpleInput.GetAxisRaw
- Input.GetButtonDown -> SimpleInput.GetButtonDown
- Input.GetButton -> SimpleInput.GetButton
- Input.GetButtonUp -> SimpleInput.GetButtonUp
- Input.GetMouseButtonDown -> SimpleInput.GetMouseButtonDown
- Input.GetMouseButton -> SimpleInput.GetMouseButton
- Input.GetMouseButtonUp -> SimpleInput.GetMouseButtonUp
- Input.GetKeyDown -> SimpleInput.GetKeyDown
- Input.GetKey -> SimpleInput.GetKey
- Input.GetKeyUp -> SimpleInput.GetKeyUp

Note that there is no replacement for *Input.GetKey(string)* function. You have to convert the string to the corresponding **KeyCode** to benefit from *SimpleInput.GetKey(KeyCode)* function.

SimpleInput works almost identically to standard Input system; only the lerping of Input.GetAxis might differ slightly.

## UPGRADING FROM PREVIOUS VERSIONS
Values of axes, buttons, mouse buttons and keys in SimpleInput components will be reset after the upgrade. So, you should write down these values somewhere before upgrading the plugin.

## BUILT-IN INPUT COMPONENTS

### SimpleInput.GetAxis Inputs
- **AxisInputKeyboard**: provides axis input while specified key is held down
- **AxisInputMouse**: redirects "Mouse X" and "Mouse Y" inputs to two other axes on standalone platforms. Normally, on mobile platforms, dragging your finger on touchscreen provides "Mouse X" and "Mouse Y" inputs. However, you may want to simulate these two axes only with certain input method(s) on mobile platforms, e.g. a joystick. In this case, use this component to redirect mouse input to some other custom axes (like "MouseNew X", "MouseNew Y") and use these axes with SimpleInput in your scripts. Other input method(s) e.g. joystick should also use these axes instead of "Mouse X" and "Mouse Y"
- **AxisInputUI**: provides axis input while attached UI Element (anything that extends *UnityEngine.UI.Graphic*) is held down
- **AxisInputPinchGesture**: provides axis input while pinch gesture is performed with two pointers on a RectTransform
- **AxisInputRotateGesture**: provides axis input while rotate gesture is performed with two pointers on a RectTransform
- **AxisInputSwipeGesture**: provides axis input while a pointer is swiped by a specified amount on a RectTransform
- **Dpad**: provides -1, 0 or 1 as axis input for x and y axes while the Dpad is held down; works similar to joystick Dpads
- **Joystick**: a standard on-screen joystick input. If *Is Dynamic Joystick* is selected, joystick only appears while a pointer touches the screen. *Dynamic Joystick Movement Area* specifies the zone that the dynamic joystick can appear in (leave blank to use the whole canvas)
- **SteeringWheel**: provides axis input while the wheel is rotated (by far, the most fun input method to play with =') )
- **Touchpad**: provides axis input while a pointer is dragged on a RectTransform

### SimpleInput.GetButton Inputs
- **ButtonInputKeyboard**: provides button input while specified key is held down
- **ButtonInputUI**: provides button input while attached UI Element is held down
- **ButtonInputSwipeGesture**: provides button input while a pointer is swiped by a specified amount on a RectTransform

### SimpleInput.GetMouseButton Inputs
- **MouseButtonInputKeyboard**: provides mouse button input while specified key is held down
- **MouseButtonInputUI**: provides mouse button input while attached UI Element is held down
- **MouseButtonInputSwipeGesture**: provides mouse button input while a pointer is swiped by a specified amount on a RectTransform

### SimpleInput.GetKey Inputs
- **KeyInputKeyboard**: provides key input while specified real key is held down
- **KeyInputUI**: provides key input while attached UI Element is held down
- **KeyInputSwipeGesture**: provides key input while a pointer is swiped by a specified amount on a RectTransform

To send an input while a mouse button is held down, you can use the XInputKeyboard component and set the key to the desired mouse button: *KeyCode.Mouse0*, *KeyCode.Mouse1*, etc.

**Prefabs** folder contains some plug 'n' play prefabs. Drag & drop them to your canvas and you are good to go! You can also customize them using the sprites provided in the **Sprites** folder (or using your own sprites, obviously).

## REBINDING INPUTS
It is possible to rebind the axes, buttons, mouse buttons and/or keys in your components during gameplay. For example, if you want to change the axes of your joystick from "Horizontal" and "Vertical" to "Horizontal2" and "Vertical2", use the following code:

```csharp
void ChangeBindingsOfJoystick( Joystick joystick )
{
	joystick.xAxis.Key = "Horizontal2";
	joystick.yAxis.Key = "Vertical2";
}
```

Rebinding inputs from the Inspector is currently not possible during gameplay.

## WRITING CUSTOM INPUT PROVIDERS
Simply create a **SimpleInput.AxisInput**, **SimpleInput.ButtonInput**, **SimpleInput.MouseButtonInput** or **SimpleInput.KeyInput** object and call its **StartTracking()** function to start sending inputs to SimpleInput. Make sure to call the **StopTracking()** function before the object is destroyed or disabled. To change the value of the input, change its **value** field.

If you need to update your input's value in *Update* function, you can register to **SimpleInput.OnUpdate** event instead of using Unity's *Update* function as SimpleInput.OnUpdate is called before other Update functions (*Script Execution Order*).

See *AxisInputKeyboard.cs* and *AxisInputUI.cs* for reference.
