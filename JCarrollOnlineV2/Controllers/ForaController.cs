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
        private readonly JCarrollOnlineV2DbContext _context;

        public ForaController(JCarrollOnlineV2DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Fora
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ForaIndexViewModel foraIndexViewModel = new ForaIndexViewModel
            {
                ForaIndexItems = new List<ForaIndexItemViewModel>()
            };

            List<Forum> fora = await _context.Forum.ToListAsync().ConfigureAwait(false);

            foreach(Forum forum in fora)
            {
                ForaIndexItemViewModel foraIndexItemViewModel = new ForaIndexItemViewModel();

                foraIndexItemViewModel.InjectFrom(forum);
                foraIndexItemViewModel.ThreadCount = await ControllerHelpers.GetThreadCountAsync(forum, _context).ConfigureAwait(false);

                if (foraIndexItemViewModel.ThreadCount > 0)
                {
                    foraIndexItemViewModel.LastThread = await ControllerHelpers.GetLatestThreadDataAsync(forum, _context).ConfigureAwait(false);
                }

                foraIndexViewModel.ForaIndexItems.Add(foraIndexItemViewModel);
            }

            return View(foraIndexViewModel);
        }

        // GET: Fora/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Forum forum = await _context.Forum.FindAsync(id).ConfigureAwait(false);

            return forum == null ? HttpNotFound() : (ActionResult)View(forum);
        }

        // GET: Fora/Create
        [Authorize(Roles ="Administrator")]
        [HttpGet]
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
                _context.Forum.Add(forum);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(forumViewModel);
        }

        // GET: Fora/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Forum forum = await _context.Forum.FindAsync(id).ConfigureAwait(false);
            
            if (forum == null)
            {
                return HttpNotFound();
            }
            
            ForumEditViewModel forumEditViewModel = new ForumEditViewModel();
            forumEditViewModel.InjectFrom(forum);

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
                _context.Entry(forum).State = EntityState.Modified;
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(forum);
        }

        // GET: Fora/Delete/5
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Forum forum = await _context.Forum.FindAsync(id).ConfigureAwait(false);
            
            if (forum == null)
            {
                return HttpNotFound();
            }

            ForumDeleteViewModel forumDeleteViewModel = new ForumDeleteViewModel();
            forumDeleteViewModel.InjectFrom(forum);

            return View(forumDeleteViewModel);
        }

        // POST: Fora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Forum forum = await _context.Forum.FindAsync(id).ConfigureAwait(false);

            _context.Forum.Remove(forum);
            await _context.SaveChangesAsync().ConfigureAwait(false);

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
