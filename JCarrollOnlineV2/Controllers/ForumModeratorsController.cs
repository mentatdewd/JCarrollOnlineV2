using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize(Roles="Administrator")]
    public class ForumModeratorsController : Controller
    {
        private JCarrollOnlineV2DbContext Data { get; set; }

        public ForumModeratorsController() : this(null)
        {

        }

        public ForumModeratorsController(JCarrollOnlineV2DbContext dataContext)
        {
            Data = dataContext ?? new JCarrollOnlineV2DbContext();
        }

        // GET: ForumModerators
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await Data.ForumModerator.ToListAsync().ConfigureAwait(false));
        }

        // GET: ForumModerators/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ForumModerator forumModerator = await Data.ForumModerator.FindAsync(id).ConfigureAwait(false);

            return forumModerator == null ? HttpNotFound() : (ActionResult)View(forumModerator);
        }

        // GET: ForumModerators/Create
        [HttpGet]
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
                Data.ForumModerator.Add(forumModerator);
                await Data.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(forumModerator);
        }

        // GET: ForumModerators/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ForumModerator forumModerator = await Data.ForumModerator.FindAsync(id).ConfigureAwait(false);

            return forumModerator == null ? HttpNotFound() : (ActionResult)View(forumModerator);
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
                Data.Entry(forumModerator).State = EntityState.Modified;
                await Data.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(forumModerator);
        }

        // GET: ForumModerators/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ForumModerator forumModerator = await Data.ForumModerator.FindAsync(id).ConfigureAwait(false);

            return forumModerator == null ? HttpNotFound() : (ActionResult)View(forumModerator);
        }

        // POST: ForumModerators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ForumModerator forumModerator = await Data.ForumModerator.FindAsync(id).ConfigureAwait(false);

            Data.ForumModerator.Remove(forumModerator);
            await Data.SaveChangesAsync().ConfigureAwait(false);

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
