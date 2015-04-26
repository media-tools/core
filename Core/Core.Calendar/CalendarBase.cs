using System;
using System.Collections.Generic;

namespace Core.Calendar
{
	public abstract class CalendarBase : ICalendar
	{
		public abstract IEnumerable<AppointmentBase> Appointments { get; }

		public CalendarBase ()
		{
		}
	}
}

