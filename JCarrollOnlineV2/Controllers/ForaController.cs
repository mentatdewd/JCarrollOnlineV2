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
using Omu.ValueInjecter;
using JCarrollOnlineV2.ControllerHelpers;

namespace JCarrollOnlineV2.Controllers
{
    public class ForaController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

        // GET: Fora
        public ActionResult Index()
        {
            ForaIndexViewModel fvm = new ForaIndexViewModel();
            fvm.ForaIndexItems = new List<ForaIndexItemViewModel>();

            var dlist = db.Forums.ToList();
            foreach(var item in dlist)
            {
                var fitem = new ForaIndexItemViewModel();
                fitem.InjectFrom<FilterId>(item);
                fitem.ThreadCount = ControllerHelpers.Forums.GetThreadCount(fitem.ForumId);
                if(fitem.ThreadCount > 0)
                    fitem.LastThread = ControllerHelpers.Forums.GetLatestThreadData(fitem.ForumId);

                fvm.ForaIndexItems.Add(fitem);
            }
            return View(fvm);
        }

        // GET: Fora/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = db.Forums.Where(f => f.ForumId == id).Include(f => f.ForumThreads).FirstOrDefault();
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }

        // GET: Fora/Create
        [Authorize]
        public ActionResult Create()
        {
            ForaCreateViewModel vm = new ForaCreateViewModel();
            return View(vm);
        }

        // POST: Fora/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Title,Description,CreatedAt,UpdatedAt")] ForaCreateViewModel forumViewModel)
        {
            if (ModelState.IsValid)
            {
                Forum forum = new Forum();
                forum.InjectFrom(forumViewModel);
                forum.CreatedAt = DateTime.Now;
                forum.UpdatedAt = DateTime.Now;
                db.Forums.Add(forum);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(forumViewModel);
        }

        // GET: Fora/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = await db.Forums.FindAsync(id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }

        // POST: Fora/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,CreatedAt,UpdatedAt")] Forum forum)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forum).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(forum);
        }

        // GET: Fora/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = await db.Forums.FindAsync(id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }

        // POST: Fora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Forum forum = await db.Forums.FindAsync(id);
            db.Forums.Remove(forum);
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
