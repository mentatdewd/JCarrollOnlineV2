using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Extensions;
using JCarrollOnlineV2.ViewModels;
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
        private IContext _data { get; set; }

        public ForumThreadEntriesController() : this(null)
        {

        }

        public ForumThreadEntriesController(IContext dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        private void DetailItemInjector(ForumThreadEntry domModel, ForumThreadEntryDetailsItemViewModel viewModel)
        {
            viewModel.Author = new ApplicationUserViewModel();
            viewModel.Author.InjectFrom(domModel.Author);
            viewModel.Forum = new ForaViewModel();
            viewModel.Forum.InjectFrom(domModel.Forum);
            if(domModel.ParentId != null)
                viewModel.ParentPostNumber = _data.ForumThreadEntries.Find(domModel.ParentId).PostNumber;

            viewModel.PostCount = _data.ForumThreadEntries.Where(m => m.Author.Id == domModel.Author.Id).Count();
        }

        // GET: ForumOriginalPost
        public async Task<ActionResult> Index(int forumId)
        {
            ForumThreadEntryIndexViewModel fvm = new ForumThreadEntryIndexViewModel();

            // Retrieve forum data
            Forum forum = _data.Forums.Find(forumId);
            fvm.Forum = new ForaViewModel();
            fvm.Forum.InjectFrom(forum);

            var forumThreads = _data.ForumThreadEntries.ToList();

            // Create the view model
            fvm.ForumThreadEntryIndex = new List<ForumThreadEntryIndexItemViewModel>();
            var forumThreadList = await _data.ForumThreadEntries.Where(p => p.Forum.Id == forumId && p.ParentId == null)
                .Include(p => p.Author)
                .Include(p => p.Forum)
                .ToListAsync();

            foreach (var item in forumThreadList)
            {
                var fitem = new ForumThreadEntryIndexItemViewModel();
                fitem.InjectFrom(item);
                fitem.Author = new ApplicationUserViewModel();
                fitem.Author.InjectFrom(item.Author);
                fitem.Forum = new ForaViewModel();
                fitem.Forum.InjectFrom(item.Forum);
                fitem.Replies = await ControllerHelpers.GetThreadPostCountAsync(item.Id, _data);
                fitem.LastReply = await ControllerHelpers.GetLastReplyAsync(item.RootId, _data);
                fvm.ForumThreadEntryIndex.Add(fitem);
            }
            return View(fvm);
        }

        // GET: ForumOriginalPost/Details/5
        public async Task<ActionResult> Details(int forumId, int id)
        {
            // Retreive the Detail data
            var detailItemInjector = new IEnumerableExtensions.InjectorDelegate<ForumThreadEntry, ForumThreadEntryDetailsItemViewModel>(DetailItemInjector);

            // Create the details view model
            ForumThreadEntryDetailsViewModel vm = new ForumThreadEntryDetailsViewModel();
            vm.ForumThreadEntryDetailItems = new ForumThreadEntryDetailItemsViewModel();
            vm.ForumThreadEntryDetailItems.ForumThreadEntries = await _data.ForumThreadEntries.Include("Author").Include("Forum").AsHierarchy("Id", "ParentId", id, 10).ProjectToViewAsync<ForumThreadEntry, ForumThreadEntryDetailsItemViewModel>(detailItemInjector);

            vm.Forum = new ForaDetailsViewModel();
            vm.Forum.InjectFrom(_data.Forums.Find(forumId));
            
            vm.ForumThreadEntryDetailItems.NumberOfReplies = _data.ForumThreadEntries.Where(b => b.RootId == id).Count();
            return View(vm);
        }

        // GET: ForumOriginalPost/Create
        [Authorize]
        public  ActionResult Create(int forumId, int? parentId, int? rootId)
        {
            ForumThreadEntriesCreateViewModel vm = new ForumThreadEntriesCreateViewModel();

            vm.ForumId = forumId;
            vm.ParentId = parentId;
            vm.RootId = rootId;

            return View(vm);
        }

        // POST: ForumOriginalPost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Title,RootId,ForumId,Content,ParentId,Id")]  ForumThreadEntriesCreateViewModel forumThreadEntryViewModel)
        {
            if (ModelState.IsValid)
            {
                ForumThreadEntry forumThreadEntry = new ForumThreadEntry();

                forumThreadEntry.InjectFrom(forumThreadEntryViewModel);

                forumThreadEntry.CreatedAt = DateTime.Now;
                forumThreadEntry.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                forumThreadEntry.Author = currentUser;

                forumThreadEntry.Forum = _data.Forums.Find(forumThreadEntryViewModel.ForumId);

                if (forumThreadEntry.ParentId != null)
                    forumThreadEntry.PostNumber = await _data.ForumThreadEntries.Where(m => m.RootId == forumThreadEntry.RootId).CountAsync() + 1;
                else
                { 
                    forumThreadEntry.PostNumber = 1;
                }

                _data.ForumThreadEntries.Add(forumThreadEntry);
                await _data.SaveChangesAsync();
                if (forumThreadEntry.ParentId == null)
                {
                    forumThreadEntry.UpdatedAt = forumThreadEntry.CreatedAt;
                    forumThreadEntry.RootId = forumThreadEntry.Id;
                    await _data.SaveChangesAsync();
                }
                return new RedirectResult(Url.Action("Details", new { forumId = forumThreadEntry.Forum.Id, id = forumThreadEntry.RootId }) + "#post" + forumThreadEntry.PostNumber);
            }

            return View(forumThreadEntryViewModel);
        }

        // GET: ForumOriginalPost/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThreadEntriesEditViewModel vm = new ForumThreadEntriesEditViewModel();

            var threadEntry = await _data.ForumThreadEntries.Include("Forum").SingleOrDefaultAsync(m => m.Id == id);

            if (threadEntry == null)
            {
                return HttpNotFound();
            }
            vm.InjectFrom(threadEntry);
            vm.ForumId = threadEntry.Forum.Id;
            vm.AuthorId = threadEntry.Author.Id;

            return View(vm);
        }

        // POST: ForumOriginalPost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ParentId,RootId,ForumId,AuthorId,Title,Content,CreatedAt,Locked,PostNumber")] ForumThreadEntriesEditViewModel forumThreadEntry)
        {
            if (ModelState.IsValid)
            {
                ForumThreadEntry domModel = new ForumThreadEntry();

                domModel.InjectFrom(forumThreadEntry);
                domModel.Author = await _data.Users.FindAsync(forumThreadEntry.AuthorId);
                domModel.Forum = await _data.Forums.FindAsync(forumThreadEntry.ForumId);
                domModel.UpdatedAt = DateTime.Now;

                _data.Entry(domModel).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = domModel.Forum.Id, id = domModel.RootId }) + "#post" + domModel.PostNumber);
            }
            return View(forumThreadEntry);
        }

        // GET: ForumOriginalPost/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThreadEntry forumThread = await _data.ForumThreadEntries.FindAsync(id);
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            return View(forumThread);
        }

        // POST: ForumOriginalPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ForumThreadEntry forumThread = await _data.ForumThreadEntries.FindAsync(id);
            _data.ForumThreadEntries.Remove(forumThread);
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
