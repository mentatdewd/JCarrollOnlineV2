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

namespace JCarrollOnlineV2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BlogController : Controller
    {
        private IContext _data { get; set; }

        public BlogController()
            : this(null)
        {

        }

        public BlogController(IContext dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        // GET: Blog
        public async Task<ActionResult> Index()
        {
            BlogIndexViewModel bivm = new BlogIndexViewModel();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = await _data.Users.Include("BlogItems").SingleAsync(u => u.Id == currentUserId);

            foreach(var item in user.BlogItems.OrderByDescending(m => m.UpdatedAt))
            {
                BlogFeedItemViewModel bfi = new BlogFeedItemViewModel();
                bfi.InjectFrom(item);
                bfi.Author.InjectFrom(item.Author);
                bivm.BlogFeedItems.BlogFeedItemVMs.Add(bfi);               
            }
            return View(bivm);
        }

        // GET: Blog/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Blog/Create
        public ActionResult Create()
        {
            BlogFeedItemViewModel bfiVM = new BlogFeedItemViewModel();

            return View(bfiVM);
        }

        // POST: Blog/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Title,Content")]  BlogFeedItemViewModel blogFeedItemViewModel)
        {
            if (ModelState.IsValid)
            {
                Blog blogPost = new Blog();

                blogPost.InjectFrom(blogFeedItemViewModel);

                blogPost.CreatedAt = DateTime.Now;
                blogPost.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                blogPost.Author = currentUser;

                _data.BlogItems.Add(blogPost);
                await _data.SaveChangesAsync();

                return new RedirectResult(Url.Action("Index"));
            }

            return View(blogFeedItemViewModel);
        }

        // GET: Blog/Edit/5
        public async Task<ActionResult> Edit(int blogItemId)
        {
            BlogFeedItemViewModel vm = new BlogFeedItemViewModel();

            var blogItem = await _data.BlogItems.Include("Author").SingleOrDefaultAsync(m => m.Id == blogItemId);

            if (blogItem == null)
            {
                return HttpNotFound();
            }
            vm.InjectFrom(blogItem);
            vm.Author.InjectFrom(blogItem.Author);

            return View(vm);
        }

        // POST: Blog/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AuthorId,Title,Content,CreatedAt")] BlogFeedItemViewModel blogItemVM)
        {
            if (ModelState.IsValid)
            {
                Blog domModel = new Blog();

                domModel.InjectFrom(blogItemVM);
                domModel.Author = await _data.Users.FindAsync(blogItemVM.Author.Id);
                domModel.UpdatedAt = DateTime.Now;

                _data.Entry(domModel).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return Redirect(Url.RouteUrl(new { controller = "Blog", action = "Index"}));
            }
            return View(blogItemVM);
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Blog/Delete/5
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
