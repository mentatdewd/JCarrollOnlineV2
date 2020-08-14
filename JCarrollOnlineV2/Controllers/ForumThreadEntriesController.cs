using JCarrollOnlineV2.CustomLinqExtensions;
using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Extensions;
using JCarrollOnlineV2.ViewModels.Fora;
using JCarrollOnlineV2.ViewModels.ForumThreadEntries;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{

    public class ForumThreadEntriesController : Controller
    {
        private JCarrollOnlineV2DbContext Data { get; set; }

        public ForumThreadEntriesController() : this(null)
        {

        }

        public ForumThreadEntriesController(JCarrollOnlineV2DbContext dataContext)
        {
            Data = dataContext ?? new JCarrollOnlineV2DbContext();
        }

        private void DetailItemInjector(ThreadEntry threadEntry, ThreadEntryDetailsItemViewModel threadEntryDetailsItemViewModel)
        {
            threadEntryDetailsItemViewModel.Author.InjectFrom(threadEntry.Author);
            threadEntryDetailsItemViewModel.Forum.InjectFrom(threadEntry.Forum);

            if (threadEntry.ParentId != null)
            {
                threadEntryDetailsItemViewModel.ParentPostNumber = Data.ForumThreadEntry.Find(threadEntry.ParentId).PostNumber;
            }

            threadEntryDetailsItemViewModel.PostCount = Data.ForumThreadEntry.Where(m => m.Author.Id == threadEntry.Author.Id).Count();
        }

        // GET: ForumOriginalPost
        [HttpGet]
        public async Task<ActionResult> Index(int forumId)
        {
            ThreadEntryIndexViewModel threadEntryIndexViewModel = new ThreadEntryIndexViewModel();

            // Retrieve forum data
            Forum currentForum = await Data.Forum
                .Include(forum => forum.ForumThreadEntries
                .Select(forumThreadEntry => forumThreadEntry.Author))              
                .FirstAsync(forum => forum.Id == forumId).ConfigureAwait(false);

            threadEntryIndexViewModel.Forum = new ForaViewModel();
            threadEntryIndexViewModel.Forum.InjectFrom(currentForum);

            // Create the view model

            foreach (ThreadEntry forumThread in currentForum.ForumThreadEntries.Where(forumThreadEntry => forumThreadEntry.ParentId == null))
            {
                ThreadEntryIndexItemViewModel threadEntryIndexItemViewModel = new ThreadEntryIndexItemViewModel();

                threadEntryIndexItemViewModel.InjectFrom(forumThread);
                threadEntryIndexItemViewModel.Forum.InjectFrom(currentForum);
                threadEntryIndexItemViewModel.Author.InjectFrom(forumThread.Author);
                
                threadEntryIndexItemViewModel.Replies = currentForum.ForumThreadEntries.Where(forumThreadEntry => forumThreadEntry.RootId == forumThread.Id && forumThreadEntry.ParentId != null).Count();
                threadEntryIndexItemViewModel.LastReply = currentForum.ForumThreadEntries.Where(m => m.RootId == forumThread.Id).OrderBy(m => m.UpdatedAt.ToFileTime()).FirstOrDefault().UpdatedAt;
                threadEntryIndexViewModel.ThreadEntryIndex.Add(threadEntryIndexItemViewModel);
            }

            return View(threadEntryIndexViewModel);
        }

        // GET: ForumOriginalPost/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(int forumId, int? id)
        {
            // Retreive the Detail data
            if (id == null)
            {
                return RedirectToAction("index", routeValues: new {  forumId });
            }

            IEnumerableExtensions.InjectorDelegate<ThreadEntry, ThreadEntryDetailsItemViewModel> detailItemInjector = new IEnumerableExtensions.InjectorDelegate<ThreadEntry, ThreadEntryDetailsItemViewModel>(DetailItemInjector);

            // Create the details view model
            ThreadEntryDetailsViewModel threadEntryDetailsViewModel = new ThreadEntryDetailsViewModel();

            IQueryable<ThreadEntry> forumThreadEntries = Data.ForumThreadEntry.Include(forumThreadEntry => forumThreadEntry.Author)
                .Include(forumThreadEntry => forumThreadEntry.Forum)
                .Where(forumThreadEntry => forumThreadEntry.Forum.Id == forumId);

            threadEntryDetailsViewModel.ForumThreadEntryDetailItems.ForumThreadEntries = 
                await forumThreadEntries.AsHierarchy("Id", "ParentId", id, 10)
                .ProjectToViewAsync(detailItemInjector).ConfigureAwait(false);

            threadEntryDetailsViewModel.Forum.InjectFrom(Data.Forum.Find(forumId)); 

            threadEntryDetailsViewModel.ForumThreadEntryDetailItems.NumberOfReplies = Data.ForumThreadEntry.Where(b => b.RootId == id).Count();

            return View(threadEntryDetailsViewModel);
        }

        // GET: ForumOriginalPost/Create
        [Authorize]
        [HttpGet]
        public  ActionResult Create(int forumId, int? parentId, int? rootId)
        {
            ThreadEntriesCreateViewModel threadEntriesCreateViewModel = new ThreadEntriesCreateViewModel
            {
                ForumId = forumId,
                ParentId = parentId,
                RootId = rootId
            };

            return View(threadEntriesCreateViewModel);
        }

        // POST: ForumOriginalPost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Title,RootId,ForumId,Content,ParentId,Id")]  ThreadEntriesCreateViewModel forumThreadEntryViewModel)
        {
            if (ModelState.IsValid)
            {
                ThreadEntry threadEntry = new ThreadEntry();

                threadEntry.InjectFrom(forumThreadEntryViewModel);

                threadEntry.CreatedAt = DateTime.Now;
                threadEntry.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);

                threadEntry.Author = currentUser;
                if (forumThreadEntryViewModel != null)
                {
                    threadEntry.Forum = Data.Forum.Find(forumThreadEntryViewModel.ForumId);
                }
                threadEntry.PostNumber = threadEntry.ParentId != null
                    ? await Data.ForumThreadEntry.Where(m => m.RootId == threadEntry.RootId).CountAsync().ConfigureAwait(false) + 1
                    : 1;

                Data.ForumThreadEntry.Add(threadEntry);
                await Data.SaveChangesAsync().ConfigureAwait(false);

                if (threadEntry.ParentId == null)
                {
                    threadEntry.UpdatedAt = threadEntry.CreatedAt;
                    threadEntry.RootId = threadEntry.Id;
                    await Data.SaveChangesAsync().ConfigureAwait(false);
                }

                return new RedirectResult(Url.Action("Details", new { forumId = threadEntry.Forum.Id, id = threadEntry.RootId }) + "#post" + threadEntry.PostNumber);
            }

            return View(forumThreadEntryViewModel);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult AdminCreate(int forumId, int? parentId, int? rootId)
        {
            AdminThreadEntriesCreateViewModel threadEntriesCreateViewModel = new AdminThreadEntriesCreateViewModel
            {
                ForumId = forumId,
                ParentId = parentId,
                RootId = rootId
            };

            return View(threadEntriesCreateViewModel);
        }

        // POST: ForumOriginalPost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
#pragma warning disable CA5363 // Do Not Disable Request Validation
        public async Task<ActionResult> AdminCreate([Bind(Include = "Title,RootId,ForumId,Content,ParentId,Id")] AdminThreadEntriesCreateViewModel forumThreadEntryViewModel)
#pragma warning restore CA5363 // Do Not Disable Request Validation
        {
            if (ModelState.IsValid)
            {
                ThreadEntry threadEntry = new ThreadEntry();

                threadEntry.InjectFrom(forumThreadEntryViewModel);

                threadEntry.CreatedAt = DateTime.Now;
                threadEntry.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);

                threadEntry.Author = currentUser;
                if (forumThreadEntryViewModel != null)
                {
                    threadEntry.Forum = Data.Forum.Find(forumThreadEntryViewModel.ForumId);
                }
                threadEntry.PostNumber = threadEntry.ParentId != null
                    ? await Data.ForumThreadEntry.Where(m => m.RootId == threadEntry.RootId).CountAsync().ConfigureAwait(false) + 1
                    : 1;

                Data.ForumThreadEntry.Add(threadEntry);
                await Data.SaveChangesAsync().ConfigureAwait(false);

                if (threadEntry.ParentId == null)
                {
                    threadEntry.UpdatedAt = threadEntry.CreatedAt;
                    threadEntry.RootId = threadEntry.Id;
                    await Data.SaveChangesAsync().ConfigureAwait(false);
                }

                return new RedirectResult(Url.Action("Details", new { forumId = threadEntry.Forum.Id, id = threadEntry.RootId }) + "#post" + threadEntry.PostNumber);
            }

            return View(forumThreadEntryViewModel);
        }

        // GET: ForumOriginalPost/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ThreadEntriesEditViewModel threadEntriesViewModel = new ThreadEntriesEditViewModel();

            ThreadEntry forumThread = await Data.ForumThreadEntry.Include("Forum").SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

            if (forumThread == null)
            {
                return HttpNotFound();
            }

            threadEntriesViewModel.InjectFrom(forumThread);
            threadEntriesViewModel.ForumId = forumThread.Forum.Id;
            threadEntriesViewModel.AuthorId = forumThread.Author.Id;

            return View(threadEntriesViewModel);
        }

        // POST: ForumOriginalPost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ParentId,RootId,ForumId,AuthorId,Title,Content,CreatedAt,Locked,PostNumber")] ThreadEntriesEditViewModel forumThreadEntry)
        {
            if (ModelState.IsValid)
            {
                ThreadEntry threadEntry = new ThreadEntry();

                threadEntry.InjectFrom(forumThreadEntry);
                if (forumThreadEntry != null)
                {
                    threadEntry.Author = await Data.ApplicationUser.FindAsync(forumThreadEntry.AuthorId).ConfigureAwait(false);
                    threadEntry.Forum = await Data.Forum.FindAsync(forumThreadEntry.ForumId).ConfigureAwait(false);
                }
                threadEntry.UpdatedAt = DateTime.Now;

                Data.Entry(threadEntry).State = EntityState.Modified;
                await Data.SaveChangesAsync().ConfigureAwait(false);

                return Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = threadEntry.Forum.Id, id = threadEntry.RootId }) + "#post" + threadEntry.PostNumber);
            }

            return View(forumThreadEntry);
        }

        // GET: ForumOriginalPost/Edit/5
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult> AdminEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ThreadEntriesEditViewModel threadEntriesViewModel = new ThreadEntriesEditViewModel();

            ThreadEntry forumThread = await Data.ForumThreadEntry.Include("Forum").SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

            if (forumThread == null)
            {
                return HttpNotFound();
            }

            threadEntriesViewModel.InjectFrom(forumThread);
            threadEntriesViewModel.ForumId = forumThread.Forum.Id;
            threadEntriesViewModel.AuthorId = forumThread.Author.Id;

            return View(threadEntriesViewModel);
        }

        // POST: ForumOriginalPost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
#pragma warning disable CA5363 // Do Not Disable Request Validation
        public async Task<ActionResult> AdminEdit([Bind(Include = "Id,ParentId,RootId,ForumId,AuthorId,Title,Content,CreatedAt,Locked,PostNumber")] ThreadEntriesEditViewModel forumThreadEntry)
#pragma warning restore CA5363 // Do Not Disable Request Validation
        {
            if (ModelState.IsValid)
            {
                ThreadEntry threadEntry = new ThreadEntry();

                threadEntry.InjectFrom(forumThreadEntry);
                if (forumThreadEntry != null)
                {
                    threadEntry.Author = await Data.ApplicationUser.FindAsync(forumThreadEntry.AuthorId).ConfigureAwait(false);
                    threadEntry.Forum = await Data.Forum.FindAsync(forumThreadEntry.ForumId).ConfigureAwait(false);
                }
                threadEntry.UpdatedAt = DateTime.Now;

                Data.Entry(threadEntry).State = EntityState.Modified;
                await Data.SaveChangesAsync().ConfigureAwait(false);

                return Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = threadEntry.Forum.Id, id = threadEntry.RootId }) + "#post" + threadEntry.PostNumber);
            }

            return View(forumThreadEntry);
        }

        // GET: ForumOriginalPost/Delete/5
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ThreadEntry threadEntry = await Data.ForumThreadEntry.Include("Author").Include("Forum").Where(m => m.Id == id).SingleOrDefaultAsync().ConfigureAwait(false);
            ThreadEntryDetailsItemViewModel threadEntryDetailsItemViewModel = new ThreadEntryDetailsItemViewModel();

            threadEntryDetailsItemViewModel.InjectFrom(threadEntry);
            threadEntryDetailsItemViewModel.Author.InjectFrom(threadEntry.Author);
            threadEntryDetailsItemViewModel.Forum.InjectFrom(threadEntry.Forum);

            return threadEntry == null ? HttpNotFound() : (ActionResult)View(threadEntryDetailsItemViewModel);
        }

        // POST: ForumOriginalPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ThreadEntry threadEntry = await Data.ForumThreadEntry.Include("Author").Include("Forum").Where(m => m.Id == id).SingleOrDefaultAsync().ConfigureAwait(false);
            string userId = User.Identity.GetUserId();
            ApplicationUser user = await Data.ApplicationUser.FindAsync(userId).ConfigureAwait(false);

            threadEntry.Title = "This post was deleted on " + DateTime.Now + " by " + user.UserName;
            threadEntry.Content = "";
            threadEntry.Locked = true;
            threadEntry.UpdatedAt = DateTime.Now;
            Data.Entry(threadEntry).State = EntityState.Modified;

            await Data.SaveChangesAsync().ConfigureAwait(false);

            return threadEntry.ParentId == null
                ? RedirectToAction("Index", new { forumId = threadEntry.Forum.Id })
                : (ActionResult)Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = threadEntry.Forum.Id, id = threadEntry.RootId }) + "#post" + threadEntry.PostNumber);
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
