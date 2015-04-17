using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.ViewModels;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

        // GET: Users
        public async Task<ActionResult> Index()
        {
            UsersIndexViewModel vm = new UsersIndexViewModel();
            vm.Users = new List<UserIndexItemViewModel>();

            vm.PageTitle = "Users";

            var users = await db.Users.ToListAsync();

            foreach(var user in users)
            {
                UserIndexItemViewModel uivm = new UserIndexItemViewModel();

                uivm.InjectFrom(user);
                vm.Users.Add(uivm);
            }

            return View(vm);
        }

        // Get: Users/Following/5
        public async Task<ActionResult> Following(string userId)
        {
            UsersFollowingViewModel vm = new UsersFollowingViewModel();

            return View(vm);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string userId)
        {
            UserDetailViewModel vm = new UserDetailViewModel();

            return View(vm);
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
    }
}
