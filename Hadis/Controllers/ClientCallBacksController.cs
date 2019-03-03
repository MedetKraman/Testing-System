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
    public class ClientCallBacksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientCallBacks
        public async Task<ActionResult> Index(bool? isThemesClosed = null, DateTime? date = null)
        {
            var query = db.ClientCallBacks.Include(c => c.Client.User);
            if (isThemesClosed != null) query = query.Where(u => u.IsThemaClosed == isThemesClosed.Value);
            if (date != null)
            {
                DateTime from = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
                DateTime to = from.AddDays(1);
                query = query.Where(u => from <= u.DateTime && u.DateTime <= to);
            }

            ViewBag.IsThemesClosed = isThemesClosed;
            ViewBag.Date = date;
            return View(await query.ToListAsync());
        }

        // GET: ClientCallBacks/Details/5
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
