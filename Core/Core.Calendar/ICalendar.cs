using System;
using System.Collections.Generic;

namespace Core.Calendar
{
	public interface ICalendar
	{
		IEnumerable<AppointmentBase> Appointments { get; }
	}
}

