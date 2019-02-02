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
using System.IO;

namespace Hadis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VideosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Videos
        public async Task<ActionResult> Index()
        {
            return View(await db.Videos.ToListAsync());
        }

        // GET: Videos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // GET: Videos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Videos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Video video, HttpPostedFileBase videofail)
        {
            if (videofail == null) return View();
            if (ModelState.IsValid)
            {
                video.Type = videofail.ContentType;
                db.Videos.Add(video);
                await db.SaveChangesAsync();
                video.Url = video.Id + "." + videofail.FileName.Split('.').Last();
                string path = Path.Combine(Server.MapPath("~/VideoFiles"), video.Id + "." + videofail.FileName.Split('.').Last());
                videofail.SaveAs(path);
                db.Entry(video).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(video);
        }

        // GET: Videos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Videos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Uploader,Title,Description,Url,Type")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Entry(video).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(video);
        }

        // GET: Videos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Video video = await db.Videos.FindAsync(id);
            db.Videos.Remove(video);
            await db.SaveChangesAsync();
            if (System.IO.File.Exists(Server.MapPath("~/VideoFiles/" + video.Url)))
            {
                System.IO.File.Delete(Server.MapPath("~/VideoFiles/" + video.Url));
            }
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
