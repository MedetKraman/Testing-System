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
    public class TestQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestQuestions
        public ActionResult Index(int testThemaId)
        {
            var testQuestions = db.TestQuestions.Where(u => u.TestThemaId == testThemaId);
            return PartialView(testQuestions.ToList());
        }

        // GET: TestQuestions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestQuestion testQuestion = await db.TestQuestions.FindAsync(id);
            if (testQuestion == null)
            {
                return HttpNotFound();
            }
            return View(testQuestion);
        }

        // GET: TestQuestions/Create
        public ActionResult Create(int testThemaId, int questionCount, int index = 1)
        {
            if (index > questionCount)
                return RedirectToAction("Index", new { controller = "TestThemas" });

            ViewBag.TestThemaId = testThemaId;
            ViewBag.QuestionCount = questionCount;
            ViewBag.Index = index;
            return View(new TestQuestion { ShareWeight = 1 });
        }

        // POST: TestQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Question,Description,ShareWeight,TestThemaId")] TestQuestion testQuestion, int answerCount, int questionCount, int index)
        {
            if (ModelState.IsValid)
            {
                db.TestQuestions.Add(testQuestion);
                await db.SaveChangesAsync();
                return RedirectToAction("Create", routeValues: 
                    new { controller = "TestAnswers", testThemaId = testQuestion.TestThemaId, testQuestionId = testQuestion.Id, answerCount, questionCount, index });
            }

            ViewBag.TestThemaId = testQuestion.TestThemaId;
            ViewBag.QuestionCount = questionCount;
            ViewBag.Index = index;
            return View(testQuestion);
        }

        // GET: TestQuestions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestQuestion testQuestion = await db.TestQuestions.FindAsync(id);
            if (testQuestion == null)
            {
                return HttpNotFound();
            }
            return View(testQuestion);
        }

        // POST: TestQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Question,Description,ShareWeight,TestThemaId")] TestQuestion testQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testQuestion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Update", routeValues: new { controller = "TestThemas", id = testQuestion.TestThemaId });
            }
            return View(testQuestion);
        }

        // GET: TestQuestions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestQuestion testQuestion = await db.TestQuestions.FindAsync(id);
            if (testQuestion == null)
            {
                return HttpNotFound();
            }
            return View(testQuestion);
        }

        // POST: TestQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TestQuestion testQuestion = await db.TestQuestions.FindAsync(id);
            int testThemaId = testQuestion.TestThemaId;
            db.TestQuestions.Remove(testQuestion);
            await db.SaveChangesAsync();
            return RedirectToAction("Update", routeValues: new { controller = "TestThemas", id = testQuestion.TestThemaId });
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
