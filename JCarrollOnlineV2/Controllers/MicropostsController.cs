using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Helpers;
using JCarrollOnlineV2.ViewModels.Email;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Omu.ValueInjecter;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class MicroPostsController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        private readonly JCarrollOnlineV2DbContext _context;

        public MicroPostsController(JCarrollOnlineV2DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: MicroPosts
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await _context.MicroPost.ToListAsync().ConfigureAwait(false));
        }

        // GET: MicroPosts/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MicroPost microPost = await _context.MicroPost.FindAsync(id).ConfigureAwait(false);

            return microPost == null ? HttpNotFound() : (ActionResult)View(microPost);
        }

        // GET: MicroPosts/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: MicroPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] MicroPostCreateViewModel microPostCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                MicroPost microPost = new MicroPost();

                microPost.InjectFrom(microPostCreateViewModel);
                microPost.CreatedAt = DateTime.Now;
                microPost.UpdatedAt = DateTime.Now;

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _context.ApplicationUser.Include("Followers").FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);

                microPost.Author = currentUser;
                _context.MicroPost.Add(microPost);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                await SendMicroPostNotification(microPost, currentUser).ConfigureAwait(false);

                return RedirectToAction("Index", "Home");
            }

            return View(microPostCreateViewModel);
        }

        private async Task SendMicroPostNotification(MicroPost micropost, ApplicationUser currentUser)
        {
            foreach (ApplicationUser user in currentUser.Followers)
            {
                if (user.MicroPostEmailNotifications == true)
                {
                    MicroPostNotificationViewModel microPostNotificationEmailViewModel = GenerateViewModel(micropost, currentUser, user);
                    
                    await SendEmail(microPostNotificationEmailViewModel).ConfigureAwait(false);
                }
            }

            if (currentUser.MicroPostEmailNotifications == true)
            {
                MicroPostNotificationViewModel microPostNotificationEmailViewModel = GenerateViewModel(micropost, currentUser, currentUser);

                await SendEmail(microPostNotificationEmailViewModel).ConfigureAwait(false);
            }
        }

        private static MicroPostNotificationViewModel GenerateViewModel(MicroPost micropost, ApplicationUser currentUser, ApplicationUser user)
        {
            MicroPostNotificationViewModel microPostNotificationEmailViewModel = new MicroPostNotificationViewModel
            {
                TargetUser = user,
                MicroPostAuthor = currentUser,
                Body = micropost.Content
            };

            return microPostNotificationEmailViewModel;
        }

        private async Task SendEmail(MicroPostNotificationViewModel microPostNotificationEmailViewModel)
        {
            // Convert to anonymous object for Handlebars
            var templateData = new
            {
                TargetUser = new
                {
                    microPostNotificationEmailViewModel.TargetUser.UserName,
                    microPostNotificationEmailViewModel.TargetUser.Email
                },
                MicroPostAuthor = new
                {
                    microPostNotificationEmailViewModel.MicroPostAuthor.UserName
                },
                MicroPostContent = microPostNotificationEmailViewModel.Body
            };

            // Use Handlebars instead of RazorEngine
            microPostNotificationEmailViewModel.Body = HandlebarsEmailHelper.RenderTemplate(
                "MicropostNotificationPage", 
                templateData
            );

            await SendEmailAsync(microPostNotificationEmailViewModel).ConfigureAwait(false);
        }

        public async Task SendEmailAsync(MicroPostNotificationViewModel microPostNotificationEmailViewModel)
        {
            if (microPostNotificationEmailViewModel != null)
            {
                IdentityMessage email = new IdentityMessage()
                {
                    Body = microPostNotificationEmailViewModel.Body,
                    Destination = microPostNotificationEmailViewModel.TargetUser.Email,
                    Subject = microPostNotificationEmailViewModel.MicroPostAuthor.UserName + " has added a new micropost"
                };

                await UserManager.EmailService.SendAsync(email).ConfigureAwait(false);
            }
        }

        // GET: MicroPosts/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int? microPostId)
        {
            if (microPostId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MicroPost micropost = await _context.MicroPost.FindAsync(microPostId).ConfigureAwait(false);

            return micropost == null ? HttpNotFound() : (ActionResult)View(micropost);
        }

        // POST: MicroPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] MicroPost microPost)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(microPost).State = EntityState.Modified;
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(microPost);
        }

        // GET: MicroPosts/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(int? microPostId)
        {
            if (microPostId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MicroPost micropost = await _context.MicroPost.FindAsync(microPostId).ConfigureAwait(false);

            return micropost == null ? HttpNotFound() : (ActionResult)View(micropost);
        }

        // POST: MicroPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int microPostId)
        {
            MicroPost micropost = await _context.MicroPost.FindAsync(microPostId).ConfigureAwait(false);

            _context.MicroPost.Remove(micropost);
            await _context.SaveChangesAsync().ConfigureAwait(false);

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
