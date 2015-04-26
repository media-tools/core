using System;

namespace Core.Calendar
{
	public interface IAppointment : IEquatable<IAppointment>
	{
		string Title { get; set; }

		string Organizer { get; set; }

		DateTime StartDate { get; set; }

		DateTime EndDate { get; set; }

		TimeSpan Duration { get; set; }

		string Body { get; set; }

		string Location { get; set; }

		bool IsAllDayEvent { get; set; }

		string UID { get; set; }

		void CopyTo (IAppointment other);
	}
}

