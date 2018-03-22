using UnityEditor;

namespace SimpleInputNamespace
{
	[CustomPropertyDrawer( typeof( SimpleInput.ButtonInput ) )]
	public class ButtonInputDrawer : BaseInputDrawer
	{
		public override string ValueToString( SerializedProperty valueProperty )
		{
			return valueProperty.boolValue.ToString();
		}
	}
}