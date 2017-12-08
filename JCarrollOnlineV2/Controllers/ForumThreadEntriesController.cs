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
            viewModel.Author.InjectFrom(domModel.Author);
            viewModel.Forum.InjectFrom(domModel.Forum);
            if(domModel.ParentId != null)
                viewModel.ParentPostNumber = _data.ForumThreadEntries.Find(domModel.ParentId).PostNumber;

            viewModel.PostCount = _data.ForumThreadEntries.Where(m => m.Author.Id == domModel.Author.Id).Count();
        }

        public void Format()
        {
            System.Diagnostics.Debug.WriteLine("");
        }

        // GET: ForumOriginalPost
        public async Task<ActionResult> Index(int forumId)
        {
            ForumThreadEntryIndexViewModel fteiVM = new ForumThreadEntryIndexViewModel();

            // Retrieve forum data
            Forum forum = _data.Forums.Find(forumId);
            fteiVM.Forum = new ForaViewModel();
            fteiVM.Forum.InjectFrom(forum);

            var forumThreads = _data.ForumThreadEntries.ToList();

            // Create the view model
            fteiVM.ForumThreadEntryIndex = new List<ForumThreadEntryIndexItemViewModel>();
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
                fteiVM.ForumThreadEntryIndex.Add(fitem);
            }
            return View(fteiVM);
        }

        // GET: ForumOriginalPost/Details/5
        public async Task<ActionResult> Details(int forumId, int? id)
        {
            // Retreive the Detail data
            if (id == null)
                return RedirectToAction("index", new { forumId = forumId });
            var detailItemInjector = new IEnumerableExtensions.InjectorDelegate<ForumThreadEntry, ForumThreadEntryDetailsItemViewModel>(DetailItemInjector);

            // Create the details view model
            ForumThreadEntryDetailsViewModel ftedVM = new ForumThreadEntryDetailsViewModel();
            ftedVM.ForumThreadEntryDetailItems = new ForumThreadEntryDetailItemsViewModel();
            ftedVM.ForumThreadEntryDetailItems.ForumThreadEntries = await _data.ForumThreadEntries.Include("Author").Include("Forum").AsHierarchy("Id", "ParentId", id, 10).ProjectToViewAsync<ForumThreadEntry, ForumThreadEntryDetailsItemViewModel>(detailItemInjector);
            var tmp =  _data.ForumThreadEntries.Include("Author").Include("Forum");//.AsHierarchy("Id", "ParentId", id, 10).ProjectToViewAsync<ForumThreadEntry, ForumThreadEntryDetailsItemViewModel>(detailItemInjector);

            ftedVM.Forum = new ForaDetailsViewModel();
            ftedVM.Forum.InjectFrom(_data.Forums.Find(forumId));
            
            ftedVM.ForumThreadEntryDetailItems.NumberOfReplies = _data.ForumThreadEntries.Where(b => b.RootId == id).Count();
            return View(ftedVM);
        }

        // GET: ForumOriginalPost/Create
        [Authorize]
        public  ActionResult Create(int forumId, int? parentId, int? rootId)
        {
            ForumThreadEntriesCreateViewModel ftecVM = new ForumThreadEntriesCreateViewModel();

            ftecVM.ForumId = forumId;
            ftecVM.ParentId = parentId;
            ftecVM.RootId = rootId;

            return View(ftecVM);
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
                ForumThreadEntry forumThread = new ForumThreadEntry();

                forumThread.InjectFrom(forumThreadEntryViewModel);

                forumThread.CreatedAt = DateTime.Now;
                forumThread.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                forumThread.Author = currentUser;

                forumThread.Forum = _data.Forums.Find(forumThreadEntryViewModel.ForumId);

                if (forumThread.ParentId != null)
                    forumThread.PostNumber = await _data.ForumThreadEntries.Where(m => m.RootId == forumThread.RootId).CountAsync() + 1;
                else
                { 
                    forumThread.PostNumber = 1;
                }

                _data.ForumThreadEntries.Add(forumThread);
                await _data.SaveChangesAsync();
                if (forumThread.ParentId == null)
                {
                    forumThread.UpdatedAt = forumThread.CreatedAt;
                    forumThread.RootId = forumThread.Id;
                    await _data.SaveChangesAsync();
                }
                return new RedirectResult(Url.Action("Details", new { forumId = forumThread.Forum.Id, id = forumThread.RootId }) + "#post" + forumThread.PostNumber);
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
            ForumThreadEntriesEditViewModel fteeVM = new ForumThreadEntriesEditViewModel();

            var forumThread = await _data.ForumThreadEntries.Include("Forum").SingleOrDefaultAsync(m => m.Id == id);

            if (forumThread == null)
            {
                return HttpNotFound();
            }
            fteeVM.InjectFrom(forumThread);
            fteeVM.ForumId = forumThread.Forum.Id;
            fteeVM.AuthorId = forumThread.Author.Id;

            return View(fteeVM);
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
                ForumThreadEntry forumThread = new ForumThreadEntry();

                forumThread.InjectFrom(forumThreadEntry);
                forumThread.Author = await _data.Users.FindAsync(forumThreadEntry.AuthorId);
                forumThread.Forum = await _data.Forums.FindAsync(forumThreadEntry.ForumId);
                forumThread.UpdatedAt = DateTime.Now;

                _data.Entry(forumThread).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = forumThread.Forum.Id, id = forumThread.RootId }) + "#post" + forumThread.PostNumber);
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
            ForumThreadEntry forumThread = await _data.ForumThreadEntries.Include("Author").Include("Forum").Where(m => m.Id == id).SingleOrDefaultAsync();
            ForumThreadEntryDetailsItemViewModel ftediVM = new ForumThreadEntryDetailsItemViewModel();

            ftediVM.InjectFrom(forumThread);
            ftediVM.Author.InjectFrom(forumThread.Author);
            ftediVM.Forum.InjectFrom(forumThread.Forum);
     
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            return View(ftediVM);
        }

        // POST: ForumOriginalPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ForumThreadEntry forumThread = await _data.ForumThreadEntries.Include("Author").Include("Forum").Where(m => m.Id == id).SingleOrDefaultAsync();
            string userId = User.Identity.GetUserId();
            ApplicationUser user = await _data.Users.FindAsync(userId);

            forumThread.Title = "This post was deleted on " + DateTime.Now + " by " + user.UserName;
            forumThread.Content = "";
            forumThread.Locked = true;
            forumThread.UpdatedAt = DateTime.Now;
            _data.Entry(forumThread).State = EntityState.Modified;
            await _data.SaveChangesAsync();
            if (forumThread.ParentId == null)
            {
                return RedirectToAction("Index", new { forumId = forumThread.Forum.Id });
            }
            else
            {
                return Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = forumThread.Forum.Id, id = forumThread.RootId }) + "#post" + forumThread.PostNumber);
            }
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
