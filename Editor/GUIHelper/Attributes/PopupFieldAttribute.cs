using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	///Use on top of any type of field to restict values to the provided ones through a popup by either providing a params array for Valuetypes,
	///or a static property of a class in the form of "MyClass.MyProperty"
	[ AttributeUsage( AttributeTargets.Field ) ]
	public class PopupFieldAttribute : Attribute
	{
		public string staticPath;
		public object[ ] values;

		public PopupFieldAttribute( params object[ ] values )
		{
			this.values = values;
		}

		public PopupFieldAttribute( string staticPath )
		{
			this.staticPath = staticPath;
		}
	}
}
