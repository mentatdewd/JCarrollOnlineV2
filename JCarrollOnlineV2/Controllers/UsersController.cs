using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private JCarrollOnlineV2DbContext Data { get; set; }

        public UsersController()
            : this(null)
        {

        }

        public UsersController(JCarrollOnlineV2DbContext dataContext)
        {
            Data = dataContext ?? new JCarrollOnlineV2DbContext();
        }

        // GET: Users
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            UsersIndexViewModel usersIndexViewModel = new UsersIndexViewModel
            {
                PageTitle = "Users"
            };

            System.Collections.Generic.List<ApplicationUser> users = await Data.ApplicationUser.Include("Following").Include("Followers").ToListAsync().ConfigureAwait(false);

            foreach (ApplicationUser user in users)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                userItemViewModel.User.InjectFrom(user);
                userItemViewModel.MicroPostsAuthored = await Data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == user.Id).Select(u => u.MicroPosts).CountAsync().ConfigureAwait(false);
                usersIndexViewModel.Users.Add(userItemViewModel);
            }

            return View(usersIndexViewModel);
        }

        // POST: Users/SendMassEmail
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMassEmail(UsersIndexViewModel model)
        {
            // Validate only the email-related fields
            ModelState.Remove("Users");

            if (string.IsNullOrWhiteSpace(model.EmailSubject) || string.IsNullOrWhiteSpace(model.EmailBody))
            {
                TempData["Error"] = "Please provide both a subject and message.";
                return RedirectToAction("Index");
            }

            try
            {
                System.Collections.Generic.List<ApplicationUser> users = await Data.ApplicationUser
                    .Where(u => !string.IsNullOrEmpty(u.Email))
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (!users.Any())
                {
                    TempData["Error"] = "No users with email addresses found.";
                    return RedirectToAction("Index");
                }

                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
                string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                string smtpFromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
                bool smtpEnableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");

                string emailBody = BuildEmailBody(model.EmailSubject, model.EmailBody, model.IsHtml);
                int sentCount = 0;
                int failedCount = 0;

                using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                    smtp.EnableSsl = smtpEnableSsl;

                    foreach (ApplicationUser user in users)
                    {
                        try
                        {
                            MailMessage message = new MailMessage
                            {
                                From = new MailAddress(smtpFromEmail, "Administrator - JCarrollOnline"),
                                Subject = model.EmailSubject,
                                Body = emailBody,
                                IsBodyHtml = model.IsHtml
                            };

                            message.To.Add(user.Email);

                            await smtp.SendMailAsync(message).ConfigureAwait(false);
                            sentCount++;
                        }
                        catch (Exception ex)
                        {
                            failedCount++;
                            _logger.Error(ex, string.Format("Failed to send email to {0}", user.Email));
                        }
                    }
                }

                TempData["Success"] = string.Format("Mass email sent successfully to {0} users. {1}",
                    sentCount,
                    failedCount > 0 ? failedCount + " emails failed." : "");
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending mass email");
                TempData["Error"] = "Error sending mass email: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private string BuildEmailBody(string subject, string body, bool isHtml)
        {
            if (!isHtml)
            {
                return body;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            sb.AppendLine(".header { background-color: #4CAF50; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine(".content { padding: 20px; }");
            sb.AppendLine(".footer { background-color: #f1f1f1; padding: 10px; text-align: center; font-size: 12px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<h1>JCarrollOnline</h1>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='content'>");
            sb.AppendLine(body);
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>This message was sent by the Administrator of JCarrollOnline</p>");
            sb.AppendLine(string.Format("<p>&copy; {0} JCarrollOnline. All rights reserved.</p>", DateTime.Now.Year));
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        // GET: Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();

            string currentUserId = User.Identity.GetUserId();
            
            if(userId == null)
            {
                userId = currentUserId;
            }

            ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
            ApplicationUser user = await Data.ApplicationUser.Include("Following").Include("Followers").FirstOrDefaultAsync(m => m.Id == userId).ConfigureAwait(false);

            if (user != null)
            {
                userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);

                userDetailViewModel.UserInfoViewModel.MicroPostEmailNotifications = user.MicroPostEmailNotifications;
                //udVM.UserInfoVM.MicroPostSMSNotifications = user.MicroPostSMSNotifications;
                userDetailViewModel.UserInfoViewModel.UserId = currentUserId;

                userDetailViewModel.UserStatsViewModel = new UserStatsViewModel
                {
                    UsersFollowing = new UserFollowingViewModel()
                };

                userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

                foreach (ApplicationUser following in user.Following)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                    userItemViewModel.User.InjectFrom(following);
                    userItemViewModel.UserId = following.Id;
                    userItemViewModel.MicroPostsAuthored = await Data.MicroPost.Where(u => u.Author.Id == following.Id).CountAsync().ConfigureAwait(false);
                    userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
                }

                userDetailViewModel.UserStatsViewModel.UserFollowers = new UserFollowersViewModel();

                foreach (ApplicationUser follower in user.Followers)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                    userItemViewModel.User.InjectFrom(follower);
                    userItemViewModel.UserId = follower.Id;
                    userItemViewModel.MicroPostsAuthored = await Data.MicroPost.Where(u => u.Author.Id == follower.Id).CountAsync().ConfigureAwait(false);
                    userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
                }
            }
            return View(userDetailViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Following(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel
            {
                PageTitle = "Following",
                UserInfoViewModel = new UserItemViewModel(_logger)
            };

            ApplicationUser user = await Data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId).ConfigureAwait(false);

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.User.InjectFrom(user);
            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (ApplicationUser following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);
                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = following.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            foreach (ApplicationUser follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);
                userItemViewModel.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = follower.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
            }

            return View("Show_Follow", userDetailViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Followed(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel
            {
                PageTitle = "Followers",
                UserInfoViewModel = new UserItemViewModel(_logger)
            };

            ApplicationUser user = await Data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId).ConfigureAwait(false);

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.User.InjectFrom(user);
            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (ApplicationUser following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = following.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            foreach (ApplicationUser follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                userItemViewModel.User.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = follower.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
            }

            return View("Show_Follow", userDetailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Follow([Bind(Include = "UserId")]  UserItemViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
                ApplicationUser followingUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == followUser.UserId).ConfigureAwait(false);

                currentUser.Following.Add(followingUser);
                await Data.SaveChangesAsync().ConfigureAwait(false);
            }

            return RedirectToAction("Details", new { userid = followUser?.UserId });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unfollow")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unfollow([Bind(Include = "UserId")]  UserItemViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
                ApplicationUser followingUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == followUser.UserId).ConfigureAwait(false);

                currentUser.Following.Remove(followingUser);
                await Data.SaveChangesAsync().ConfigureAwait(false);
            }

            return RedirectToAction("Details", new { userid = followUser?.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserSettings([Bind(Include = "UserId,MicroPostEmailNotifications,MicroPostSmsNotifications")] UserItemViewModel userItemViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == userItemViewModel.UserId).ConfigureAwait(false);

                if (userItemViewModel != null)
                {
                    user.MicroPostEmailNotifications = userItemViewModel.MicroPostEmailNotifications;
                    //user.MicroPostSMSNotifications = auVM.MicroPostSMSNotifications;
                    await Data.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return RedirectToAction("Details", new { userid = userItemViewModel?.UserId });
        }

        // GET: Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);

        }

        // GET: Users/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        // GET: Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
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
