using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	///Use for friendly names
	public class NameAttribute : Attribute
	{
		public string name;

		public NameAttribute( string name )
		{
			this.name = name;
		}
	}
}
