using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandMate.Controllers
{
    public class BandMemberController : Controller
    {
        // GET: BandMember
        public ActionResult Index()
        {
            return View();
        }
    }
}