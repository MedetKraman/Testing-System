using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hadis.Models.DBModels;

namespace Hadis.Controllers
{
    public class CallBackMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CallBackMessages/Create
        public ActionResult Create(int clientCallBackId)
        {
            string userId = db.Users.Where(u => u.UserName == User.Identity.Name).Single().Id;
            ViewBag.Thema = db.ClientCallBacks.Find(clientCallBackId).Thema;
            return View(new CallBackMessage { ClientCallBackId = clientCallBackId, UserId = userId, DateTime = DateTime.Now });
        }

        // POST: CallBackMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DateTime,Message,UserId,ClientCallBackId")] CallBackMessage callBackMessage)
        {
            if (ModelState.IsValid)
            {
                db.CallBackMessages.Add(callBackMessage);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", routeValues: new { controller = "ClientCallBacks", id = callBackMessage.ClientCallBackId });
            }

            ViewBag.Thema = db.ClientCallBacks.Find(callBackMessage.ClientCallBackId).Thema;
            return View(callBackMessage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
