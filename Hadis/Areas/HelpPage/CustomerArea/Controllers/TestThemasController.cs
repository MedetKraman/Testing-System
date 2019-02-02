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
    public class TestThemasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerArea/TestThemas
        public async Task<ActionResult> Index()
        {
            var testThemas = db.TestThemas.Where(u => u.IsActual == true).Include(t => t.TestThemaVersion);
            return View(await testThemas.ToListAsync());
        }

        // GET: CustomerArea/TestThemas/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: CustomerArea/TestThemas/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.TestThemaVersions, "Id", "Description");
            return View();
        }

        // POST: CustomerArea/TestThemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Thema,Description,CreatedDateTime,IsActual,TotalPoint")] TestThema testThema)
        {
            if (ModelState.IsValid)
            {
                db.TestThemas.Add(testThema);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.TestThemaVersions, "Id", "Description", testThema.Id);
            return View(testThema);
        }

        // GET: CustomerArea/TestThemas/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.Id = new SelectList(db.TestThemaVersions, "Id", "Description", testThema.Id);
            return View(testThema);
        }

        // POST: CustomerArea/TestThemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Thema,Description,CreatedDateTime,IsActual,TotalPoint")] TestThema testThema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testThema).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.TestThemaVersions, "Id", "Description", testThema.Id);
            return View(testThema);
        }

        // GET: CustomerArea/TestThemas/Delete/5
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

        // POST: CustomerArea/TestThemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TestThema testThema = await db.TestThemas.FindAsync(id);
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
