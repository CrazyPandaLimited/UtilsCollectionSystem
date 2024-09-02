using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	///Makes the string field show as text field with specified height
	[ AttributeUsage( AttributeTargets.Field ) ]
	public class TextAreaFieldAttribute : Attribute
	{
		public float height;

		public TextAreaFieldAttribute( float height )
		{
			this.height = height;
		}
	}
}
