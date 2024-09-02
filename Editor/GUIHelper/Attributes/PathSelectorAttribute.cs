using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	[ AttributeUsage( AttributeTargets.Field | AttributeTargets.Property ) ]
	public class PathSelectorAttribute : Attribute
	{
		public bool AbsolutePath { get; set; }

		public PathSelectorAttribute( bool absolutePath )
		{
			AbsolutePath = absolutePath;
		}
	}
}
