using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Fora;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class ForaController : Controller
    {
        private JCarrollOnlineV2DbContext _data { get; set; }

        public ForaController()
        {
            _data = new JCarrollOnlineV2DbContext();
        }

        // GET: Fora
        public async Task<ActionResult> Index()
        {
            ForaIndexViewModel foraIndexViewModel = new ForaIndexViewModel
            {
                ForaIndexItems = new List<ForaIndexItemViewModel>()
            };

            List<Forum> fora = await _data.Forum.ToListAsync();

            foreach(Forum forum in fora)
            {
                ForaIndexItemViewModel foraIndexItemViewModel = new ForaIndexItemViewModel();

                foraIndexItemViewModel.InjectFrom(forum);
                foraIndexItemViewModel.ThreadCount = await ControllerHelpers.GetThreadCountAsync(forum, _data);

                if (foraIndexItemViewModel.ThreadCount > 0)
                {
                    foraIndexItemViewModel.LastThread = await ControllerHelpers.GetLatestThreadDataAsync(forum, _data);
                }

                foraIndexViewModel.ForaIndexItems.Add(foraIndexItemViewModel);
            }

            return View(foraIndexViewModel);
        }

        // GET: Fora/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Forum forum = await _data.Forum.FindAsync(id);

            if (forum == null)
            {
                return HttpNotFound();
            }

            return View(forum);
        }

        // GET: Fora/Create
        [Authorize(Roles ="Administrator")]
        public ActionResult Create()
        {
            ForaCreateViewModel foraCreateViewModel = new ForaCreateViewModel();

            return View(foraCreateViewModel);
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
                _data.Forum.Add(forum);
                await _data.SaveChangesAsync();

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

            Forum forum = await _data.Forum.FindAsync(id);
            ForumEditViewModel forumEditViewModel = new ForumEditViewModel();

            forumEditViewModel.InjectFrom(forum);

            if (forum == null)
            {
                return HttpNotFound();
            }

            return View(forumEditViewModel);
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
                _data.Entry(forum).State = EntityState.Modified;
                await _data.SaveChangesAsync();

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

            Forum forum = await _data.Forum.FindAsync(id);
            ForumDeleteViewModel forumDeleteViewModel = new ForumDeleteViewModel();

            forumDeleteViewModel.InjectFrom(forum);

            if (forum == null)
            {
                return HttpNotFound();
            }

            return View(forumDeleteViewModel);
        }

        // POST: Fora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Forum forum = await _data.Forum.FindAsync(id);

            _data.Forum.Remove(forum);
            await _data.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}
