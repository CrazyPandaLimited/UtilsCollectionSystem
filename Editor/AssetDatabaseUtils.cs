#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CrazyPanda.UnityCore.Utils.Editor
{
	public static class AssetDatabaseUtils
	{
		/// <summary>
		/// Поиск в проекте или указанной папке префабов содержащих компонент указанного типа
		/// </summary>
		public static IEnumerable< T > FindAssetsWithComponent< T >( string root = null ) where T : Component
		{
			return PassThroughAssetsOfType( typeof( GameObject ), ".prefab", root ).Select( x => ( ( GameObject ) x ).GetComponent< T >() ).Where( x => x != null );
		}

        public static IEnumerable< T > FindAssetsWithComponentInChildren< T >( string root = null ) where T : Component
        {
            return PassThroughAssetsOfType( typeof( GameObject ), ".prefab", root ).SelectMany( x => ( ( GameObject ) x ).GetComponentsInChildren< T >(true) ).Where( x => x != null );
        }
        
		/// <summary>
		/// Проход по ассетам проекта/папки с поиском ассетов заданного типа
		/// </summary>
		public static IEnumerable< Object > PassThroughAssetsOfType( Type type, string fileExtension, string root = null )
		{
			var directory = new DirectoryInfo( root ?? Application.dataPath );
			var files = directory.GetFiles( "*" + fileExtension, SearchOption.AllDirectories );
			var i = 0;
			foreach( var tempGoFileInfo in files )
			{
				if( tempGoFileInfo != null )
				{
					var tempFilePath = PathUtils.Normalize( tempGoFileInfo.FullName ).Replace( Application.dataPath, "Assets" );
					var tempGo = AssetDatabase.LoadAssetAtPath< Object >( tempFilePath );

					if( tempGo != null && tempGo.GetType() == type )
						yield return tempGo;
				}
				++i;
			}
		}
	}
}

#endif