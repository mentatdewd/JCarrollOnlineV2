using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.Controllers
{
    public class SandboxController : Controller
    {
        // GET: Sandbox
        public ActionResult Index()
        {
            SandboxViewModel vm = new SandboxViewModel();

            vm.PageTitle = "Sandbox";
            return View(vm);
        }

        // GET: Sandbox/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Sandbox/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sandbox/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
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

        // GET: Sandbox/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Sandbox/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: Sandbox/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Sandbox/Delete/5
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
