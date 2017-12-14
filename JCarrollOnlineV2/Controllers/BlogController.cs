using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
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
            BlogIndexViewModel blogIndexViewModel = new BlogIndexViewModel();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = await _data.ApplicationUser.FindAsync(currentUserId);
            List<BlogItem> blogItems = await _data.BlogItem.Include("BlogItemComments").ToListAsync();

            foreach(var blogItem in blogItems.OrderByDescending(m => m.UpdatedAt))
            {
                BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

                blogFeedItemViewModel.InjectFrom(blogItem);
                blogFeedItemViewModel.Author.InjectFrom(blogItem.Author);

                foreach(var blogItemComment in blogItem.BlogItemComments.ToList())
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BlogItemComment/CreateComment
        //public ActionResult CreateComment(int blogItemId, Uri returnUrl)
        //{
        //    BlogCommentItemViewModel blogCommentItemViewModel = new BlogCommentItemViewModel();

        //    blogCommentItemViewModel.BlogItemId = blogItemId;
        //    blogCommentItemViewModel.ReturnUrl = returnUrl;

        //    return View("_BlogCommentFormPartial", blogCommentItemViewModel);
        //}

        // POST: BlogItemComment/CreateComment
        //[HttpPost]
        //public void CreateComment()
        //{
        //    System.Diagnostics.Debug.WriteLine("CreateComment called");
        //}

        // POST: BlogItemComment/CreateComment
        [HttpPost]
        public void CreateComment(BlogCommentItemViewModel blogCommentItemViewModel)
        {
            if(ModelState.IsValid)
            {
                BlogItemComment blogItemComment = new BlogItemComment();

                blogItemComment.InjectFrom(blogCommentItemViewModel);
                blogItemComment.CreatedAt = DateTime.Now;

                blogItemComment.BlogItem = _data.BlogItem.Find(blogCommentItemViewModel.Id);

                _data.BlogItemComment.Add(blogItemComment);
                _data.SaveChanges();
            }
        }

        // GET: BlogItemId/Create
        public ActionResult Create()
        {
            BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

            return View(blogFeedItemViewModel);
        }

        // POST: BlogItemId/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Title,Content")]  BlogFeedItemViewModel blogFeedItemViewModel)
        {
            if (ModelState.IsValid)
            {
                BlogItem blogItem = new BlogItem();

                blogItem.InjectFrom(blogFeedItemViewModel);

                blogItem.CreatedAt = DateTime.Now;
                blogItem.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId);

                blogItem.Author = currentUser;

                _data.BlogItem.Add(blogItem);
                await _data.SaveChangesAsync();

                return new RedirectResult(Url.Action("Index"));
            }

            return View(blogFeedItemViewModel);
        }

        // GET: BlogItemId/Edit/5
        public async Task<ActionResult> Edit(int blogItemId)
        {
            BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

            var blogItem = await _data.BlogItem.Include("Author").SingleOrDefaultAsync(m => m.Id == blogItemId);

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
        public async Task<ActionResult> Edit([Bind(Include = "Id,AuthorId,Title,Content,CreatedAt")] BlogFeedItemViewModel blogItemVM)
        {
            if (ModelState.IsValid)
            {
                BlogItem blogItem = new BlogItem();

                blogItem.InjectFrom(blogItemVM);
                blogItem.Author = await _data.ApplicationUser.FindAsync(blogItemVM.AuthorId);
                blogItem.UpdatedAt = DateTime.Now;

                _data.Entry(blogItem).State = EntityState.Modified;
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
