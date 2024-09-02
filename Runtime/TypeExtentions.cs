using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyPanda.UnityCore.Utils
{
	public static class TypeExtentions
    {
        private const char dotSymbol = '.';
        private static readonly List< Type > _list = new List< Type >();
        private static readonly StringBuilder _stringBuilder = new StringBuilder( 1024 * 4 );
        
        private static readonly Dictionary< Type, string > _cachedTypeNames = new Dictionary< Type, string >
        {
            [ typeof( string ) ] = "string",
            [ typeof( bool ) ] = "bool",
            [ typeof( byte ) ] = "byte",
            [ typeof( char ) ] = "char",
            [ typeof( decimal ) ] = "decimal",
            [ typeof( double ) ] = "double",
            [ typeof( float ) ] = "float",
            [ typeof( int ) ] = "int",
            [ typeof( sbyte ) ] = "sbyte",
            [ typeof( uint ) ] = "uint",
            [ typeof( ulong ) ] = "ulong",
            [ typeof( ushort ) ] = "ushort",
            [ typeof( short ) ] = "short",
            [ typeof( long ) ] = "long",
            [ typeof( void ) ] = "void",
            [ typeof( object ) ] = "object"
        };

        private static readonly Dictionary< Type, string > _cachedFullTypeNames;

        static TypeExtentions()
        {
            _cachedFullTypeNames = new Dictionary< Type, string >();

            foreach( var cachedTypeName in _cachedTypeNames )
            {
                _cachedFullTypeNames[ cachedTypeName.Key ] = cachedTypeName.Value;
            }
        }

		/// <summary>
		/// Вернет красивое представление generic типа. Например, вместо List`1 => List<int>
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        public static string CSharpName( this Type type )
        {
            if( _cachedTypeNames.TryGetValue( type, out var typeName ) )
            {
                return typeName;
            }

            _stringBuilder.Clear();
            type.CSharpNameInternal( _stringBuilder );

            var result = _stringBuilder.ToString();
            _cachedTypeNames[ type ] = result;
            _stringBuilder.Clear();

            return result;
        }

        /// <summary>
        /// Вернет красивое представление generic типа. Например, вместо List`1 => List<int>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static void CSharpName( this Type type, StringBuilder stringBuilder )
        {
            stringBuilder.Append( type.CSharpName() );
        }
        
		/// <summary>
		/// Вернет полное имя generic типа, включая namespace
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        public static string FullTypeName( this Type type )
        {
            if( _cachedFullTypeNames.TryGetValue( type, out var typeName ) )
            {
                return typeName;
            }

            _stringBuilder.Clear();
            type.FullTypeNameInternal( _stringBuilder );

            var result = _stringBuilder.ToString();
            _cachedFullTypeNames[ type ] = result;
            _stringBuilder.Clear();

            return result;
        }

        /// <summary>
        /// Вернет полное имя generic типа, включая namespace
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static void FullTypeName( this Type type, StringBuilder stringBuilder )
        {
            stringBuilder.Append( type.FullTypeName() );
        }
        
        public static bool HasInterface( this Type type, Type interfaceTypeToCheck )
        {
            if( type.IsInterface( interfaceTypeToCheck ) )
            {
                return true;
            }
            
            var @interfaces = type.GetInterfaces();

            for( var i = 0; i < @interfaces.Length; ++i )
            {
                var @interface = @interfaces[ i ];

                if( @interface.IsInterface( interfaceTypeToCheck ) )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInterface( this Type typeToCheck, Type interfaceTypeToCheck )
        {
            return typeToCheck == interfaceTypeToCheck || (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == interfaceTypeToCheck);
        }
        
        private static void CSharpNameInternal( this Type type, StringBuilder stringBuilder )
        {
            var name = type.Name;

            if( name[ name.Length - 1 ] == '&' )
            {
                stringBuilder.Append( "ref" );
            }

            if( !type.IsGenericType )
            {
                stringBuilder.Append( _cachedTypeNames.TryGetValue( type, out string cachedTypeName ) ? cachedTypeName : name );
                return;
            }

            for( int i = 0; i < name.IndexOf( '`' ); ++i )
            {
                stringBuilder.Append( name[ i ] );
            }

            stringBuilder.Append( '<' );

            var genericArguments = type.GetGenericArguments();

            for( var i = 0; i < genericArguments.Length; ++i )
            {
                if( i > 0 )
                {
                    stringBuilder.Append( ", " );
                }

                genericArguments[ i ].CSharpNameInternal( stringBuilder );
            }

            stringBuilder.Append( '>' );
        }

        private static void FullTypeNameInternal( this Type type, StringBuilder stringBuilder, bool appendNestedTypes = true )
        {
            var name = type.Name;
            var nameSpace = type.Namespace;

            if( !type.IsGenericType && !type.IsNested )
            {
                if( _cachedTypeNames.TryGetValue( type, out string cachedTypeName ) )
                {
                    stringBuilder.Append( cachedTypeName );
                }
                else
                {
                    stringBuilder.Append( nameSpace ).Append( dotSymbol ).Append( name );
                }

                return;
            }

            if( type.IsNested && appendNestedTypes )
            {
                _list.Clear();
            
                for( Type iterationType = type.DeclaringType; iterationType != null; iterationType = iterationType.DeclaringType )
                {
                    _list.Add( iterationType );
                }

                for( int i = _list.Count - 1; i >= 0; --i )
                {
                    _list[ i ].FullTypeNameInternal( stringBuilder, appendNestedTypes: false );
                    stringBuilder.Append( dotSymbol );
                }
            
                _list.Clear();
            }
            
            if (!type.IsNested)
            {
                stringBuilder.Append( nameSpace ).Append( dotSymbol );
            }

            if( !type.IsGenericType )
            {
                stringBuilder.Append( name );
                return;
            }
            
            for( int i = 0; i < name.IndexOf( '`' ); ++i )
            {
                stringBuilder.Append( name[ i ] );
            }

            stringBuilder.Append( '<' );

            var genericArguments = type.GetGenericArguments();

            for( var i = 0; i < genericArguments.Length; ++i )
            {
                if( i > 0 )
                {
                    stringBuilder.Append( ", " );
                }

                genericArguments[ i ].FullTypeNameInternal( stringBuilder );
            }

            stringBuilder.Append( '>' );
        }
    }
}
