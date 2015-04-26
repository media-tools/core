using System;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Core.Common;

namespace Core.Calendar.Google
{
	public class GoogleAppointment : AppointmentBase, IEditableAppointment
	{
		private readonly Event internalEvent;
		private readonly CalendarService service;
		private readonly string calendarId;

		public GoogleAppointment (Event internalEvent, CalendarService service, string calendarId)
		{
			this.service = service;
			this.internalEvent = internalEvent;
			this.calendarId = calendarId;

			Copy (from: internalEvent, to: this);
		}

		public static void Insert (AppointmentBase appointment, CalendarService service, string calendarId)
		{
			var newEvent = new Event { };
			Copy (from: appointment, to: newEvent);

			try {
				var request = service.Events.Insert (newEvent, calendarId);
				request.Execute ();
			} catch (Exception ex) {
				Log.Error (ex);
			}
		}

		#region IEditableAppointment implementation

		public void Update ()
		{
			Copy (from: this, to: internalEvent);
			try {
				var request = service.Events.Update (body: internalEvent, calendarId: calendarId, eventId: internalEvent.Id);
				request.Execute ();
			} catch (Exception ex) {
				Log.Error (ex);
			}
		}

		public void Delete ()
		{
			try {
				var request = service.Events.Delete (calendarId: calendarId, eventId: internalEvent.Id);
				request.Execute ();
				//PortableThread.Sleep (1000);
			} catch (Exception ex) {
				Log.Error (ex);
			}
		}

		#endregion

		private static void Copy (Event from, AppointmentBase to)
		{
			if (from.Start != null && from.Start.DateTime.HasValue)
				to.StartDate = from.Start.DateTime.Value;
			if (from.End != null && from.End.DateTime.HasValue)
				to.EndDate = from.End.DateTime.Value;
			to.Body = from.Description;
			to.Title = from.Summary;
			to.Location = from.Location;
			to.UID = from.ICalUID;
		}

		private static void Copy (AppointmentBase from, Event to)
		{
			to.Start = new EventDateTime{ DateTime = from.StartDate };
			to.End = new EventDateTime{ DateTime = from.EndDate };
			to.Description = from.Body;
			to.Summary = from.Title;
			to.Location = from.Location;
		}

		public override string ToString ()
		{
			return string.Format ("[GoogleAppointment: Title={0}, Organizer={1}, StartDate={2}, EndDate={3}, Body={4}, Location={5}, IsAllDayEvent={6}, UID={7}]", Title, Organizer, StartDate, EndDate, Body != null ? Body.Length : 0, Location, IsAllDayEvent, UID);
		}
	}
}

