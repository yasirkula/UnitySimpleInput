using UnityEngine;

namespace SimpleInputNamespace
{
	public class KeyInputSwipeGesture : SwipeGestureBase<KeyCode, bool>
	{
		public SimpleInput.KeyInput key = new SimpleInput.KeyInput();

		protected override BaseInput<KeyCode, bool> Input { get { return key; } }
		protected override bool Value { get { return true; } }

		public override int Priority { get { return 1; } }
	}
}