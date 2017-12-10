using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Omu.ValueInjecter;
using System;
using System.Net;
using System.Collections.Generic;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BlogController : Controller
    {
        private IJCarrollOnlineV2Context _data { get; set; }

        public BlogController()
            : this(null)
        {

        }

        public BlogController(IJCarrollOnlineV2Context dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Connection();
        }

        // GET: BlogItemId
        public async Task<ActionResult> Index()
        {
            BlogIndexViewModel biVM = new BlogIndexViewModel();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = await _data.ApplicationUser.FindAsync(currentUserId);
            List<BlogItem> blogItems = await _data.BlogItem.Include("BlogItemComments").ToListAsync();

            foreach(var item in blogItems.OrderByDescending(m => m.UpdatedAt))
            {
                BlogFeedItemViewModel bfi = new BlogFeedItemViewModel();
                bfi.InjectFrom(item);
                bfi.Author.InjectFrom(item.Author);
                bfi.Comments.BlogItemId = item.Id;

                foreach(var comment in item.BlogItemComments.ToList())
                {
                    BlogCommentItemViewModel bciVM = new BlogCommentItemViewModel();
                    bciVM.InjectFrom(comment);
                    bciVM.TimeAgo = bciVM.CreatedAt.ToUniversalTime().ToString("o");
                    bfi.Comments.BlogComments.Add(bciVM);
                }
                biVM.BlogFeedItems.BlogFeedItemVMs.Add(bfi);               
            }
            return View(biVM);
        }

        // GET: BlogItemId/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BlogItemComment/CreateComment
        public ActionResult CreateComment(int blogItemId, string returnUrl)
        {
            BlogCommentItemViewModel bciVM = new BlogCommentItemViewModel();
            bciVM.BlogItemId = blogItemId;
            bciVM.ReturnUrl = returnUrl;
            return View("_BlogCommentFormPartial", bciVM);
        }

        // POST: BlogItemComment/CreateComment
        [HttpPost]
        public async Task<ActionResult> CreateComment([Bind(Include = "Author,Content,BlogItemId,ReturnUrl")] BlogCommentItemViewModel blogCommentItemViewModel)
        {
            if(ModelState.IsValid)
            {
                BlogItemComment bic = new BlogItemComment();

                bic.InjectFrom(blogCommentItemViewModel);
                bic.CreatedAt = DateTime.Now;

                bic.BlogItem = await _data.BlogItem.FindAsync(blogCommentItemViewModel.BlogItemId);

                _data.BlogItemComment.Add(bic);
                await _data.SaveChangesAsync();

                //var cmts = await _data.BlogItemComments.Include("BlogItem").Where(m => m.BlogItem.Id == bic.BlogItem.Id).ToListAsync();
                //BlogCommentsViewModel comments = new BlogCommentsViewModel();

                //foreach(var item in cmts)
                //{
                //    BlogCommentItemViewModel bciVM = new BlogCommentItemViewModel();
                //    bciVM.InjectFrom(item);

                //    comments.BlogComments.Add(bciVM);
                //}
                
                return new RedirectResult(blogCommentItemViewModel.ReturnUrl);
                //return PartialView("_BlogCommentsPartial", comments);
            }
            return View(blogCommentItemViewModel);
        }

        // GET: BlogItemId/Create
        public ActionResult Create()
        {
            BlogFeedItemViewModel bfiVM = new BlogFeedItemViewModel();

            return View(bfiVM);
        }

        // POST: BlogItemId/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Title,Content")]  BlogFeedItemViewModel blogFeedItemViewModel)
        {
            if (ModelState.IsValid)
            {
                BlogItem blogPost = new BlogItem();

                blogPost.InjectFrom(blogFeedItemViewModel);

                blogPost.CreatedAt = DateTime.Now;
                blogPost.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId);
                blogPost.Author = currentUser;

                _data.BlogItem.Add(blogPost);
                await _data.SaveChangesAsync();

                return new RedirectResult(Url.Action("Index"));
            }

            return View(blogFeedItemViewModel);
        }

        // GET: BlogItemId/Edit/5
        public async Task<ActionResult> Edit(int blogItemId)
        {
            BlogFeedItemViewModel bfiVM = new BlogFeedItemViewModel();

            var blogItem = await _data.BlogItem.Include("Author").SingleOrDefaultAsync(m => m.Id == blogItemId);

            if (blogItem == null)
            {
                return HttpNotFound();
            }
            bfiVM.InjectFrom(blogItem);
            bfiVM.Author.InjectFrom(blogItem.Author);
            bfiVM.AuthorId = bfiVM.Author.Id;

            return View(bfiVM);
        }

        // POST: BlogItemId/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AuthorId,Title,Content,CreatedAt")] BlogFeedItemViewModel blogItemVM)
        {
            if (ModelState.IsValid)
            {
                BlogItem domModel = new BlogItem();

                domModel.InjectFrom(blogItemVM);
                domModel.Author = await _data.ApplicationUser.FindAsync(blogItemVM.AuthorId);
                domModel.UpdatedAt = DateTime.Now;

                _data.Entry(domModel).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return Redirect(Url.RouteUrl(new { controller = "Blog", action = "Index"}));
            }
            return View(blogItemVM);
        }

        // GET: BlogItemId/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BlogItemId/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
