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
using Microsoft.AspNet.Identity;
using JCarrollOnlineV2.Extensions;
using Omu.ValueInjecter;
using JCarrollOnlineV2;

namespace JCarrollOnlineV2.Controllers
{

    public class ForumThreadEntriesController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

        private void DetailItemInjector(ForumThreadEntryDetailsItemViewModel item)
        {
            item.Author = ControllerHelpers.GetAuthor(item.AuthorId);
            item.PostCount = ControllerHelpers.GetAuthorPostCount(item.AuthorId);
            item.ParentPostNumber = ControllerHelpers.GetParentPostNumber(item.ParentId);
        }

        private void TOCDetailItemInjector(ForumThreadEntryTOCItemViewModel item)
        {
            item.Author = ControllerHelpers.GetAuthor(item.AuthorId);
        }

        // GET: ForumOriginalPost
        public async Task<ActionResult> Index(int forumId)
        {
            ForumThreadEntryIndexViewModel fvm = new ForumThreadEntryIndexViewModel();

            fvm.ForumThreadIndexEntries = new List<ForumThreadEntryIndexItemViewModel>();
            fvm.ForumId = forumId;
            fvm.ForumTitle = db.Forums.Where(m => m.ForumId == forumId).FirstOrDefault().Title;
            var dlist = await db.ForumThreadEntries.Where(p => p.ForumId == forumId && p.ParentId == null).ToListAsync();

            foreach (var item in dlist)
            {
                var fitem = new ForumThreadEntryIndexItemViewModel();
                fitem.InjectFrom<FilterId>(item);
                fitem.ForumId = forumId;
                fitem.Author = await ControllerHelpers.GetAuthorAsync(item.AuthorId);
                fitem.Replies = await ControllerHelpers.GetThreadPostCountAsync(item.ForumThreadEntryId);
                fitem.LastReply = await ControllerHelpers.GetLastReplyAsync(item.RootId);
                fvm.ForumThreadIndexEntries.Add(fitem);
            }
            return View(fvm);
        }

        // GET: ForumOriginalPost/Details/5
        public async Task<ActionResult> Details(int forumId, int forumThreadEntryId)
        {
            var d1 = new IEnumerableExtensions.InjectorDelegate<ForumThreadEntryTOCItemViewModel>(TOCDetailItemInjector);
            IEnumerable<HierarchyNode<ForumThreadEntryTOCItemViewModel>> fteTOCHierarchy = await db.ForumThreadEntries.AsHierarchy("ForumThreadEntryId", "ParentId", forumThreadEntryId, 10).ProjectToViewAsync<ForumThreadEntry, ForumThreadEntryTOCItemViewModel>(d1);

            var d2 = new IEnumerableExtensions.InjectorDelegate<ForumThreadEntryDetailsItemViewModel>(DetailItemInjector);
            IEnumerable<HierarchyNode<ForumThreadEntryDetailsItemViewModel>> fteHierarchy = await db.ForumThreadEntries.AsHierarchy("ForumThreadEntryId", "ParentId", forumThreadEntryId, 10).ProjectToViewAsync<ForumThreadEntry, ForumThreadEntryDetailsItemViewModel>(d2);



            ForumThreadEntryDetailsViewModel vm = new ForumThreadEntryDetailsViewModel();
            vm.ForumThreadEntryDetailItems = new ForumThreadEntryDetailItemsViewModel();
            vm.ForumThreadEntryDetailItems.ForumThreadEntries = fteHierarchy;

            vm.ForumThreadEntryTOCItems = new ForumThreadEntryTOCItemsViewModel();
            vm.ForumThreadEntryTOCItems.ForumThreadEntriesToc = fteTOCHierarchy;

            vm.ForumThreadEntryTOCItems.NumberOfReplies = db.ForumThreadEntries.Where(b => b.RootId == forumThreadEntryId).Count();

            vm.ForumTitle = db.Forums.Find(forumId).Title;
            vm.ForumId = forumId;

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
        public async Task<ActionResult> Create([Bind(Include = "Title,RootId,Content,ParentId,ForumId")]  ForumThreadEntriesCreateViewModel forumThreadEntryViewModel)
        {
            if (ModelState.IsValid)
            {
                ForumThreadEntry forumThreadEntry = new ForumThreadEntry();

                forumThreadEntry.InjectFrom(forumThreadEntryViewModel);

                forumThreadEntry.CreatedAt = DateTime.Now;
                forumThreadEntry.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await db.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                forumThreadEntry.AuthorId = currentUser.Id;

                if (forumThreadEntry.ParentId != null)
                    forumThreadEntry.PostNumber = await db.ForumThreadEntries.Where(m => m.RootId == forumThreadEntry.RootId).CountAsync() + 1;
                else
                { 
                    forumThreadEntry.PostNumber = 1;
                }

                db.ForumThreadEntries.Add(forumThreadEntry);
                await db.SaveChangesAsync();
                if (forumThreadEntry.ParentId == null)
                {
                    forumThreadEntry.UpdatedAt = forumThreadEntry.CreatedAt;
                    forumThreadEntry.RootId = forumThreadEntry.ForumThreadEntryId;
                    await db.SaveChangesAsync();
                }
                return new RedirectResult(Url.Action("Details", new { forumId = forumThreadEntry.ForumId, forumThreadEntryId = forumThreadEntry.RootId }) + "#post" + forumThreadEntry.PostNumber);
            }

            return View(forumThreadEntryViewModel);
        }

        // GET: ForumOriginalPost/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? forumThreadEntryId)
        {
            if (forumThreadEntryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThreadEntriesEditViewModel vm = new ForumThreadEntriesEditViewModel();

            var threadEntry = await db.ForumThreadEntries.FindAsync(forumThreadEntryId);
            if (threadEntry == null)
            {
                return HttpNotFound();
            }

            vm.InjectFrom(threadEntry);
            return View(vm);
        }

        // POST: ForumOriginalPost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "ForumThreadEntryId,ParentId,RootId,Title,Content,AuthorId,Locked,ForumId,CreatedAt,UpdatedAt,PostNumber")] ForumThreadEntriesEditViewModel forumThreadEntry)
        {
            if (ModelState.IsValid)
            {
                ForumThreadEntry domModel = new ForumThreadEntry();

                domModel.InjectFrom(forumThreadEntry);
                domModel.UpdatedAt = DateTime.Now;

                db.Entry(domModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Details", new { forumId = domModel.ForumId, forumThreadEntryId = domModel.RootId }) + "#post" + domModel.PostNumber;
                return Redirect(Url.RouteUrl(new { controller = "ForumThreadEntries", action = "Details", forumId = domModel.ForumId, forumThreadEntryId = domModel.RootId }) + "#post" + domModel.PostNumber);
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
            ForumThreadEntry forumThread = await db.ForumThreadEntries.FindAsync(id);
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
            ForumThreadEntry forumThread = await db.ForumThreadEntries.FindAsync(id);
            db.ForumThreadEntries.Remove(forumThread);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
