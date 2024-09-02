using System;

namespace CrazyPanda.UnityCore.Utils
{
	public static class DateTimeExtensions
	{
		private static readonly DateTime _origin = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
		
		private static readonly int[] s_daysToMonth365 = new int[13]
		{
			0,
			31,
			59,
			90,
			120,
			151,
			181,
			212,
			243,
			273,
			304,
			334,
			365
		};
		private static readonly int[] s_daysToMonth366 = new int[13]
		{
			0,
			31,
			60,
			91,
			121,
			152,
			182,
			213,
			244,
			274,
			305,
			335,
			366
		};

		public static long ToUnixTimestamp( this DateTime dateTime )
		{
            return new DateTimeOffset( dateTime ).ToUnixTimeSeconds();
		}

		public static double ToDoubleUnixTimestamp( this DateTime dateTime )
		{
            return new DateTimeOffset( dateTime ).ToUnixTimeSeconds();
		}

		public static DateTime FromUnixTimestamp( long seconds )
		{
            return DateTimeOffset.FromUnixTimeSeconds( seconds ).UtcDateTime;
        }

		public static DateTime FromUnixTimestamp( double seconds )
		{
			return new DateTime( _origin.Ticks + ( long ) ( seconds * TimeSpan.TicksPerSecond ), DateTimeKind.Utc );
		}

        public static DateTime AsUnixTime( this long seconds )
		{
            return FromUnixTimestamp( seconds );
        }

        public static DateTime FromUnixTimestampMilliseconds( long timeStamp )
        {
            return DateTimeOffset.FromUnixTimeMilliseconds( timeStamp ).UtcDateTime;
        }

        public static long ToUnixTimestampMilliseconds( this DateTime dateTime )
        {
            return new DateTimeOffset( dateTime ).ToUnixTimeMilliseconds();
        }
		/// <summary>
		/// Threadsafe fast way to extract year, month, day in one call, twice faster than getting this them separatly
		/// code copied from internal DateTime.GetDatePart
		/// </summary>
		/// <cref>DateTime.GetDatePart</cref>
		public static unsafe void GetDateParts( this DateTime date, out int year, out int month, out int day )
		{
			ulong _dateData = *(( ulong* ) &date);
			long internalTicks = (long) _dateData & 4611686018427387903L;
			int num1 = (int) (internalTicks / 864000000000L);
			int num2 = num1 / 146097;
			int num3 = num1 - num2 * 146097;
			int num4 = num3 / 36524;
			if (num4 == 4)
				num4 = 3;
			int num5 = num3 - num4 * 36524;
			int num6 = num5 / 1461;
			int num7 = num5 - num6 * 1461;
			int num8 = num7 / 365;
			if (num8 == 4)
				num8 = 3;
			year = num2 * 400 + num4 * 100 + num6 * 4 + num8 + 1;
			int num9 = num7 - num8 * 365;
			int[] numArray = (num8 != 3 ? 0 : (num6 != 24 ? 1 : (num4 == 3 ? 1 : 0))) != 0 ? DateTimeExtensions.s_daysToMonth366 : DateTimeExtensions.s_daysToMonth365;
			int index = (num9 >> 5) + 1;
			while (num9 >= numArray[index])
				++index;
			month = index;
			day = num9 - numArray[index - 1] + 1;
		}
	}
}
