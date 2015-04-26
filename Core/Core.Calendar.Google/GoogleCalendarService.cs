using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.Calendar;

namespace Core.Calendar.Google
{
	public class GoogleCalendarService
	{
		private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
		private const string ApplicationName = "AtractanthaAureolanata";

		private CalendarService service;
		private string calendarId;

		public GoogleCalendarService (IGoogleConfig config)
		{

			service = new CalendarService (new BaseClientService.Initializer () {
				HttpClientInitializer = config.Auth.Authorize (googleUser: config.GoogleUser),
				ApplicationName = ApplicationName,
			});

			var calendars = service.CalendarList.List ().Execute ().Items;
			Log.Info ("List of google calendars:");
			Log.Indent++;
			foreach (CalendarListEntry entry in calendars) {
				Log.Info (entry.Summary + " - " + entry.Id);
				if (entry.Summary.ToLower ().Contains ("stundenplan")) {
					calendarId = entry.Id;
					Log.Indent++;
					Log.Info ("We'll use this one!");
					Log.Indent--;
				}
			}
			Log.Indent--;

			if (string.IsNullOrWhiteSpace (calendarId)) {
				throw new ArgumentException ("The google account has no calendar that matches the required name.");
			}
		}

		public void Clear ()
		{
			Events existingEvents = ListEvents ();

			foreach (Event e in existingEvents.Items) {
				Log.Debug ("delete: ", e.Summary, "/", e.Start.DateTimeRaw);

				try {
					var request = service.Events.Delete (calendarId: calendarId, eventId: e.Id);
					request.Execute ();
					//PortableThread.Sleep (1000);
				} catch (Exception ex) {
					Log.Error (ex);
				}
			}
		}

		public void Sync (CalendarBase cal)
		{
			Events existingEvents = ListEvents ();
			IEnumerable<Event> allEvents = ConvertEvents (cal);

			IEnumerable<Event> newEvents = from e in allEvents
			                               where existingEvents.Items.All (ee => ee.Start.DateTime != e.Start.DateTime)
			                               select e; 
			AddEvents (newEvents);

			foreach (Event e in allEvents) {
				IEnumerable<Event> sameEvents = existingEvents.Items.Where (ee => ee.Start.DateTime == e.Start.DateTime); // ee.Summary == e.Summary && 
				Log.Debug (e.Summary, "/", e.Start.DateTimeRaw, "/", string.Join (",", sameEvents));
				UpdateEvents (sameEvents, e);
			}
		}

		Events ListEvents ()
		{
			Events request = null;
			var lr = service.Events.List (calendarId);

			lr.TimeMin = DateTime.Now.AddDays (-360); //five days in the past
			lr.TimeMax = DateTime.Now.AddDays (360); //five days in the future

			request = lr.Execute ();
			return request;
		}

		IEnumerable<Event> ConvertEvents (CalendarBase cal)
		{
			foreach (AppointmentBase appointment in cal.Appointments) {
				var newEvent = new Event {
					Start = new EventDateTime { DateTime = appointment.StartDate },
					End = new EventDateTime { DateTime = appointment.EndDate },
					Description = appointment.Body,
					Summary = appointment.Title,
					Location = appointment.Location,
				};
				yield return newEvent;
			}
		}

		void AddEvents (IEnumerable<Event> events)
		{
			foreach (Event e in events) {
				Log.Debug ("Add event: ", e.Start, e.Summary, e.Location);
				var request = service.Events.Insert (e, calendarId);
				request.Execute ();
			}
		}

		void UpdateEvents (IEnumerable<Event> events, Event update)
		{
			foreach (Event e in events) {
				Log.Debug ("Update event: ", e.Start, e.Summary, e.Location);
				e.Summary = update.Summary;
				e.Location = update.Location;
				e.End = update.End;
				e.Description = update.Description;

				try {
					var request = service.Events.Update (body: e, calendarId: calendarId, eventId: e.Id);
					request.Execute ();
					PortableThread.Sleep (1000);
				} catch (Exception ex) {
					Log.Error (ex);
				}
			}
		}
	}
}

