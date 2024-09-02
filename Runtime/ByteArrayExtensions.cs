using System.Text;

namespace CrazyPanda.UnityCore.Utils
{
	public static class ByteArrayExtensions
	{
		/// <summary>
		/// Human readable bytes
		/// Often Used for stringifiyng md5 hash
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToPrettyString(this byte[] bytes)
		{
			var sb = new StringBuilder();
			for( var i = 0; i < bytes.Length; i++ )
			{
				sb.Append( bytes[ i ].ToString( "x2" ) );
			}
			return sb.ToString();
		}
    }
}