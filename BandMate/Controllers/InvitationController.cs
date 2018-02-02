using BandMate.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandMate.Controllers
{
    public class InvitationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invitation
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Bands)
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            var invitations = db.Invitations
                .Where(i => i.IsAccepted == false)
                .Where(i => i.Email == user.Email)
                .ToList();

            if (TempData["infoMessage"] != null)
            {
                ViewBag.infoMessage = TempData["infoMessage"].ToString();
            }
            if (TempData["dangerMessage"] != null)
            {
                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
            }

            return View(invitations);
        }

        public ActionResult Decline(int invitationId)
        {
            var invitation = db.Invitations.Find(invitationId);
            db.Invitations.Remove(invitation);
            db.SaveChanges();
            TempData["infoMessage"] = "Invitation declined.";
            return RedirectToAction("Index");
        }

        public ActionResult Accept(int invitationId)
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Bands)
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            var invitation = db.Invitations.Find(invitationId);
            invitation.IsAccepted = true;
            db.SaveChanges();

            BandMember bandMember = new BandMember();
            bandMember.UserId = user.Id;
            bandMember.Title = invitation.Title;
            bandMember.BandId = invitation.BandId;
            db.BandMembers.Add(bandMember);
            db.SaveChanges();

            var band = db.Bands
                .Include(b => b.BandMembers)
                .Where(b => b.BandId == invitation.BandId)
                .FirstOrDefault();

            band.BandMembers.Add(bandMember);
            db.SaveChanges();

            return RedirectToAction("Index", "BandMember", new { bandId = band.BandId } );
        }

    }
}