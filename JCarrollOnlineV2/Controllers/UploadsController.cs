using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class UploadsController : Controller
    {
        [HttpPost]
        [Authorize] // optional but recommended
        public async Task<ActionResult> UploadToB2()
        {
            if (Request.Files.Count == 0)
                return Json(new { error = "No file uploaded" });

            var file = Request.Files[0];

            var url = await BackblazeService.UploadAsync(file);

            return Json(new { url });
        }
    }
}
