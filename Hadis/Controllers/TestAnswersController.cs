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
    public class TestAnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestAnswers
        public ActionResult Index(int testQuestionId)
        {
            var testAnswers = db.TestAnswers.Where(u => u.TestQuestionId == testQuestionId).Include(t => t.TestQuestion);
            return View( testAnswers.ToList());
        }

        // GET: TestAnswers/Create
        public ActionResult Create(int testThemaId, int testQuestionId, int answerCount, int questionCount, int index)
        {
            ViewBag.TestThemaId = testThemaId;
            ViewBag.TestQuestionId = testQuestionId;
            ViewBag.AnswerCount = answerCount;
            ViewBag.QuestionCount = questionCount;
            ViewBag.Index = index;
            ViewBag.Question = db.TestQuestions.Find(testQuestionId).Question;

            var model = new List<TestAnswer>();
            for (int i = 0; i < answerCount; i ++)
            {
                model.Add(new TestAnswer { ShareWeight = 1 });
            }

            return View(model);
        }

        // POST: TestAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(List<TestAnswer> model, int testThemaId, int questionCount, int index, int answerCount)
        {
            if (ModelState.IsValid)
            {
                db.TestAnswers.AddRange(model);
                await db.SaveChangesAsync();
                index++;
                return RedirectToAction("Create", routeValues: new { controller = "TestQuestions", testThemaId, questionCount, index });
            }

            ViewBag.TestQuestionId = model.First().TestQuestionId;
            ViewBag.AnswerCount = answerCount;
            ViewBag.QuestionCount = questionCount;
            ViewBag.Index = index;
            return View(model);
        }

        // GET: TestAnswers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestAnswer testAnswer = await db.TestAnswers.FindAsync(id);
            if (testAnswer == null)
            {
                return HttpNotFound();
            }
            var ques = db.TestQuestions.Find(testAnswer.TestQuestionId);
            ViewBag.TestThemaId = ques.TestThemaId;
            return View(testAnswer);
        }

        // POST: TestAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Answer,Description,ShareWeight,IsCurrect,TestQuestionId")] TestAnswer testAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testAnswer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var ques = db.TestQuestions.Find(testAnswer.TestQuestionId);
                return RedirectToAction("Update", routeValues: new { controller = "TestThemas", id = ques.TestThemaId });
            }
            var ques1 = db.TestQuestions.Find(testAnswer.TestQuestionId);
            ViewBag.TestThemaId = ques1.TestThemaId;
            return View(testAnswer);
        }

        // GET: TestAnswers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestAnswer testAnswer = await db.TestAnswers.FindAsync(id);
            if (testAnswer == null)
            {
                return HttpNotFound();
            }
            var ques1 = db.TestQuestions.Find(testAnswer.TestQuestionId);
            ViewBag.TestThemaId = ques1.TestThemaId;
            return View(testAnswer);
        }

        // POST: TestAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TestAnswer testAnswer = await db.TestAnswers.FindAsync(id);
            db.TestAnswers.Remove(testAnswer);
            await db.SaveChangesAsync();
            var ques = db.TestQuestions.Find(testAnswer.TestQuestionId);
            return RedirectToAction("Update", routeValues: new { controller = "TestThemas", id = ques.TestThemaId });
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
