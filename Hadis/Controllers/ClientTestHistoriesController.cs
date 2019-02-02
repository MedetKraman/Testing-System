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
    public class ClientTestHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientTestHistories
        public async Task<ActionResult> Index()
        {
            var clientTestHistories = db.ClientTestHistories.Include(c => c.Client.User).Include(c => c.TestThema);
            return View(await clientTestHistories.ToListAsync());
        }

        // GET: ClientTestHistories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTestHistory clientTestHistory = await db.ClientTestHistories.FindAsync(id);
            if (clientTestHistory == null)
            {
                return HttpNotFound();
            }
            clientTestHistory.TestThema = await db.TestThemas.FindAsync(clientTestHistory.TestThemaId);
            clientTestHistory.ClientTestQuestions = await db.ClientTestQuestions.Include(u => u.TestQuestion).Include(u => u.ClientSelectedAnswers).Where(u => u.ClientTestHistoryId == id).ToListAsync();
            foreach (var ques in clientTestHistory.ClientTestQuestions)
            {
                foreach (var ans in ques.ClientSelectedAnswers)
                {
                    ans.TestAnswer = await db.TestAnswers.FindAsync(ans.TestAnswerId);
                }
            }
            return View(clientTestHistory);
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
