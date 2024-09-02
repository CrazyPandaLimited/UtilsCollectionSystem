using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	///When a class is associated with an icon
	[ AttributeUsage( AttributeTargets.Class ) ]
	public class IconAttribute : Attribute
	{
		public bool fixedColor;
		public string iconName;

		public IconAttribute( string iconName, bool fixedColor = false )
		{
			this.iconName = iconName;
			this.fixedColor = fixedColor;
		}
	}
}
