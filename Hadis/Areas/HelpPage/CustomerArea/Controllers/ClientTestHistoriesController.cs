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
    public class ClientTestHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerArea/ClientTestHistories
        public async Task<ActionResult> Index()
        {
            string clientId = db.Users.Where(u => u.UserName == User.Identity.Name).Single().Id;
            var clientTestHistories = db.ClientTestHistories.Include(c => c.Client).Include(c => c.TestThema).Where(u => u.ClientId == clientId);
            return View(await clientTestHistories.ToListAsync());
        }

        // GET: CustomerArea/ClientTestHistories/Details/5
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

        // GET: CustomerArea/ClientTestHistories/Create
        public async Task<ActionResult> Create(int id)
        {
            double totalPoint = db.TestThemas.Find(id).TotalPoint;
            string clientId = db.Users.Where(u => u.UserName == User.Identity.Name).Single().Id;
            ClientTestHistory clientTestHistory = new ClientTestHistory
            {
                TestThemaId = id,
                ClientId = clientId,
                Date = DateTime.Now,
                Point = 0,
                TotalPoint = totalPoint
            };
            db.ClientTestHistories.Add(clientTestHistory);
            await db.SaveChangesAsync();

            ViewBag.Thema = db.TestThemas.Find(id).Thema;
            ViewBag.ClientTestHistoryId = clientTestHistory.Id;
            return RedirectToAction("Create", routeValues: new { controller = "ClientTestQuestions", clientTestHistoryId = clientTestHistory.Id, isFirst = true });
        }
        
        public ActionResult CreateFinish(int clientTestHistoryId)
        {
            ViewBag.ClientTestHistoryId = clientTestHistoryId;
            ViewBag.Thema = db.ClientTestHistories.Find(clientTestHistoryId).TestThema.Thema;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFinish(int clientTestHistoryId, string comment)
        {
            ClientTestHistory clientTestHistory = db.ClientTestHistories.Find(clientTestHistoryId);
            clientTestHistory.Comment = comment;
            db.Entry(clientTestHistory).State = EntityState.Modified;
            await db.SaveChangesAsync();

            // Баллын есептеу

            double totalPoint = clientTestHistory.TotalPoint;
            double point = 0;
            List<TestQuestion> testQuestions = await db.TestQuestions.Include(u => u.TestAnswers).Where(u => u.TestThemaId == clientTestHistory.TestThemaId).ToListAsync();
            double totalWeight = testQuestions.Select(u => u.ShareWeight).Sum();
            List<ClientTestQuestion> clientTestQuestions = await db.ClientTestQuestions.Include(u => u.ClientSelectedAnswers).Where(u => u.ClientTestHistoryId == clientTestHistoryId).ToListAsync();
            foreach (var clQues in clientTestQuestions)
            {
                TestQuestion testQuestion = testQuestions.Where(u => u.Id == clQues.TestQuestionId).First();
                double quesTotalPoint = testQuestion.ShareWeight / totalWeight * totalPoint;
                double curAnsTotalWeight = testQuestion.TestAnswers.Where(u => u.IsCurrect).Select(u => u.ShareWeight).Sum();
                double curSelectTotalPoint = 0;
                double notCurSelectTotalPoint = 0;
                foreach (var selectAns in clQues.ClientSelectedAnswers)
                {
                    TestAnswer testAnswer = testQuestion.TestAnswers.Where(u => u.Id == selectAns.TestAnswerId).First();
                    if (testAnswer.IsCurrect)
                    {
                        curSelectTotalPoint += testAnswer.ShareWeight;
                    }else
                    {
                        notCurSelectTotalPoint += testAnswer.ShareWeight;
                    }
                }

                double quesPoint = quesTotalPoint * curSelectTotalPoint / curAnsTotalWeight;
                quesPoint -= quesTotalPoint * notCurSelectTotalPoint / curAnsTotalWeight;
                point += quesPoint;
            }
            clientTestHistory.Point = point;
            db.Entry(clientTestHistory).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction("Details", routeValues: new { id = clientTestHistoryId });
        }

        // GET: CustomerArea/ClientTestHistories/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Id", clientTestHistory.ClientId);
            ViewBag.TestThemaId = new SelectList(db.TestThemas, "Id", "Thema", clientTestHistory.TestThemaId);
            return View(clientTestHistory);
        }

        // POST: CustomerArea/ClientTestHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Date,Point,TotalPoint,Comment,TestThemaId,ClientId")] ClientTestHistory clientTestHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientTestHistory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Id", clientTestHistory.ClientId);
            ViewBag.TestThemaId = new SelectList(db.TestThemas, "Id", "Thema", clientTestHistory.TestThemaId);
            return View(clientTestHistory);
        }

        // GET: CustomerArea/ClientTestHistories/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(clientTestHistory);
        }

        // POST: CustomerArea/ClientTestHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ClientTestHistory clientTestHistory = await db.ClientTestHistories.FindAsync(id);
            db.ClientTestHistories.Remove(clientTestHistory);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
