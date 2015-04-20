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
using Microsoft.AspNet.Identity;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class MicropostsController : Controller
    {
        private IContext _data { get; set; }

        public MicropostsController() : this(null)
        {

        }

        public MicropostsController(IContext dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        // GET: Microposts
        public async Task<ActionResult> Index()
        {
            return View(await _data.Microposts.ToListAsync());
        }

        // GET: Microposts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Micropost micropost = await _data.Microposts.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // GET: Microposts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Microposts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] MicropostCreateViewModel micropostVM)
        {
            if (ModelState.IsValid)
            {
                Micropost domModel = new Micropost();
                domModel.InjectFrom(micropostVM);
                domModel.CreatedAt = DateTime.Now;
                domModel.UpdatedAt = DateTime.Now;
                domModel.UserId = User.Identity.GetUserId();
                _data.Microposts.Add(domModel);
                await _data.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(micropostVM);
        }

        // GET: Microposts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Micropost micropost = await _data.Microposts.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // POST: Microposts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] Micropost micropost)
        {
            if (ModelState.IsValid)
            {
                _data.Entry(micropost).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(micropost);
        }

        // GET: Microposts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Micropost micropost = await _data.Microposts.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // POST: Microposts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Micropost micropost = await _data.Microposts.FindAsync(id);
            _data.Microposts.Remove(micropost);
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
