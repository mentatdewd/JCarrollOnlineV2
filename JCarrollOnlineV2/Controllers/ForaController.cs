using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
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
        private IContext _data { get; set; }

        public ForaController() : this(null)
        {

        }

        public ForaController(IContext dataContext = null)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        // GET: Fora
        public async Task<ActionResult> Index()
        {
            ForaIndexViewModel fiVM = new ForaIndexViewModel();
            fiVM.ForaIndexItems = new List<ForaIndexItemViewModel>();

            var dlist = await _data.Forums.ToListAsync();
            foreach(var item in dlist)
            {
                var fiiVM = new ForaIndexItemViewModel();
                fiiVM.InjectFrom(item);
                fiiVM.ThreadCount = await ControllerHelpers.GetThreadCountAsync(item, _data);
                if(fiiVM.ThreadCount > 0)
                    fiiVM.LastThread = await ControllerHelpers.GetLatestThreadDataAsync(item, _data);

                fiVM.ForaIndexItems.Add(fiiVM);
            }
            return View(fiVM);
        }

        // GET: Fora/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = await _data.Forums.FindAsync(id);
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
            ForaCreateViewModel fcVM = new ForaCreateViewModel();
            return View(fcVM);
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
                _data.Forums.Add(forum);
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
            Forum forum = await _data.Forums.FindAsync(id);
            ForumEditViewModel forumEditVM = new ForumEditViewModel();
            forumEditVM.InjectFrom(forum);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forumEditVM);
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
            Forum forum = await _data.Forums.FindAsync(id);
            ForumDeleteViewModel forumDeleteVM = new ForumDeleteViewModel();
            forumDeleteVM.InjectFrom(forum);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forumDeleteVM);
        }

        // POST: Fora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Forum forum = await _data.Forums.FindAsync(id);
            _data.Forums.Remove(forum);
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
