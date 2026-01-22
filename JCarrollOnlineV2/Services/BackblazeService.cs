using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public static class BackblazeService
{
    private static readonly string KeyId = ConfigurationManager.AppSettings["B2KeyId"];
    private static readonly string AppKey = ConfigurationManager.AppSettings["B2AppKey"];
    private static readonly string BucketId = ConfigurationManager.AppSettings["B2BucketId"];
    private static readonly string BucketName = ConfigurationManager.AppSettings["B2BucketName"];

    public static async Task<string> UploadAsync(HttpPostedFileBase file)
    {
        if (file == null || file.ContentLength == 0)
            throw new Exception("No file provided.");

        // Debug: confirm config loaded
        System.Diagnostics.Debug.WriteLine($"[B2] KeyId: {KeyId}");
        System.Diagnostics.Debug.WriteLine($"[B2] AppKey length: {AppKey?.Length}");

        if (string.IsNullOrWhiteSpace(KeyId) || string.IsNullOrWhiteSpace(AppKey))
            throw new Exception("Backblaze KeyId or AppKey missing from Web.config");

        // 1. Authorize account
        var basicAuth = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{KeyId}:{AppKey}")
        );

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", basicAuth);

            var authResponse = await client.GetAsync(
                "https://api.backblazeb2.com/b2api/v2/b2_authorize_account"
            );

            authResponse.EnsureSuccessStatusCode();

            dynamic authJson = await authResponse.Content.ReadAsAsync<dynamic>();

            string apiUrl = authJson.apiUrl;
            string authToken = authJson.authorizationToken;

            System.Diagnostics.Debug.WriteLine($"[B2] Authorized. apiUrl={apiUrl}");

            // 2. Get upload URL
            var uploadUrlRequest = new { bucketId = BucketId };

            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authToken);

            var uploadUrlResponse = await client.PostAsJsonAsync(
                $"{apiUrl}/b2api/v2/b2_get_upload_url",
                uploadUrlRequest
            );

            uploadUrlResponse.EnsureSuccessStatusCode();

            dynamic uploadUrlJson = await uploadUrlResponse.Content.ReadAsAsync<dynamic>();

            string uploadUrl = uploadUrlJson.uploadUrl;
            string uploadAuthToken = uploadUrlJson.authorizationToken;

            System.Diagnostics.Debug.WriteLine($"[B2] Upload URL acquired.");

            // 3. Upload file
            string safeFileName = MakeSafeFilename(file.FileName);

            using (var fileStream = file.InputStream)
            {
                byte[] sha1 = ComputeSha1(fileStream);
                fileStream.Position = 0;

                var content = new StreamContent(fileStream);
                content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Headers.Add("X-Bz-File-Name", safeFileName);
                content.Headers.Add("X-Bz-Content-Sha1", BitConverter.ToString(sha1).Replace("-", "").ToLower());

                var uploadClient = new HttpClient();
                uploadClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", uploadAuthToken);

                var uploadResponse = await uploadClient.PostAsync(uploadUrl, content);
                uploadResponse.EnsureSuccessStatusCode();

                System.Diagnostics.Debug.WriteLine($"[B2] Upload complete: {safeFileName}");
            }
            //_logger.LogInformation("Uploaded file: {FileName}", safeFileName);

            // 4. Return public URL
            //string endpoint = "https://s3.us-east-005.backblazeb2.com";
            //return $"![image]({endpoint}/file/{BucketName}/{safeFileName})";
            return $"![image](https://JCarrollOnline.s3.us-east-005.backblazeb2.com/{safeFileName})";
        }
    }

    private static string MakeSafeFilename(string original)
    {
        string name = Path.GetFileName(original);
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        return $"{timestamp}_{name.Replace(" ", "_")}";
    }

    private static byte[] ComputeSha1(Stream stream)
    {
        using (var sha1 = SHA1.Create())
        {
            return sha1.ComputeHash(stream);
        }
    }
}
