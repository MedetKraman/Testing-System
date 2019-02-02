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
    public class ClientTestQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerArea/ClientTestQuestions
        public async Task<ActionResult> Index()
        {
            var clientTestQuestions = db.ClientTestQuestions.Include(c => c.ClientTestHistory).Include(c => c.TestQuestion);
            return View(await clientTestQuestions.ToListAsync());
        }

        // GET: CustomerArea/ClientTestQuestions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTestQuestion clientTestQuestion = await db.ClientTestQuestions.FindAsync(id);
            if (clientTestQuestion == null)
            {
                return HttpNotFound();
            }
            return View(clientTestQuestion);
        }

        // GET: CustomerArea/ClientTestQuestions/Create
        public async Task<ActionResult> Create(int clientTestHistoryId, bool isFirst = false)
        {
            var temp =
                await (from ctq in db.ClientTestQuestions
                       where ctq.ClientTestHistoryId == clientTestHistoryId
                       select new
                       {
                           ctq.TestQuestionId
                       }).ToListAsync();
            var testQuestions =
                await (from cth in db.ClientTestHistories
                       from tq in db.TestQuestions

                       where cth.Id == clientTestHistoryId
                       where tq.TestThemaId == cth.TestThemaId

                       select tq).ToListAsync();
            TestQuestion testQuestion = null;
            foreach (var tq in testQuestions)
            {
                if (temp.Where(u => u.TestQuestionId == tq.Id).Count() == 0)
                {
                    testQuestion = tq;
                    break;
                }
            }
            if (isFirst)
                testQuestion =
                    await (from cth in db.ClientTestHistories
                           from tq in db.TestQuestions
                           where cth.Id == clientTestHistoryId
                           where cth.TestThemaId == tq.TestThemaId
                           select tq).FirstAsync();

            if (testQuestion == null)
                return RedirectToAction("CreateFinish", routeValues: new { controller = "ClientTestHistories", clientTestHistoryId });


            ViewBag.TestQuestion = await db.TestQuestions.Include(u => u.TestThema).Include(u => u.TestAnswers).Where(u => u.Id == testQuestion.Id).FirstAsync();
            return View(new ClientTestQuestion { TestQuestionId = testQuestion.Id, ClientTestHistoryId = clientTestHistoryId });
        }

        // POST: CustomerArea/ClientTestQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClientTestQuestion clientTestQuestion, int[] ClientSelectedAnswersId)
        {
            db.ClientTestQuestions.Add(clientTestQuestion);
            await db.SaveChangesAsync();
            for (int i = 0; i < ClientSelectedAnswersId.Length; i++)
            {
                if (ClientSelectedAnswersId[i] != 0)
                {
                    db.ClientSelectedAnswers.Add(new ClientSelectedAnswer
                    {
                        ClientTestQuestionId = clientTestQuestion.Id,
                        TestAnswerId = ClientSelectedAnswersId[i]
                    });
                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Create", routeValues: new { clientTestHistoryId = clientTestQuestion.ClientTestHistoryId });
        }

        // GET: CustomerArea/ClientTestQuestions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTestQuestion clientTestQuestion = await db.ClientTestQuestions.FindAsync(id);
            if (clientTestQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientTestHistoryId = new SelectList(db.ClientTestHistories, "Id", "Comment", clientTestQuestion.ClientTestHistoryId);
            ViewBag.TestQuestionId = new SelectList(db.TestQuestions, "Id", "Question", clientTestQuestion.TestQuestionId);
            return View(clientTestQuestion);
        }

        // POST: CustomerArea/ClientTestQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Comment,ClientTestHistoryId,TestQuestionId")] ClientTestQuestion clientTestQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientTestQuestion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTestHistoryId = new SelectList(db.ClientTestHistories, "Id", "Comment", clientTestQuestion.ClientTestHistoryId);
            ViewBag.TestQuestionId = new SelectList(db.TestQuestions, "Id", "Question", clientTestQuestion.TestQuestionId);
            return View(clientTestQuestion);
        }

        // GET: CustomerArea/ClientTestQuestions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTestQuestion clientTestQuestion = await db.ClientTestQuestions.FindAsync(id);
            if (clientTestQuestion == null)
            {
                return HttpNotFound();
            }
            return View(clientTestQuestion);
        }

        // POST: CustomerArea/ClientTestQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ClientTestQuestion clientTestQuestion = await db.ClientTestQuestions.FindAsync(id);
            db.ClientTestQuestions.Remove(clientTestQuestion);
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
