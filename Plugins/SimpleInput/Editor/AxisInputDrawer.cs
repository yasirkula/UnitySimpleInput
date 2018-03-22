using UnityEditor;

namespace SimpleInputNamespace
{
	[CustomPropertyDrawer( typeof( SimpleInput.AxisInput ) )]
	public class AxisInputDrawer : BaseInputDrawer
	{
		public override string ValueToString( SerializedProperty valueProperty )
		{
			return valueProperty.floatValue.ToString();
		}
	}
}