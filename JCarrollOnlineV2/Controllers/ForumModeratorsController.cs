using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize(Roles="Administrator")]
    public class ForumModeratorsController : Controller
    {
        private IJCarrollOnlineV2Context _data { get; set; }

        public ForumModeratorsController() : this(null)
        {

        }

        public ForumModeratorsController(IJCarrollOnlineV2Context dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Connection();
        }

        // GET: ForumModerators
        public async Task<ActionResult> Index()
        {
            return View(await _data.ForumModerator.ToListAsync());
        }

        // GET: ForumModerators/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumModerator forumModerator = await _data.ForumModerator.FindAsync(id);
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Id,CreatedAt,UpdatedAt")] ForumModerator forumModerator)
        {
            if (ModelState.IsValid)
            {
                _data.ForumModerator.Add(forumModerator);
                await _data.SaveChangesAsync();
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
            ForumModerator forumModerator = await _data.ForumModerator.FindAsync(id);
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Id,CreatedAt,UpdatedAt")] ForumModerator forumModerator)
        {
            if (ModelState.IsValid)
            {
                _data.Entry(forumModerator).State = EntityState.Modified;
                await _data.SaveChangesAsync();
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
            ForumModerator forumModerator = await _data.ForumModerator.FindAsync(id);
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
            ForumModerator forumModerator = await _data.ForumModerator.FindAsync(id);
            _data.ForumModerator.Remove(forumModerator);
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
