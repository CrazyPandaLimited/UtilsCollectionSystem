using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	[ AttributeUsage( AttributeTargets.Field | AttributeTargets.Property ) ]
	public class RequiredFieldAttribute : Attribute
	{
	}
}
