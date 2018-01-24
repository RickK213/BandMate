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

            List<Band> inviteBands = new List<Band>();
            if ( invitations.Count>0 )
            {
                foreach (Invitation invitation in invitations)
                {
                    var band = db.Bands
                        .Include(b => b.BandMembers)
                        .Where(b => b.BandId == invitation.BandId)
                        .FirstOrDefault();
                    inviteBands.Add(band);
                }
            }

            InvitationIndexViewModel viewModel = new InvitationIndexViewModel();
            viewModel.Invitations = invitations;
            viewModel.InviteBands = inviteBands;

            return View(viewModel);
        }
    }
}