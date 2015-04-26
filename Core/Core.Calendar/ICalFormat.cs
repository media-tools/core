using System;
using DDay.iCal.Serialization.iCalendar;
using DDay.iCal;

namespace Core.Calendar
{
	public static class ICalFormat
	{
		public static string Export (CalendarBase calendar)
		{
			var iCal = new iCalendar {
				Method = "PUBLISH",
				Version = "2.0"
			};

			foreach (AppointmentBase appointment in calendar.Appointments) {
				CreateCalendarEvent (iCal: ref iCal, appointment: appointment);
			}

			return new iCalendarSerializer ().SerializeToString (iCal);
		}

		private static void CreateCalendarEvent (ref iCalendar iCal, AppointmentBase appointment)
		{
			// mandatory for outlook 2007
			//if (String.IsNullOrEmpty (organizer))
			//	throw new Exception ("Organizer provided was null");

			// "REQUEST" will update an existing event with the same UID (Unique ID) and a newer time stamp.
			//if (updatePreviousEvent)
			//{
			//    iCal.Method = "REQUEST";
			//}

			var evt = iCal.Create<Event> ();
			evt.Summary = appointment.Title;
			evt.Start = new iCalDateTime (appointment.StartDate);
			evt.Duration = appointment.Duration; // TimeSpan.FromHours (duration);
			evt.Description = appointment.Body;
			evt.Location = appointment.Location;
			evt.IsAllDay = appointment.IsAllDayEvent;
			evt.UID = appointment.UID; //String.IsNullOrEmpty (eventId) ? new Guid ().ToString () : eventId;
			evt.Organizer = new Organizer (appointment.Organizer);
			evt.Alarms.Add (new Alarm {
				Duration = new TimeSpan (0, 15, 0),
				Trigger = new Trigger (new TimeSpan (0, 15, 0)),
				Action = AlarmAction.Display,
				Description = "Reminder"
			});
		}
	}
}

