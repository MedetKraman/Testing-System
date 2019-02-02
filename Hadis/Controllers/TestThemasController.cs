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
    public class TestThemasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestThemas
        public async Task<ActionResult> Index()
        {
            var testThemas = db.TestThemas;
            return View(await testThemas.ToListAsync());
        }

        // GET: TestThemas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestThema testThema = db.TestThemas.Find(id);
            testThema.TestQuestions = db.TestQuestions.Where(u => u.TestThemaId == id).Include(u => u.TestAnswers).ToList();
            if (testThema == null)
            {
                return HttpNotFound();
            }
            return View(testThema);
        }

        // GET: TestThemas/Create
        public ActionResult Create()
        {
            return View(new TestThema { TotalPoint = 100 });
        }

        // POST: TestThemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Thema,Description,CreatedDateTime,IsActual,TotalPoint")] TestThema testThema, int questionCount)
        {
            if (ModelState.IsValid)
            {
                db.TestThemas.Add(testThema);
                await db.SaveChangesAsync();
                return RedirectToAction("Create", routeValues: new { controller = "TestQuestions", testThemaId = testThema.Id, questionCount });
            }

            return View(testThema);
        }

        // GET: TestThemas/Edit/5
        public ActionResult Update(int? id, bool createNew = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestThema testThema = db.TestThemas.Find(id);
            if (testThema == null)
            {
                return HttpNotFound();
            }
            if (createNew)
            {
                var model = new TestThema
                {
                    CreatedDateTime = DateTime.Now,
                    Description = testThema.Description,
                    IsActual = testThema.IsActual,
                    Thema = testThema.Thema,
                    TotalPoint = testThema.TotalPoint
                };
                testThema.TestQuestions = db.TestQuestions.Where(u => u.TestThemaId == id).Include(u => u.TestAnswers).ToList();
                model.TestQuestions = new List<TestQuestion>();
                foreach (var ques in testThema.TestQuestions)
                {
                    model.TestQuestions.Add(new TestQuestion
                    {
                        Description = ques.Description,
                        Question = ques.Question,
                        ShareWeight = ques.ShareWeight,
                        TestAnswers = new List<TestAnswer>()
                    });
                    foreach (var ans in ques.TestAnswers)
                    {
                        model.TestQuestions.Last().TestAnswers.Add(new TestAnswer
                        {
                            ShareWeight = ans.ShareWeight,
                            IsCurrect = ans.IsCurrect,
                            Description = ans.Description,
                            Answer = ans.Answer
                        });
                    }
                }
                testThema.TestCategoryTestThemas = db.TestCategoryTestThemas.Where(u => u.TestThemaId == id).ToList();
                model.TestCategoryTestThemas = new List<TestCategoryTestThema>();
                foreach (var item in testThema.TestCategoryTestThemas)
                {
                    model.TestCategoryTestThemas.Add(new TestCategoryTestThema
                    {
                        TestCategoryId = item.TestCategoryId
                    });
                }
                db.TestThemas.Add(model);
                db.SaveChanges();
                db.TestThemaVersions.Add(new TestThemaVersion { Id = model.Id, ParentTestThemaId = id });
                db.SaveChanges();
            }
            return View(testThema);
        }

        // POST: TestThemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([Bind(Include = "Id,Thema,Description,CreatedDateTime,IsActual,TotalPoint")] TestThema testThema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testThema).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(testThema);
        }

        // GET: TestThemas/NotRelevant/5
        public async Task<ActionResult> NotRelevant(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestThema testThema = await db.TestThemas.FindAsync(id);
            if (testThema == null)
            {
                return HttpNotFound();
            }
            return View(testThema);
        }

        // POST: TestThemas/NotRelevant/5
        [HttpPost, ActionName("NotRelevant")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NotRelevantConfirmed(int id)
        {
            TestThema testThema = await db.TestThemas.FindAsync(id);
            testThema.IsActual = false;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: TestThemas/Relevant/5
        public async Task<ActionResult> Relevant(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestThema testThema = await db.TestThemas.FindAsync(id);
            if (testThema == null)
            {
                return HttpNotFound();
            }
            return View(testThema);
        }

        // POST: TestThemas/Relevant/5
        [HttpPost, ActionName("Relevant")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RelevantConfirmed(int id)
        {
            TestThema testThema = await db.TestThemas.FindAsync(id);
            testThema.IsActual = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: TestThemas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestThema testThema = await db.TestThemas.FindAsync(id);
            if (testThema == null)
            {
                return HttpNotFound();
            }
            return View(testThema);
        }

        // POST: TestThemas/Relevant/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TestThema testThema = await db.TestThemas.FindAsync(id);
            TestThemaVersion testThemaVersion = db.TestThemaVersions.Find(id);
            db.TestThemaVersions.Remove(testThemaVersion);
            db.SaveChanges();
            db.TestThemas.Remove(testThema);
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
