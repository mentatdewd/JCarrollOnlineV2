using JCarrollOnlineV2.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class SandboxController : Controller
    {
        // GET: Sandbox
        public async Task<ActionResult> Index()
        {
            SandboxViewModel vm = new SandboxViewModel();

            vm.PageTitle = "Sandbox";
            return View(vm);
        }

        // GET: Sandbox/Details/5
        public async Task<ActionResult> Details(int id)
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

        // GET: Sandbox/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Sandbox/Edit/5
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

        // GET: Sandbox/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: Sandbox/Delete/5
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
