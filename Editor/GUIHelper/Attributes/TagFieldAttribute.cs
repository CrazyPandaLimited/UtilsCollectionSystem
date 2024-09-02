using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	///Makes the string field show as tagfield
	[ AttributeUsage( AttributeTargets.Field | AttributeTargets.Property ) ]
	public class TagFieldAttribute : Attribute
	{
	}
}
