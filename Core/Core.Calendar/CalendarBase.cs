using System;
using System.Collections.Generic;

namespace Core.Calendar
{
	public abstract class CalendarBase<AppointmentType> where AppointmentType : AppointmentBase
	{
		public abstract IEnumerable<AppointmentType> Appointments { get; }

		public CalendarBase ()
		{
		}
	}
}

