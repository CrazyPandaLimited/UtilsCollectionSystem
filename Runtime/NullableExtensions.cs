using System;
using System.Runtime.CompilerServices;

namespace CrazyPanda.UnityCore.Utils
{
    public static class NullableExtensions
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
        public static T CheckArgumentForNull <T>( this T @object, string memberName ) where T : class
        {
            if( @object == null )
            {
                throw new ArgumentNullException( memberName );
            }

            return @object;
        }

        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
        public static string ValidateEmptyString( this string @object, string memberName )
        {
            if( string.IsNullOrEmpty( @object ) )
            {
                throw new ArgumentNullException( memberName );
            }
            
            return @object;
        }
    }
}