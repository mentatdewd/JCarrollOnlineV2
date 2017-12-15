using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.EmailViewModels;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using RazorEngine.Templating;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class MicroPostsController : Controller
    {
        private JCarrollOnlineV2DbContext _data { get; set; }

        public MicroPostsController()
            : this(null)
        {

        }

        public MicroPostsController(JCarrollOnlineV2DbContext dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2DbContext();
        }

        // GET: MicroPosts
        public async Task<ActionResult> Index()
        {
            return View(await _data.MicroPost.ToListAsync());
        }

        // GET: MicroPosts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MicroPost microPost = await _data.MicroPost.FindAsync(id);

            if (microPost == null)
            {
                return HttpNotFound();
            }

            return View(microPost);
        }

        // GET: MicroPosts/Create
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
                ApplicationUser currentUser = await _data.ApplicationUser.Include("Followers").FirstOrDefaultAsync(x => x.Id == currentUserId);

                microPost.Author = currentUser;
                _data.MicroPost.Add(microPost);
                await _data.SaveChangesAsync();

                await SendMicroPostNotification(microPost, currentUser);

                return RedirectToAction("Index", "Home");
            }

            return View(microPostCreateViewModel);
        }

        private static async Task SendMicroPostNotification(MicroPost micropost, ApplicationUser currentUser)
        {
            foreach (var user in currentUser.Followers)
            {
                if (user.MicroPostEmailNotifications == true)
                {
                    var microPostNotificationEmailViewModel = GenerateViewModel(micropost, currentUser, user);
                    
                    await SendEmail(microPostNotificationEmailViewModel);
                }
            }

            if (currentUser.MicroPostEmailNotifications == true)
            {
                var microPostNotificationEmailViewModel = GenerateViewModel(micropost, currentUser, currentUser);

                await SendEmail(microPostNotificationEmailViewModel);
            }
        }

        private static MicroPostNotificationEmailViewModel GenerateViewModel(MicroPost micropost, ApplicationUser currentUser, ApplicationUser user)
        {
            var microPostNotificationEmailViewModel = new MicroPostNotificationEmailViewModel
            {
                TargetUser = new ApplicationUserViewModel()
            };

            microPostNotificationEmailViewModel.TargetUser.InjectFrom(user);
            microPostNotificationEmailViewModel.MicroPostAuthor = new ApplicationUserViewModel();
            microPostNotificationEmailViewModel.MicroPostAuthor.InjectFrom(currentUser);
            microPostNotificationEmailViewModel.MicroPostContent = micropost.Content;

            return microPostNotificationEmailViewModel;
        }

        private static async Task SendEmail(MicroPostNotificationEmailViewModel microPostNotificationEmailViewModel)
        {

            var templateFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            var templateFilePath = System.IO.Path.Combine(templateFolderPath, "MicroPostNotificationPage.cshtml");
            var templateService = RazorEngineService.Create();

            microPostNotificationEmailViewModel.Content = templateService.RunCompile(System.IO.File.ReadAllText(templateFilePath), "microPostTemplatekey", null, microPostNotificationEmailViewModel);

            await SendEmailAsync(microPostNotificationEmailViewModel);
        }

        public static async Task SendEmailAsync(MicroPostNotificationEmailViewModel microPostNotificationEmailViewModel)
        {
            var email = new IdentityMessage()
            {
                Body = microPostNotificationEmailViewModel.Content,
                Destination = microPostNotificationEmailViewModel.TargetUser.Email,
                Subject = microPostNotificationEmailViewModel.MicroPostAuthor.UserName + " has added a new micropost"
            };
            
            var emailService = new EmailService();

            await emailService.SendAsync(email);
        }

        // GET: MicroPosts/Edit/5
        public async Task<ActionResult> Edit(int? microPostId)
        {
            if (microPostId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MicroPost micropost = await _data.MicroPost.FindAsync(microPostId);

            if (micropost == null)
            {
                return HttpNotFound();
            }

            return View(micropost);
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
                _data.Entry(microPost).State = EntityState.Modified;
                await _data.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(microPost);
        }

        // GET: MicroPosts/Delete/5
        public async Task<ActionResult> Delete(int? microPostId)
        {
            if (microPostId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MicroPost micropost = await _data.MicroPost.FindAsync(microPostId);

            if (micropost == null)
            {
                return HttpNotFound();
            }

            return View(micropost);
        }

        // POST: MicroPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int microPostId)
        {
            MicroPost micropost = await _data.MicroPost.FindAsync(microPostId);

            _data.MicroPost.Remove(micropost);
            await _data.SaveChangesAsync();

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
