SimpleInput available at: https://github.com/yasirkula/SimpleInput

= About SimpleInput =

SimpleInput is a replacement for Unity's standard Input system that allows you to use your own input providers 
for axes, buttons, mouse buttons and keys. In other words, it lets you simulate e.g. Input.GetAxis 
when a button is pressed or a joystick is dragged. It also supports using custom axes and buttons that
don't necessarily exist in Edit-Project Settings-Input.

To use the SimpleInput system, simply replace Input with SimpleInput in your scripts; i.e:

Input.GetAxis -> SimpleInput.GetAxis
Input.GetAxisRaw -> SimpleInput.GetAxisRaw
Input.GetButtonDown -> SimpleInput.GetButtonDown
Input.GetButton -> SimpleInput.GetButton
Input.GetButtonUp -> SimpleInput.GetButtonUp
Input.GetMouseButtonDown -> SimpleInput.GetMouseButtonDown
Input.GetMouseButton -> SimpleInput.GetMouseButton
Input.GetMouseButtonUp -> SimpleInput.GetMouseButtonUp
Input.GetKeyDown -> SimpleInput.GetKeyDown
Input.GetKey -> SimpleInput.GetKey
Input.GetKeyUp -> SimpleInput.GetKeyUp

Note that there is no replacement for Input.GetKey(string) function. You have to convert the string 
to the corresponding KeyCode to benefit from SimpleInput.GetKey(KeyCode) function.

Also note that SimpleInput works almost identically to standard Input system even if you don't use
any custom input providers. Only the lerping of Input.GetAxis might work slightly different.

= Built-in Input Components =

1. SimpleInput.GetAxis Inputs
- AxisInputKeyboard: provides input while specified key is held down
- AxisInputUI: provides input while attached UI Element (anything that extends UnityEngine.UI.Graphic) is held down
- Dpad: provides -1, 0 or 1 as input for x and y axes while the Dpad is held down; works similar to joystick Dpads
- Joystick: a standard on-screen joystick input. If "Is Dynamic Joystick" is selected, joystick only appears while 
a pointer touches the screen. "Dynamic Joystick Movement Area" specifies the zone that the dynamic joystick 
can appear in (leave blank to use the whole canvas)
- SteeringWheel: provides input while the wheel is rotated (by far, the most fun input method to play with =') )
- Touchpad: provides input while a pointer is dragged on a RectTransform (the RectTransform specifies the boundaries of the touchpad)

2. SimpleInput.GetButton Inputs
- ButtonInputKeyboard: provides input while specified key is held down
- ButtonInputUI: provides input while attached UI Element is held down

3. SimpleInput.GetMouseButton Inputs
- MouseButtonInputKeyboard: provides input while specified key is held down
- MouseButtonInputUI: provides input while attached UI Element is held down

4. SimpleInput.GetKey Inputs
- KeyInputKeyboard: provides input while specified key is held down
- KeyInputUI: provides input while attached UI Element is held down

To send an input while a mouse button is held down, you can use the <X>InputKeyboard component and set the key 
to the desired mouse button: KeyCode.Mouse0, KeyCode.Mouse1, etc.

The Prefabs folder contains some plug 'n' play prefabs that you can use by simply dragging them to your canvas. You can also 
customize them using the sprites provided in the Sprites folder (or using your own sprites, obviously).

= Writing Custom Input Providers =

Simply create a SimpleInput.AxisInput, SimpleInput.ButtonInput, SimpleInput.MouseButtonInput or SimpleInput.KeyInput object 
and call its StartTracking() function to start sending inputs to SimpleInput. Make sure to call the StopTracking() function 
before the object is destroyed or disabled.

If you need to update your input's value in Update function, you can register to SimpleInput.OnUpdate event instead of 
using Unity's Update function as SimpleInput.OnUpdate is called before the other Update functions (Script Execution Order).

See AxisInputKeyboard.cs or AxisInputUI.cs for simple example codes.