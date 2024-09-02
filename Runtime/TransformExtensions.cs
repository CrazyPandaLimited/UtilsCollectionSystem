#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CrazyPanda.UnityCore.Utils
{
	public static class TransformExtensions
	{
		public static void RoundPosition( this Transform transform, bool includeChildren = true )
		{
			if( includeChildren )
			{
				transform.RoundPosition( false );

				for( var i = 0; i < transform.childCount; i++ )
				{
					transform.GetChild( i ).RoundPosition( true );
				}
			}
			else
			{
				var position = transform.localPosition;
				transform.localPosition = new Vector3( Mathf.RoundToInt( position.x ), Mathf.RoundToInt( position.y ), Mathf.RoundToInt( position.z ) );
			}
		}
		
		public static void SetTransformAfterOrBefore( this Transform currentTransform, Transform target, bool isBefore )
		{
			if( currentTransform.parent != target.parent )
			{
				currentTransform.SetParent( target.parent );
			}

			var targetSublingIndex = target.GetSiblingIndex();
			var needSublingIndex = isBefore ? targetSublingIndex - 1 : targetSublingIndex + 1;
			var currenTransoformSublingIndex = currentTransform.GetSiblingIndex();

			if( needSublingIndex != currenTransoformSublingIndex )
			{
				if( isBefore )
				{
					currentTransform.SetSiblingIndex( currenTransoformSublingIndex > targetSublingIndex ? targetSublingIndex : needSublingIndex );
				}
				else
				{
					currentTransform.SetSiblingIndex( currenTransoformSublingIndex < targetSublingIndex ? targetSublingIndex : needSublingIndex );
				}
			}
		}
	}
}
#endif
