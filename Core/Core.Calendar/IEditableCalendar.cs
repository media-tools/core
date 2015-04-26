using System;

namespace Core.Calendar
{
	public interface IEditableCalendar
	{
		void AddAppointment (AppointmentBase appointment);
	}
}

