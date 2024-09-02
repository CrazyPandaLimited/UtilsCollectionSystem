using System;
using System.Text.RegularExpressions;

namespace CrazyPanda.UnityCore.Utils
{
	public static class StringExtensions
	{
		/// <summary>
		/// Удаляет строку pefix с начала строки str, если он есть
		/// </summary>
		public static string TrimStart( this string str, string prefix, StringComparison stringComparision = StringComparison.InvariantCulture )
		{
			if( !string.IsNullOrEmpty( str ) && str.StartsWith( prefix, stringComparision ) )
			{
				return str.Substring( prefix.Length );
			}
			return str;
		}

		/// <summary>
		/// Удаляет строку suffix с конца строки str, если он есть
		/// </summary>
		public static string TrimEnd( this string str, string suffix, StringComparison stringComparision = StringComparison.InvariantCulture )
		{
			if( !string.IsNullOrEmpty( str ) && str.EndsWith( suffix, stringComparision ) )
			{
				return str.Substring( 0, str.Length - suffix.Length );
			}
			return str;
		}

		/// <summary>
		/// Более короткий вариант !string.IsNullOrEmpty
		/// </summary>
		public static bool IsNotEmpty( this string str )
		{
			return !string.IsNullOrEmpty( str );
		}

		/// <summary>
		/// Строка не нулевая не нулевой длины и не состоит полностью их пробелов.
		/// Более короткий вариант string!=null && string.Trim().Length>0
		/// </summary>
		public static bool IsNotBlank( this string str )
		{
			if( str != null )
			{
				for( var i = 0; i < str.Length; ++i )
				{
					if( !char.IsWhiteSpace( str[ i ] ) )
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Строка нулевая или нулевой длины или состоит полностью их пробелов.
		/// Более короткий вариант string==null || string.Trim().Length==0
		/// </summary>
		public static bool IsBlank( this string str )
		{
			if( str != null )
			{
				for( var i = 0; i < str.Length; ++i )
				{
					if( !char.IsWhiteSpace( str[ i ] ) )
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Вариант string.Contains игнорирующий регистр букв
		/// </summary>
		public static bool ContainsIgnoreCase( this string a, string b )
		{
			return a.IndexOf( b, StringComparison.OrdinalIgnoreCase ) != -1;
		}

		/// <summary>
		/// Укороченный вариант x.Equals(y,StringComparison.InvariantCultureIgnoreCase)
		/// </summary>
		public static bool EqualsIgnoreCase( this string a, string b )
		{
			return a.Equals( b, StringComparison.InvariantCultureIgnoreCase );
		}

		public static string SplitCamelCase( this string s )
		{
			if( string.IsNullOrEmpty( s ) )
			{
				return s;
			}
			s = char.ToUpperInvariant( s[ 0 ] ) + s.Substring( 1 );
			return Regex.Replace( s, "(?<=[a-z])([A-Z])", " $1" ).Trim();
		}
	}
}
