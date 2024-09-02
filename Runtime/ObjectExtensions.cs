#if UNITY_5_3_OR_NEWER
using System;
using System.Text;
using UnityEngine;
using static CrazyPanda.UnityCore.Utils.ExtensionsUtils;
using Object = UnityEngine.Object;

namespace CrazyPanda.UnityCore.Utils
{
	public static class ObjectExtensions
	{
        /// <summary>
        /// Returns full scene hierarchy path to unity object
        /// </summary>
        /// <param name="unityObject"></param>
        /// <returns></returns>
        public static string NameInScene( this Object unityObject )
        {
            return unityObject.UnityNameInternal( includeTypeName: false );
        }
        
		/// <summary>
		/// Если type является монобехом или геймобжектом 
		/// вернет полный путь до геймобжекта на котором висит объект 
		/// и сам тип объекта
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string UnityName( this object obj )
        {
            return obj.UnityNameInternal( includeTypeName: true );
        }
        
        /// <summary>
		/// Если type является монобехом или геймобжектом 
		/// вернет полный путь до геймобжекта на котором висит объект 
		/// и сам тип объекта
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static void UnityName( this object obj, StringBuilder stringBuilder)
        {
            UnityNameInternal( obj, stringBuilder, includeTypeName: true );
        }

        private static string UnityNameInternal( this object obj, bool includeTypeName )
        {
            _stringBuilder.Clear();
            obj.UnityNameInternal( _stringBuilder, includeTypeName );
            var result = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return result;
        }
        
        private static void UnityNameInternal( this object obj, StringBuilder stringBuilder, bool includeTypeName)
        {
            if( obj == null )
                return;
            
            var objType = obj.GetType();

            switch( obj )
            {
                case GameObject gameObject:
                    AppendHiearchyPath( objType, gameObject, stringBuilder, includeTypeName );
                    return;
                case Component component:
                    AppendHiearchyPath( objType, component.gameObject, stringBuilder, includeTypeName );
                    return;
                default:
                    objType.CSharpName( stringBuilder );
                    return;
            }
        }
        
        private static void AppendHiearchyPath( Type objType, GameObject gameObject, StringBuilder stringBuilder, bool includeTypeName)
        {
            gameObject.HierarchyPath( stringBuilder );

            if( includeTypeName )
            {
                stringBuilder.Append( ' ' ).Append( '(' );
                objType.CSharpName( stringBuilder );
                stringBuilder.Append( ')' );
            }
        }

		public static T Clamp< T >( this T value, T min, T max ) where T : IComparable< T >
		{
			var result = value;
			if( value.CompareTo( max ) > 0 )
			{
				return max;
			}
			if( value.CompareTo( min ) < 0 )
			{
				return min;
			}
			return result;
		}
	}
}
#endif
