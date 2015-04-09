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
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using JCarrollOnlineV2.LinqToSqlExtensions;

namespace JCarrollOnlineV2.Controllers
{
    public class ForumThreadEntriesController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

        // GET: ForumOriginalPost
        public async Task<ActionResult> Index(int forumId, string forumTitle)
        {
            //retreive all root Person objects from db   
            //ViewBag.ForumId = forumId;
            ViewBag.ForumTitle = forumTitle;
            var forumThreadEntries = await db.ForumThreadEntries.Where(p => p.ForumId == forumId && p.ParentId == null).ToListAsync();
            return View(forumThreadEntries);
        }

        // GET: ForumOriginalPost/Details/5
        public ActionResult Details(string forumTitle, int threadId)
        {
            ViewBag.ForumTitle = forumTitle;
            var threadEntries = db.ForumThreadEntries.AsHierarchy("ForumThreadEntryId", "ParentId", threadId, 10);
            return View(threadEntries);
        }

        // GET: ForumOriginalPost/Create
        public ActionResult Create(int forumId, int parentId)
        {
           // ForumThread forumThreadEntry = new ForumThread();

            ViewBag.forumId = forumId;
            ViewBag.parent = parentId;

            return View();
        }

        // POST: ForumOriginalPost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content,AuthorId,Locked,ForumId,CreatedAt,UpdatedAt,PostNumber")]  ForumThreadEntry forumThreadEntry)
        {
            if (ModelState.IsValid)
            {
                forumThreadEntry.CreatedAt = DateTime.Now;
                forumThreadEntry.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                forumThreadEntry.AuthorId = currentUser.Id;
                db.ForumThreadEntries.Add(forumThreadEntry);

                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { forumId = forumThreadEntry.ForumId });
            }

            return View(forumThreadEntry);
        }

        // GET: ForumOriginalPost/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThreadEntry forumThread = await db.ForumThreadEntries.FindAsync(id);
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            return View(forumThread);
        }

        // POST: ForumOriginalPost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,AuthorId,Locked,ForumId,CreatedAt,UpdatedAt,PostNumber")] ForumThreadEntry forumThreadEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forumThreadEntry).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(forumThreadEntry);
        }

        // GET: ForumOriginalPost/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThreadEntry forumThread = await db.ForumThreadEntries.FindAsync(id);
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            return View(forumThread);
        }

        // POST: ForumOriginalPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ForumThreadEntry forumThread = await db.ForumThreadEntries.FindAsync(id);
            db.ForumThreadEntries.Remove(forumThread);
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
