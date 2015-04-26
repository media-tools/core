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
	public class GoogleCalendarService : CalendarBase<GoogleAppointment>
	{
		private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
		private const string ApplicationName = "AtractanthaAureolanata";

		private readonly CalendarService service;
		private readonly string calendarId;

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
			GoogleAppointment[] existingEvents = Appointments.ToArray ();

			foreach (GoogleAppointment app in existingEvents) {
				Log.Debug ("delete: ", app);

				app.Delete ();
				PortableThread.Sleep (1000);
			}
		}

		Events ListInternalEvents ()
		{
			var lr = service.Events.List (calendarId);

			//lr.TimeMin = DateTime.Now.AddDays (-9999); //five days in the past
			//lr.TimeMax = DateTime.Now.AddDays (9999); //five days in the future

			lr.MaxResults = 2499;
			//lr.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
			//lr.ShowDeleted = true;

			Log.Debug ("TimeMin: ", lr.TimeMin.ToString (), ", TimeMax: ", lr.TimeMax.ToString ());

			Events request = lr.Execute ();
			return request;
		}

		public override IEnumerable<GoogleAppointment> Appointments {
			get {
				try {
					List<GoogleAppointment> result = new List<GoogleAppointment> ();
					Events internalEvents = ListInternalEvents ();
					foreach (Event internalEvent in internalEvents.Items) {
						result.Add (new GoogleAppointment (internalEvent: internalEvent, service: service, calendarId: calendarId));
					}
					return result.ToArray ();
				} catch (Exception ex) {
					Log.Error (ex);
					return new GoogleAppointment[]{ };
				}
			}
		}

		public void AddAppointment (AppointmentBase appointment)
		{
			try {
				GoogleAppointment.Insert (appointment: appointment, service: service, calendarId: calendarId);
			} catch (Exception ex) {
				Log.Error (ex);
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

