#if UNITY_EDITOR

using System;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	public interface IAttributeGuiHelper
	{
		Type AttributeTarget { get; }

		bool DrawGenericField( ContextGuiHelper context );
	}
}

#endif