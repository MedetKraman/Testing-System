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
    public class TestCategoryTestThemasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestCategoryTestThemas
        public async Task<ActionResult> Index(int testThemaId)
        {
            var testCategoryTestThemas = db.TestCategoryTestThemas.Where(u => u.TestThemaId == testThemaId).Include(t => t.TestCategory);

            ViewBag.TestThema = db.TestThemas.Find(testThemaId).Thema;
            ViewBag.TestThemaId = testThemaId;
            return View(await testCategoryTestThemas.ToListAsync());
        }

        // GET: TestCategoryTestThemas/Create
        public ActionResult Create(int testThemaId)
        {
            ViewBag.TestCategoryId = new SelectList(db.TestCategories, "Id", "Category");
            ViewBag.TestThema = db.TestThemas.Find(testThemaId).Thema;
            return View(new TestCategoryTestThema { TestThemaId = testThemaId });
        }

        // POST: TestCategoryTestThemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,TestCategoryId,TestThemaId")] TestCategoryTestThema testCategoryTestThema)
        {
            if (ModelState.IsValid)
            {
                db.TestCategoryTestThemas.Add(testCategoryTestThema);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", routeValues: new { testThemaId = testCategoryTestThema.TestThemaId });
            }

            ViewBag.TestCategoryId = new SelectList(db.TestCategories, "Id", "Category", testCategoryTestThema.TestCategoryId);
            ViewBag.TestThema = db.TestThemas.Find(testCategoryTestThema.TestThemaId).Thema;
            return View(testCategoryTestThema);
        }
        
        // GET: TestCategoryTestThemas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCategoryTestThema testCategoryTestThema = await db.TestCategoryTestThemas.FindAsync(id);
            if (testCategoryTestThema == null)
            {
                return HttpNotFound();
            }
            ViewBag.TestThema = db.TestThemas.Find(testCategoryTestThema.TestThemaId).Thema;
            ViewBag.Category = db.TestCategories.Find(testCategoryTestThema.TestCategoryId).Category;
            return View(testCategoryTestThema);
        }

        // POST: TestCategoryTestThemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TestCategoryTestThema testCategoryTestThema = await db.TestCategoryTestThemas.FindAsync(id);
            int testThemaId = testCategoryTestThema.TestThemaId;
            db.TestCategoryTestThemas.Remove(testCategoryTestThema);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", routeValues: new { testThemaId });
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
