﻿#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;

namespace CrazyPanda.UnityCore.Utils
{
	public static class IListExtensions
	{
		public static void Shuffle< T >( this IList< T > list )
		{
			var n = list.Count;
			while( n > 1 )
			{
				n--;
				var k = Random.Range( 0, n + 1 );
				var value = list[ k ];
				list[ k ] = list[ n ];
				list[ n ] = value;
			}
		}
	}
}
#endif
