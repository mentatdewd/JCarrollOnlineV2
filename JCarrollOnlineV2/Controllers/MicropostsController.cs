using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using System;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using RazorEngine.Templating;
using System.ComponentModel;
using RazorEngine.Configuration;
using JCarrollOnlineV2.EmailViewModels;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class MicroPostsController : Controller
    {
        private IJCarrollOnlineV2Context _data { get; set; }

        public MicroPostsController()
            : this(null)
        {

        }

        public MicroPostsController(IJCarrollOnlineV2Context dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] MicroPostCreateViewModel micropostVM)
        {
            if (ModelState.IsValid)
            {
                MicroPost microPost = new MicroPost();
                microPost.InjectFrom(micropostVM);
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
            return View(micropostVM);
        }

        private static async Task SendMicroPostNotification(MicroPost micropost, ApplicationUser currentUser)
        {
            foreach (var user in currentUser.Followers)
            {
                if (user.MicroPostEmailNotifications == true)
                {
                    var mneVM = GenerateViewModel(micropost, currentUser, user);
                    
                    await SendEmail(mneVM);
                }
            }
            if (currentUser.MicroPostEmailNotifications == true)
            {
                var mneVM = GenerateViewModel(micropost, currentUser, currentUser);
                await SendEmail(mneVM);
            }
        }

        private static MicroPostNotificationEmailViewModel GenerateViewModel(MicroPost micropost, ApplicationUser currentUser, ApplicationUser user)
        {
            var mneVM = new MicroPostNotificationEmailViewModel();

            mneVM.TargetUser = new ApplicationUserViewModel();
            mneVM.TargetUser.InjectFrom(user);
            mneVM.MicroPostAuthor = new ApplicationUserViewModel();
            mneVM.MicroPostAuthor.InjectFrom(currentUser);
            mneVM.MicroPostContent = micropost.Content;
            return mneVM;
        }

        private static async Task SendEmail(MicroPostNotificationEmailViewModel mneVM)
        {

            var templateFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            var templateFilePath = System.IO.Path.Combine(templateFolderPath, "MicroPostNotificationPage.cshtml");
            var templateService = RazorEngineService.Create();
            mneVM.Content = templateService.RunCompile(System.IO.File.ReadAllText(templateFilePath), "microPostTemplatekey", null, mneVM);

            await SendEmailAsync(mneVM);
        }

        public static async Task SendEmailAsync(MicroPostNotificationEmailViewModel mneVM)
        {
            var email = new IdentityMessage()
            {
                Body = mneVM.Content,
                Destination = mneVM.TargetUser.Email,
                Subject = mneVM.MicroPostAuthor.UserName + " has added a new micropost"
            };
            var emailService = new EmailService();

            await emailService.SendAsync(email);
        }

        // GET: MicroPosts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MicroPost micropost = await _data.MicroPost.FindAsync(id);
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] MicroPost micropost)
        {
            if (ModelState.IsValid)
            {
                _data.Entry(micropost).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(micropost);
        }

        // GET: MicroPosts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MicroPost micropost = await _data.MicroPost.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // POST: MicroPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MicroPost micropost = await _data.MicroPost.FindAsync(id);
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
