using JCarrollOnlineV2.ViewModels.Sandbox;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class SandboxController : Controller
    {
        // GET: Sandbox
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            SandboxViewModel sandboxViewModel = new SandboxViewModel
            {
                PageTitle = "Sandbox"
            };

            return await Task.Run<ActionResult>(() =>
            {
                return View(sandboxViewModel);
            }).ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<ActionResult> YellowStoneSlideShow()
        {
            YellowstoneViewModel yellowstoneViewModel = new YellowstoneViewModel();

            string relativeImagePath = ControllerContext.HttpContext.Server.MapPath("~/Content/images/yellowstone");
            IEnumerable<string> imageFiles = Directory.EnumerateFiles(relativeImagePath, "*.jpg");
            string uri = Request.Url.AbsoluteUri;
            string[] uriParts = uri.Split('/');
            string baseUri = uriParts[0] + "//" + uriParts[2] + "/content/images/yellowstone/";

            foreach(string imageFile in imageFiles)
            {
                yellowstoneViewModel.AddImageFile(new ImageFileMetadata()
                {
                    Path = baseUri + Path.GetFileName(imageFile),
                    Caption = "",
                    AltString = Path.GetFileNameWithoutExtension(imageFile)
                });
            }

            yellowstoneViewModel.PageTitle = "Yellowstone Slideshow";
            return await Task.Run<ActionResult>(() =>
            {
                return View(yellowstoneViewModel);
            }).ConfigureAwait(false);
        }

        // GET: Sandbox/Details/5
        [HttpGet]
        public async Task<ActionResult> Details()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        // GET: Sandbox/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        // POST: Sandbox/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create()
        //{
        //    return await Task.Run<ActionResult>(() =>
        //    {
        //        // TODO: Add insert logic here
        //        return RedirectToAction("Index");
        //    }).ConfigureAwait(false);
        //}

        // GET: Sandbox/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        //// POST: Sandbox/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit()
        //{
        //    return await Task.Run<ActionResult>(() =>
        //    {
        //        // TODO: Add update logic here
        //        return RedirectToAction("Index");
        //    }).ConfigureAwait(false);
        //}

        // GET: Sandbox/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        // POST: Sandbox/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Delete()
        //{
        //    return await Task.Run<ActionResult>(() =>
        //    {
        //        try
        //        {
        //            // TODO: Add delete logic here
        //            return RedirectToAction("Index");
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }).ConfigureAwait(false);
        //}
    }
}
