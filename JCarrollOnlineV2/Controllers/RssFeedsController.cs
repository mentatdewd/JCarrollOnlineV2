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
    public class RssFeedsController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

        // GET: RssFeeds
        public async Task<ActionResult> Index()
        {
            return View(await db.RssFeedEntries.ToListAsync());
        }

        // GET: RssFeeds/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssFeed = await db.RssFeedEntries.FindAsync(id);
            if (rssFeed == null)
            {
                return HttpNotFound();
            }
            return View(rssFeed);
        }

        // GET: RssFeeds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RssFeeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Summary,Url,PublishedAt,Guid,CreatedAt,UpdatedAt")] RssFeed rssFeed)
        {
            if (ModelState.IsValid)
            {
                db.RssFeedEntries.Add(rssFeed);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(rssFeed);
        }

        // GET: RssFeeds/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssFeed = await db.RssFeedEntries.FindAsync(id);
            if (rssFeed == null)
            {
                return HttpNotFound();
            }
            return View(rssFeed);
        }

        // POST: RssFeeds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Summary,Url,PublishedAt,Guid,CreatedAt,UpdatedAt")] RssFeed rssFeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rssFeed).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(rssFeed);
        }

        // GET: RssFeeds/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssFeed = await db.RssFeedEntries.FindAsync(id);
            if (rssFeed == null)
            {
                return HttpNotFound();
            }
            return View(rssFeed);
        }

        // POST: RssFeeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RssFeed rssFeed = await db.RssFeedEntries.FindAsync(id);
            db.RssFeedEntries.Remove(rssFeed);
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
