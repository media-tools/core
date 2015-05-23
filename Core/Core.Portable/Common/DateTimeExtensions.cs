using System;

namespace Core.Common
{
	public static class DateTimeExtensions
	{
		private static long EPOCH_TICKS { get { return new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks; } }

		public static DateTimeOffset MillisToDateTimeOffset (long milliSecondsSinceEpoch, long offsetMinutes)
		{
			TimeSpan offset = TimeSpan.FromMinutes ((double)offsetMinutes);
			long num = EPOCH_TICKS + (milliSecondsSinceEpoch * 10000);
			return new DateTimeOffset (num + offset.Ticks, offset);
		}

		public static long ToMillisecondsSinceEpoch (this DateTime dateTime)
		{
			if (dateTime.Kind != DateTimeKind.Utc) {
				throw new ArgumentException ("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
			}
			return new DateTimeOffset (DateTime.SpecifyKind (dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch ();
		}

		public static long ToMillisecondsSinceEpoch (this DateTimeOffset dateTimeOffset)
		{
			return (((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) - EPOCH_TICKS) / TimeSpan.TicksPerMillisecond);
		}
	}
}

