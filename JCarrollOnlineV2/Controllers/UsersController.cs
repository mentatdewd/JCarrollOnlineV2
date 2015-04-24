﻿using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private IContext _data { get; set; }

        public UsersController()
            : this(null)
        {

        }

        public UsersController(IContext dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        // GET: Users
        public async Task<ActionResult> Index()
        {
            UsersIndexViewModel vm = new UsersIndexViewModel();

            vm.PageTitle = "Users";

            var users = await _data.Users.Include("Following").Include("Followers").ToListAsync();

            foreach (var user in users)
            {
                UserItemViewModel uivm = new UserItemViewModel();
                //uivm.User = new ApplicationUserViewModel();

                uivm.User.InjectFrom(user);
                uivm.MicropostsAuthored = await _data.Users.Include("Microposts").Where(u => u.Id == user.Id).Select(u => u.Microposts).CountAsync();
                vm.Users.Add(uivm);
            }

            return View(vm);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string userId)
        {
            UserDetailViewModel vm = new UserDetailViewModel();

            vm.CurrentUser = new ApplicationUserViewModel();
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            vm.CurrentUser.InjectFrom(currentUser);

            ApplicationUser user = await _data.Users.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);
            vm.User.InjectFrom(user);

            vm.UserStatsVM = new UserStatsViewModel();
            vm.UserStatsVM.UsersFollowing = new UserFollowingViewModel();
            vm.UserStatsVM.User.InjectFrom(user);
            foreach (var item in user.Following)
            {
                UserItemViewModel auvm = new UserItemViewModel();
                auvm.User.InjectFrom(item);
                auvm.MicropostsAuthored = await _data.Users.Include("Microposts").Where(u => u.Id == item.Id).Select(u => u.Microposts).CountAsync();
                vm.UserStatsVM.UsersFollowing.Users.Add(auvm);
            }
            vm.UserStatsVM.UserFollowers = new UserFollowersViewModel();
            foreach (var item in user.Followers)
            {
                UserItemViewModel auvm = new UserItemViewModel();
                auvm.User.InjectFrom(item);
                auvm.MicropostsAuthored = await _data.Users.Include("Microposts").Where(u => u.Id == item.Id).Select(u => u.Microposts).CountAsync();
                vm.UserStatsVM.UserFollowers.Users.Add(auvm);
            }
            return View(vm);
        }

        public async Task<ActionResult> Following(string userId)
        {
            UserDetailViewModel vm = new UserDetailViewModel();
            vm.PageTitle = "Following";
            vm.UserInfoVM = new UserItemViewModel();

            ApplicationUser user = await _data.Users.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);
            vm.UserStatsVM = new UserStatsViewModel();
            vm.User.InjectFrom(user);
            vm.UserInfoVM.User.InjectFrom(user);
            vm.UserStatsVM.User.InjectFrom(user);

            foreach (var item in user.Following)
            {
                UserItemViewModel auvm = new UserItemViewModel();
                auvm.User.InjectFrom(item);
                auvm.MicropostsAuthored = item.Microposts.Count();
                vm.UserStatsVM.UsersFollowing.Users.Add(auvm);
            }
            foreach (var item in user.Followers)
            {
                UserItemViewModel uivm = new UserItemViewModel();
                uivm.InjectFrom(item);
                uivm.MicropostsAuthored = item.Microposts.Count();
                vm.UserStatsVM.UserFollowers.Users.Add(uivm);
            }
            return View("Show_Follow", vm);
        }

        public async Task<ActionResult> Followed(string userId)
        {
            UserDetailViewModel vm = new UserDetailViewModel();
            vm.PageTitle = "Followers";
            vm.UserInfoVM = new UserItemViewModel();

            ApplicationUser user = await _data.Users.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);
            vm.UserStatsVM = new UserStatsViewModel();
            vm.User.InjectFrom(user);
            vm.UserInfoVM.User.InjectFrom(user);
            vm.UserStatsVM.User.InjectFrom(user);

            foreach (var item in user.Following)
            {
                UserItemViewModel auvm = new UserItemViewModel();
                auvm.User.InjectFrom(item);
                auvm.MicropostsAuthored = item.Microposts.Count();
                vm.UserStatsVM.UsersFollowing.Users.Add(auvm);
            }
            foreach (var item in user.Followers)
            {
                UserItemViewModel uivm = new UserItemViewModel();
                uivm.User.InjectFrom(item);
                uivm.MicropostsAuthored = item.Microposts.Count();
                vm.UserStatsVM.UserFollowers.Users.Add(uivm);
            }
            return View("Show_Follow", vm);
        }

        public async Task<ActionResult> Follow([Bind(Include = "Id,UserName")]  ApplicationUserViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                var followingUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == followUser.Id);

                currentUser.Following.Add(followingUser);
                await _data.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { userid = followUser.Id });
        }

        public async Task<ActionResult> Unfollow([Bind(Include = "Id,UserName")]  ApplicationUserViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                var followingUser = await _data.Users.FirstOrDefaultAsync(x => x.Id == followUser.Id);

                currentUser.Following.Remove(followingUser);
                await _data.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { userid = followUser.Id });
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string userId)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
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
