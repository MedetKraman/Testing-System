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
    public class TestCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestCategories
        public async Task<ActionResult> Index()
        {
            return View(await db.TestCategories.ToListAsync());
        }

        // GET: TestCategories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCategory testCategory = await db.TestCategories.FindAsync(id);
            if (testCategory == null)
            {
                return HttpNotFound();
            }
            return View(testCategory);
        }

        // GET: TestCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Category")] TestCategory testCategory)
        {
            if (ModelState.IsValid)
            {
                db.TestCategories.Add(testCategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(testCategory);
        }

        // GET: TestCategories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCategory testCategory = await db.TestCategories.FindAsync(id);
            if (testCategory == null)
            {
                return HttpNotFound();
            }
            return View(testCategory);
        }

        // POST: TestCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Category")] TestCategory testCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(testCategory);
        }

        // GET: TestCategories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCategory testCategory = await db.TestCategories.FindAsync(id);
            if (testCategory == null)
            {
                return HttpNotFound();
            }
            return View(testCategory);
        }

        // POST: TestCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TestCategory testCategory = await db.TestCategories.FindAsync(id);
            db.TestCategories.Remove(testCategory);
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
