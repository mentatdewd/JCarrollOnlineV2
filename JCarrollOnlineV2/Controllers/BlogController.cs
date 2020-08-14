using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Blog;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BlogController : Controller
    {
        private JCarrollOnlineV2DbContext Data { get; set; }

        public BlogController()
            : this(null)
        {

        }

        public BlogController(JCarrollOnlineV2DbContext dataContext)
        {
            Data = dataContext ?? new JCarrollOnlineV2DbContext();
        }

        // GET: BlogItemId
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            BlogIndexViewModel blogIndexViewModel = new BlogIndexViewModel();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = await Data.ApplicationUser.FindAsync(currentUserId).ConfigureAwait(false);
            List<BlogItem> blogItems = await Data.BlogItem.Include("BlogItemComments").ToListAsync().ConfigureAwait(false);

            foreach(BlogItem blogItem in blogItems.OrderByDescending(m => m.UpdatedAt))
            {
                BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

                blogFeedItemViewModel.InjectFrom(blogItem);
                blogFeedItemViewModel.Author.InjectFrom(blogItem.Author);

                foreach(BlogItemComment blogItemComment in blogItem.BlogItemComments.ToList())
                {
                    BlogCommentItemViewModel blogCommentItemViewModel = new BlogCommentItemViewModel(blogItem.Id);

                    blogCommentItemViewModel.InjectFrom(blogItemComment);
                    blogCommentItemViewModel.TimeAgo = blogCommentItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    blogFeedItemViewModel.Comments.BlogComments.Add(blogCommentItemViewModel);
                }

                blogIndexViewModel.BlogFeedItems.BlogFeedItemViewModels.Add(blogFeedItemViewModel);               
            }

            return View(blogIndexViewModel);
        }

        // GET: BlogItemId/Details/5
        [HttpGet]
        public ActionResult Details()
        {
            return View();
        }

        // POST: BlogItemComment/CreateComment
        [HttpPost]
        public void CreateComment(BlogCommentItemViewModel blogCommentItemViewModel)
        {
            if(ModelState.IsValid)
            {
                BlogItemComment blogItemComment = new BlogItemComment();

                blogItemComment.InjectFrom(blogCommentItemViewModel);
                blogItemComment.CreatedAt = DateTime.Now;

                if (blogCommentItemViewModel != null)
                {
                    blogItemComment.BlogItem = Data.BlogItem.Find(blogCommentItemViewModel.Id);
                }
                Data.BlogItemComment.Add(blogItemComment);
                Data.SaveChanges();
            }
        }

        // GET: BlogItemId/Create
        [HttpGet]
        public ActionResult Create()
        {
            BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

            return View(blogFeedItemViewModel);
        }

        // POST: BlogItemId/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
#pragma warning disable CA5363 // Do Not Disable Request Validation
        public async Task<ActionResult> Create([Bind(Include = "Title,Content")]  BlogFeedItemViewModel blogFeedItemViewModel)
#pragma warning restore CA5363 // Do Not Disable Request Validation
        {
            if (ModelState.IsValid)
            {
                BlogItem blogItem = new BlogItem();

                blogItem.InjectFrom(blogFeedItemViewModel);

                blogItem.CreatedAt = DateTime.Now;
                blogItem.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);

                blogItem.Author = currentUser;

                Data.BlogItem.Add(blogItem);
                await Data.SaveChangesAsync().ConfigureAwait(false);

                return new RedirectResult(Url.Action("Index"));
            }

            return View(blogFeedItemViewModel);
        }

        // GET: BlogItemId/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int blogItemId)
        {
            BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

            BlogItem blogItem = await Data.BlogItem.Include("Author").SingleOrDefaultAsync(m => m.Id == blogItemId).ConfigureAwait(false);

            if (blogItem == null)
            {
                return HttpNotFound();
            }

            blogFeedItemViewModel.InjectFrom(blogItem);
            blogFeedItemViewModel.Author.InjectFrom(blogItem.Author);
            blogFeedItemViewModel.AuthorId = blogFeedItemViewModel.Author.Id;

            return View(blogFeedItemViewModel);
        }

        // POST: BlogItemId/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AuthorId,Title,Content,CreatedAt")] BlogFeedItemViewModel blogItemVM)
        {
            if (ModelState.IsValid)
            {
                BlogItem blogItem = new BlogItem();

                blogItem.InjectFrom(blogItemVM);
                if (blogItemVM != null)
                {
                    blogItem.Author = await Data.ApplicationUser.FindAsync(blogItemVM.AuthorId).ConfigureAwait(false);
                }
                blogItem.UpdatedAt = DateTime.Now;

                Data.Entry(blogItem).State = EntityState.Modified;
                await Data.SaveChangesAsync().ConfigureAwait(false);

                return Redirect(Url.RouteUrl(new { controller = "Blog", action = "Index"}));
            }

            return View(blogItemVM);
        }

        // GET: BlogItemId/Delete/5
        [HttpGet]
        public ActionResult Delete()
        {
            return View();
        }

        //// POST: BlogItemId/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete()
        //{
        //    // TODO: Add delete logic here
        //    return RedirectToAction("Index");
        //}
    }
}
