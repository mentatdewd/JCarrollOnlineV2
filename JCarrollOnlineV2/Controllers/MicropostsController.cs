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
using JCarrollOnlineV2.EmailModels;
using System.ComponentModel;
using RazorEngine.Configuration;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class MicropostsController : Controller
    {
        private IContext _data { get; set; }

        public MicropostsController()
            : this(null)
        {

        }

        public MicropostsController(IContext dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        // GET: Microposts
        public async Task<ActionResult> Index()
        {
            return View(await _data.Microposts.ToListAsync());
        }

        // GET: Microposts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Micropost micropost = await _data.Microposts.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // GET: Microposts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Microposts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] MicropostCreateViewModel micropostVM)
        {
            if (ModelState.IsValid)
            {
                Micropost domModel = new Micropost();
                domModel.InjectFrom(micropostVM);
                domModel.CreatedAt = DateTime.Now;
                domModel.UpdatedAt = DateTime.Now;
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.Users.Include("Followers").FirstOrDefaultAsync(x => x.Id == currentUserId);
                domModel.Author = currentUser;
                _data.Microposts.Add(domModel);
                await _data.SaveChangesAsync();

                await SendMicropostNotification(domModel, currentUser);

                return RedirectToAction("Index", "Home");
            }
            return View(micropostVM);
        }

        private static async Task SendMicropostNotification(Micropost domModel, ApplicationUser currentUser)
        {
            //var smtpClient = new SmtpClient("localhost", 25);
            var templateFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            var templateFilePath = System.IO.Path.Combine(templateFolderPath, "MicropostNotificationPage.cshtml");
            var templateService = RazorEngineService.Create();

            foreach (var user in currentUser.Followers)
            {
                if (user.MicropostEmailNotifications == true)
                {
                    await SendEmail(domModel, currentUser, templateFilePath, templateService, user);
                }
            }
            if (currentUser.MicropostEmailNotifications == true)
            {
                await SendEmail(domModel, currentUser, templateFilePath, templateService, currentUser);
            }
        }

        private static async Task SendEmail(Micropost domModel, ApplicationUser currentUser, string templateFilePath, IRazorEngineService templateService, ApplicationUser user)
        {
            var micropostNotification = new EmailModels.MicropostNotification();

            micropostNotification.Email = user.Email;
            micropostNotification.Name = user.UserName;
            micropostNotification.Author = currentUser.UserName;
            micropostNotification.Content = domModel.Content;

            var emailHtmlBody = templateService.RunCompile(System.IO.File.ReadAllText(templateFilePath), "templatekey", null, micropostNotification);

            var email = new IdentityMessage()
            {
                Body = emailHtmlBody,
                Destination = user.Email,
                Subject = currentUser.UserName + " has added a new micropost"
            };
            var emailService = new EmailService();

            await emailService.SendAsync(email);
        }

        // GET: Microposts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Micropost micropost = await _data.Microposts.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // POST: Microposts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,UserId,CreatedAt,UpdatedAt")] Micropost micropost)
        {
            if (ModelState.IsValid)
            {
                _data.Entry(micropost).State = EntityState.Modified;
                await _data.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(micropost);
        }

        // GET: Microposts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Micropost micropost = await _data.Microposts.FindAsync(id);
            if (micropost == null)
            {
                return HttpNotFound();
            }
            return View(micropost);
        }

        // POST: Microposts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Micropost micropost = await _data.Microposts.FindAsync(id);
            _data.Microposts.Remove(micropost);
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
