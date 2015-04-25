using System;
using System.Collections.Generic;

namespace Core.Calendar
{
	public abstract class CalendarBase
	{
		public abstract IEnumerable<AppointmentBase> Appointments { get; }

		public CalendarBase ()
		{
		}
	}
}

