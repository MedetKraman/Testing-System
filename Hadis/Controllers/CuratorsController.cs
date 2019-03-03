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
    public class CuratorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Curators
        public async Task<ActionResult> Index(string viewMessage = null)
        {
            ViewBag.ViewMessage = viewMessage;
            return View(await db.Curators.ToListAsync());
        }

        // GET: Curators/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Curators/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Curator curator, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                using(var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Curators.Add(curator);
                        await db.SaveChangesAsync();

                        if (!Directory.Exists(Server.MapPath("~/Images")))
                            Directory.CreateDirectory(Server.MapPath("~/Images"));

                        if(avatarFile != null)
                        {
                            curator.AvatarUrl = "~/Images/curator-" + curator.Id + avatarFile.FileName.Split('.').Last();
                            avatarFile.SaveAs(Server.MapPath(curator.AvatarUrl));
                            db.Entry(curator).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        transaction.Rollback();
                        return RedirectToAction("Index", new { viewMessage = "Ошибка при сохранении" });
                    }
                }
            }

            return View(curator);
        }

        // GET: Curators/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curator curator = await db.Curators.FindAsync(id);
            if (curator == null)
            {
                return HttpNotFound();
            }
            return View(curator);
        }

        // POST: Curators/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Curator curator, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                using(var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(curator).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        if (avatarFile != null)
                        {
                            if (!Directory.Exists(Server.MapPath("~/Images")))
                                Directory.CreateDirectory(Server.MapPath("~/Images"));

                            if (string.IsNullOrWhiteSpace(curator.AvatarUrl) == false && System.IO.File.Exists(Server.MapPath(curator.AvatarUrl)))
                            {
                                System.IO.File.Delete(curator.AvatarUrl);
                            }
                            curator.AvatarUrl = "~/Images/curator-" + curator.Id + avatarFile.FileName.Split('.').Last();
                            avatarFile.SaveAs(Server.MapPath(curator.AvatarUrl));
                            db.Entry(avatarFile).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                        transaction.Commit();
                        return RedirectToAction("Index");
                    } catch
                    {
                        transaction.Rollback();
                        return RedirectToAction("Index", routeValues: new { viewMessage = "Ошибка при сохранении изменении. Изменение не сохранено!!!" });
                    }
                }
            }
            return View(curator);
        }

        // GET: Curators/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curator curator = await db.Curators.FindAsync(id);
            if (curator == null)
            {
                return HttpNotFound();
            }
            return View(curator);
        }

        // POST: Curators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Curator curator = await db.Curators.FindAsync(id);
                    db.Curators.Remove(curator);
                    await db.SaveChangesAsync();

                    if (String.IsNullOrWhiteSpace(curator.AvatarUrl) == false && System.IO.File.Exists(Server.MapPath(curator.AvatarUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(curator.AvatarUrl));
                    }

                    transaction.Commit();
                    return RedirectToAction("Index");
                } catch
                {
                    transaction.Rollback();
                    return RedirectToAction("Index", routeValues: new { viewMessage = "Ошибка при удалении" });
                }
            }
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
