using System;

namespace Assets.Scripts.Core.Managers.Helpers
{
	public static class EnumExtensions
	{
		public static bool HasFlags ( this Enum variable, Enum flags )
		{
			if( variable == null || flags == null )
			{
				return false;
			}

			var num = Convert.ToUInt64( flags );
			return ( Convert.ToUInt64( variable ) & num ) == num;
		}
	}
}
