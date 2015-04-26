using System;

namespace Core.Calendar
{
	public interface IEditableCalendar : ICalendar
	{
		void AddAppointment (AppointmentBase appointment);
	}
}

