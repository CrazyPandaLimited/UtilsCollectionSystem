using System.Globalization;

namespace CrazyPanda.UnityCore.Utils
{
	public static class IntExtensions
	{
		private const string SingleSpace = " ";

		private static readonly NumberFormatInfo PrettyNumberFormat = new NumberFormatInfo
		{
			NumberGroupSeparator = SingleSpace
		};

		/// <summary>
		/// ToString в формате 1 000 000
		/// </summary>
		public static string ToPrettyString( this int value )
		{
			return value.ToString( "N0", PrettyNumberFormat );
		}

		/// <summary>
		/// ToString в формате 1 000 000
		/// </summary>
		public static string ToPrettyString( this long value )
		{
			return value.ToString( "N0", PrettyNumberFormat );
		}
	}
}
