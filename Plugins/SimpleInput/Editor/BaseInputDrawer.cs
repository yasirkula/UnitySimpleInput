using UnityEditor;
using UnityEngine;

namespace SimpleInputNamespace
{
	public abstract class BaseInputDrawer : PropertyDrawer
	{
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
		{
			EditorGUI.BeginProperty( position, label, property );

			Rect inputRect = new Rect( position.x, position.y, position.width - 90, position.height );
			Rect valueRect = new Rect( position.x + position.width - 85, position.y, 85, position.height );

			GUI.enabled = !Application.isPlaying;
			EditorGUI.PropertyField( inputRect, property.FindPropertyRelative( "m_key" ), label );
			GUI.enabled = true;

			EditorGUI.LabelField( valueRect, "Value: " + ValueToString( property.FindPropertyRelative( "value" ) ) );
			EditorGUI.EndProperty();
		}

		public abstract string ValueToString( SerializedProperty valueProperty );
	}
}