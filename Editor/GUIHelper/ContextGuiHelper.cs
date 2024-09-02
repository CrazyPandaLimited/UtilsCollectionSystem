#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	public class ContextGuiHelper
	{
		public GenericField DrawGenericField;
		public MemberInfo Member;

		public string Name;
		public Type ObjectType;
		public object Value;

		public delegate object GenericField( string name, object value, Type t, MemberInfo member = null, object instance = null, Type subType = null, bool addElem = true );

		public ContextGuiHelper( string name, object value, Type t, MemberInfo member, GenericField drawGenericField )
		{
			DrawGenericField = drawGenericField;
			Name = name;
			Value = value;
			ObjectType = t;
			Member = member;
		}

		public T GetAttribute< T >()
		{
			try
			{
				if( Member.GetCustomAttributes( typeof( T ), true ).FirstOrDefault() != null )
				{
					return ( T ) Member.GetCustomAttributes( typeof( T ), true ).FirstOrDefault();
				}
			}
			catch( Exception ex )
			{
				Debug.LogException( new ArgumentException( "Can't get attribute.", ex ) );
			}
			return default( T );
		}
	}
}

#endif