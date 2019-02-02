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
        public async Task<ActionResult> Index()
        {
            var clientCallBacks = db.ClientCallBacks.Include(c => c.Client.User);
            return View(await clientCallBacks.ToListAsync());
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
