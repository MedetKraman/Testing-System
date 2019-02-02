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

namespace Hadis.Areas.CustomerArea.Controllers
{
    public class CallBackMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerArea/CallBackMessages
        public async Task<ActionResult> Index()
        {
            var callBackMessages = db.CallBackMessages.Include(c => c.ClientCallBack).Include(c => c.User);
            return View(await callBackMessages.ToListAsync());
        }

        // GET: CustomerArea/CallBackMessages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CallBackMessage callBackMessage = await db.CallBackMessages.FindAsync(id);
            if (callBackMessage == null)
            {
                return HttpNotFound();
            }
            return View(callBackMessage);
        }

        // GET: CustomerArea/CallBackMessages/Create
        public ActionResult Create(int clientCallBackId)
        {
            string clientId = db.Users.Where(u => u.UserName == User.Identity.Name).Single().Id;
            ViewBag.Thema = db.ClientCallBacks.Find(clientCallBackId).Thema;
            return View(new CallBackMessage { ClientCallBackId = clientCallBackId, UserId = clientId, DateTime = DateTime.Now });
        }

        // POST: CustomerArea/CallBackMessages/Create
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
