using System;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;

namespace Core.Calendar
{
	public class AppointmentBase
	{
		public string Title { get; set; }

		public string Organizer { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public TimeSpan Duration { get { return EndDate - StartDate; } set { EndDate = StartDate + value; } }

		public string Body { get; set; }

		public string Location { get; set; }

		public bool IsAllDayEvent { get; set; }

		public string UID { get; set; }

		public AppointmentBase ()
		{
			Title = "";
			Organizer = "test@example.com";
			Body = "";
			Location = "";
			UID = "";
		}

		public override string ToString ()
		{
			return string.Format ("[AppointmentBase: Title={0}, Organizer={1}, StartDate={2}, EndDate={3}, Body={4}, Location={5}, IsAllDayEvent={6}, UID={7}]", Title, Organizer, StartDate, EndDate, Body != null ? Body.Length : 0, Location, IsAllDayEvent, UID);
		}
	}

}

