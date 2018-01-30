using BandMate.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using SendGrid;
using SendGrid.Helpers.Mail;



namespace BandMate.Controllers
{
    public class EventController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Event
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int bandId, DateTime eventDate, string name, string description)
        {
            Event newEvent = new Event();
            newEvent.BandId = bandId;
            newEvent.EventDate = eventDate;
            newEvent.Name = name;
            newEvent.Description = description.Replace(System.Environment.NewLine, " ");
            db.Events.Add(newEvent);
            db.SaveChanges();
            TempData["infoMessage"] = name + " created!";
            return RedirectToAction("Events", "Band", new { bandId = bandId });            
        }

        [HttpGet]
        public ActionResult Edit(int eventId)
        {
            var eventToEdit = db.Events.Find(eventId);
            return View(eventToEdit);
        }

        [HttpPost]
        public ActionResult Edit(int eventId, DateTime eventDate, string name, string description)
        {
            Event eventToEdit = db.Events.Find(eventId);
            eventToEdit.EventDate = eventDate;
            eventToEdit.Name = name;
            eventToEdit.Description = description.Replace(System.Environment.NewLine, " ");
            db.SaveChanges();
            TempData["infoMessage"] = "Success! Event modified!";
            return RedirectToAction("Events", "Band", new { bandId = eventToEdit.BandId });
        }

        [HttpGet]
        public ActionResult Delete(int eventId)
        {
            Event eventToDelete = db.Events.Find(eventId);
            db.Events.Remove(eventToDelete);
            db.SaveChanges();
            TempData["infoMessage"] = "You have removed the event: " + eventToDelete.Name;
            return RedirectToAction("Events", "Band", new { bandId = eventToDelete.BandId });
        }

        [HttpGet]
        public ActionResult SendReminders(DateTime eventDate, int bandId)
        {
            //get all band members for the band.
            //generate the link url
            //foreach band member
            //send email with link
            Band band = db.Bands
                .Include(b => b.BandMembers)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            if ( band.BandMembers.Count <= 0 )
            {
                TempData["dangerMessage"] = "There are no members in your band. No reminders were sent.";
                return RedirectToAction("Events", "Band", new { bandId = bandId });
            }
            
            //URL to daily itinerary:
            string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            url += "Event/Itinerary?eventDate=" + String.Format("{0:MM/dd/yy}", eventDate) + "&bandId=" + bandId;
            //Band Name
            string bandName = band.Name;

            //Email subject and content
            string subject = String.Format("Itinerary for {0} on {1:MM/dd/yy}", bandName, eventDate);
            string plainTextContent = String.Format("Here is a link to events on {0:MM/dd/yy} for your band {1}:\r\n{2}", eventDate, bandName, url);
            string htmlTextContent = String.Format("<p>Here is a link to events on {0:MM/dd/yy} for your band {1}:</p><p>{2}</p>", eventDate, bandName, url);

            //Send out each reminder
            foreach (BandMember bandMember in band.BandMembers)
            {
                var user = db.Users
                    .Include(u => u.NotificationPreference)
                    .Where(u => u.Id == bandMember.UserId)
                    .FirstOrDefault();

                if (user.NotificationPreference.Name == "Email")
                {
                    string email = user.Email;
                    EmailIninerary(email, subject, plainTextContent, htmlTextContent);
                }

            }

            return View();
        }

        private void EmailIninerary(string toEmail, string subject, string plainTextContent, string htmlTextContent)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            KeyManager keyManager = new KeyManager();
            var apiKey = keyManager.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("info@bandmate.com", "BandMate");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlTextContent);
            var response = client.SendEmailAsync(msg);
        }


        public ActionResult Itinerary(DateTime eventDate, int bandId)
        {
            Band band = db.Bands
                .Include(b => b.Events)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            string eventsJson = "[";
            foreach (Event bandEvent in band.Events)
            {
                eventsJson += "{";
                eventsJson += "\"id\": " + bandEvent.EventId + ",";
                eventsJson += "\"title\": \"" + bandEvent.Name + "\",";
                eventsJson += "\"start\": \"" + bandEvent.EventDate.ToString("r") + "\",";
                eventsJson += "\"description\": \"" + bandEvent.Description + "\"";
                eventsJson += "},";
            }
            eventsJson += "]";

            return View();
        }

    }
}