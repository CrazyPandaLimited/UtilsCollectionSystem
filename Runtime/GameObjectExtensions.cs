#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static CrazyPanda.UnityCore.Utils.ExtensionsUtils;

namespace CrazyPanda.UnityCore.Utils
{
	public static class GameObjectExtensions
    {
        private static readonly List< string > _cachedParents = new List< string >();

		/// <summary>
		/// Вернет путь в сцене. Например GameObject1/GameObject2/GameObject3
		/// </summary>
		/// <param name="go"></param>
		/// <returns></returns>
		public static string HierarchyPath( this GameObject go )
        {
            _stringBuilder.Clear();
            
            go.HierarchyPath( _stringBuilder );
            var result = _stringBuilder.ToString();

            _stringBuilder.Clear();
            return result;
        }

        /// <summary>
		/// Вернет путь в сцене. Например GameObject1/GameObject2/GameObject3
		/// </summary>
		/// <param name="go"></param>
		/// <returns></returns>
        public static void HierarchyPath( this GameObject go, StringBuilder stringBuilder )
        {
            for( var parent = go.transform.parent; parent != null; parent = parent.parent )
            {
                _cachedParents.Add( parent.name );
            }

            for( var i = _cachedParents.Count - 1; i >= 0; --i )
            {
                _stringBuilder.Append( _cachedParents[ i ] ).Append( '/' );
            }

            stringBuilder.Append( go.name );

            _cachedParents.Clear();
        }
	}
}
#endif
