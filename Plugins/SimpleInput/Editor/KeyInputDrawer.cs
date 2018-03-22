using UnityEditor;

namespace SimpleInputNamespace
{
	[CustomPropertyDrawer( typeof( SimpleInput.KeyInput ) )]
	public class KeyInputDrawer : BaseInputDrawer
	{
		public override string ValueToString( SerializedProperty valueProperty )
		{
			return valueProperty.boolValue.ToString();
		}
	}
}