using NLog;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class UploadsController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        [Authorize] // optional but recommended
        public async Task<ActionResult> UploadToB2()
        {
            if (Request.Files.Count == 0)
                return Json(new { error = "No file uploaded" });

            HttpPostedFileBase file = Request.Files[0];

            _logger.Info($"Uploading file {file.FileName} of size {file.ContentLength} bytes to Backblaze B2");
            string url = await BackblazeService.UploadAsync(file);

            _logger.Info($"File uploaded successfully. Accessible at {url}");
            return Json(new { url });
        }
    }
}
