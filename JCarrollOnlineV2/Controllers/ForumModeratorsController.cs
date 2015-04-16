using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize(Roles="Administrators")]
    public class ForumModeratorsController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

        // GET: ForumModerators
        public async Task<ActionResult> Index()
        {
            return View(await db.ForumModerators.ToListAsync());
        }

        // GET: ForumModerators/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumModerator forumModerator = await db.ForumModerators.FindAsync(id);
            if (forumModerator == null)
            {
                return HttpNotFound();
            }
            return View(forumModerator);
        }

        // GET: ForumModerators/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ForumModerators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ForumId,CreatedAt,UpdatedAt")] ForumModerator forumModerator)
        {
            if (ModelState.IsValid)
            {
                db.ForumModerators.Add(forumModerator);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(forumModerator);
        }

        // GET: ForumModerators/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumModerator forumModerator = await db.ForumModerators.FindAsync(id);
            if (forumModerator == null)
            {
                return HttpNotFound();
            }
            return View(forumModerator);
        }

        // POST: ForumModerators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ForumId,CreatedAt,UpdatedAt")] ForumModerator forumModerator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forumModerator).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(forumModerator);
        }

        // GET: ForumModerators/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumModerator forumModerator = await db.ForumModerators.FindAsync(id);
            if (forumModerator == null)
            {
                return HttpNotFound();
            }
            return View(forumModerator);
        }

        // POST: ForumModerators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ForumModerator forumModerator = await db.ForumModerators.FindAsync(id);
            db.ForumModerators.Remove(forumModerator);
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
