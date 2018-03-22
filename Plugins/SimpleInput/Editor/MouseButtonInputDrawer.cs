using UnityEditor;

namespace SimpleInputNamespace
{
	[CustomPropertyDrawer( typeof( SimpleInput.MouseButtonInput ) )]
	public class MouseButtonInputDrawer : BaseInputDrawer
	{
		public override string ValueToString( SerializedProperty valueProperty )
		{
			return valueProperty.boolValue.ToString();
		}
	}
}