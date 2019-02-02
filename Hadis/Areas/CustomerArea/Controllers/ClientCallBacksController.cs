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
    public class ClientCallBacksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerArea/ClientCallBacks
        public async Task<ActionResult> Index()
        {
            var clientCallBacks = db.ClientCallBacks.Include(c => c.Client);
            return View(await clientCallBacks.ToListAsync());
        }

        // GET: CustomerArea/ClientCallBacks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCallBack clientCallBack = await db.ClientCallBacks.FindAsync(id);
            if (clientCallBack == null)
            {
                return HttpNotFound();
            }
            clientCallBack.CallBackMessages = await db.CallBackMessages.Include(u => u.User).Where(u => u.ClientCallBackId == id).ToListAsync();
            return View(clientCallBack);
        }

        // GET: CustomerArea/ClientCallBacks/Create
        public ActionResult Create()
        {
            string clientId = db.Users.Where(u => u.UserName == User.Identity.Name).Single().Id;
            return View(new ClientCallBack { IsThemaClosed = false, DateTime = DateTime.Now, ClientId = clientId });
        }

        // POST: CustomerArea/ClientCallBacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DateTime,Thema,IsThemaClosed,SatisfactionScore,ClientId")] ClientCallBack clientCallBack)
        {
            if (ModelState.IsValid)
            {
                db.ClientCallBacks.Add(clientCallBack);
                await db.SaveChangesAsync();
                return RedirectToAction("Create", routeValues: new { controller = "CallBackMessages", clientCallBackId = clientCallBack.Id });
            }
            
            return View(clientCallBack);
        }

        // GET: CustomerArea/ClientCallBacks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCallBack clientCallBack = await db.ClientCallBacks.FindAsync(id);
            if (clientCallBack == null)
            {
                return HttpNotFound();
            }
            return View(clientCallBack);
        }

        // POST: CustomerArea/ClientCallBacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DateTime,Thema,IsThemaClosed,SatisfactionScore,ClientId")] ClientCallBack clientCallBack)
        {
            if (ModelState.IsValid)
            {
                clientCallBack.IsThemaClosed = true;
                db.Entry(clientCallBack).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", routeValues: new { id = clientCallBack.Id });
            }
            return View(clientCallBack);
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
