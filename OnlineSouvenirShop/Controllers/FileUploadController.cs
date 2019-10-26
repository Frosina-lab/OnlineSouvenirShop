using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Threading.Tasks;
namespace OnlineSouvenirShop.Controllers
{
    public class FileUploadController : ApiController
    {
        [Route("api/getFolderName")]
        [HttpGet]
        public string GetFolderName()
        {
            return Guid.NewGuid().ToString();
        }
        [Route("api/uploadFiles/{folder}")]
        [HttpPost]
        public async Task<string> Post(string folder)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataFiles", folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return "Not Mime Multipart Content";
                }

                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.Contents)
                {
                    var fileName = "1.jpg";
                    var fullPath = Path.Combine(path, fileName);
                    var buffer = await file.ReadAsByteArrayAsync();
                    File.WriteAllBytes(fullPath, buffer);
                }

                return folder;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/uploadFiles/{folder}")]
        [HttpGet]
        public Dictionary<string, string> GetFiles(string folder)
        {
            Dictionary<string, string> FilesList = new Dictionary<string, string>();
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataFiles", folder);
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (FileInfo fInfo in dirInfo.GetFiles())
                {
                    FilesList.Add(fInfo.Length.ToString(), fInfo.Name);
                }
            }
            catch { }
            return FilesList;
        }

        [Route("api/uploadFiles/{folder}")]
        [HttpDelete]
        public IHttpActionResult DeleteFile(string folder, [FromBody]string file)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataFiles", folder, file);
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            FileInfo fInfo = new FileInfo(path);
            if (fInfo.Exists)
                fInfo.Delete();
            return Ok();
        }

    }
}
