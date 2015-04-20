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
            ForaIndexViewModel fvm = new ForaIndexViewModel();
            fvm.ForaIndexItems = new List<ForaIndexItemViewModel>();

            var dlist = await _data.Forums.ToListAsync();
            foreach(var item in dlist)
            {
                var fitem = new ForaIndexItemViewModel();
                fitem.InjectFrom(item);
                fitem.ThreadCount = await ControllerHelpers.GetThreadCountAsync(item, _data);
                if(fitem.ThreadCount > 0)
                    fitem.LastThread = await ControllerHelpers.GetLatestThreadDataAsync(item, _data);

                fvm.ForaIndexItems.Add(fitem);
            }
            return View(fvm);
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
            Forum forum = await _data.Forums.FindAsync(id);
            _data.Forums.Remove(forum);
            await _data.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _data.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
