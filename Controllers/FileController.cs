//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Net.Http.Headers;
//using UploadApplication;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyWebsite.Controllers
{
    [Route("api/[controller]s")]
    public class FileController : Controller
    {
        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"}
        };
        private readonly string _folder;
        //private IWebHostEnvironment hostingEnv;
        public FileController(IWebHostEnvironment env)
        {
            // 把上傳目錄設為：wwwroot\UploadFolder
            _folder = $@"{env.WebRootPath}\UploadFolder";
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var size = files.Sum(f => f.Length);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var path = $@"{_folder}\{file.FileName}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { count = files.Count, size });
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var path = $@"{_folder}\{fileName}";
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);

            // 回傳檔案到 Client 需要附上 Content Type，否則瀏覽器會解析失敗。
            return new FileStreamResult(memoryStream, _contentTypes[Path.GetExtension(path).ToLowerInvariant()]);
        }

    }
}







//namespace UploadApplication.Controllers
//{
////[Produces("application/json")]
//[Route("api/[controller]")]
//[EnableCors("AllowSpecificOrigin")]

//	public class FileController : Controller
//	{
//        private readonly string suffix;
//        private IHostingEnvironment hostingEnv;
//		public FileController(IHostingEnvironment env)
//		{
//			this.hostingEnv = env;
//		}

//		[HttpPost]
//		public IActionResult Post()
//		{
//			var files = Request.Form.Files;
//			long size = files.Sum(f => f.Length);
//			//size > 10MB refuse upload

//			if (size > 10485760)
//			{
//				return Json(Return_Helper.Error_Msg_Ecode_Elevel_HttpCode("Files total size over 10MB , server refused! ,Please try again"));
//			}
//			List<String> filePathResultList = new List<String>();

//			foreach (var file in files) {
//				var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim();
//				string filePath = hostingEnv.WebRootPath;

//				if (!Directory.Exists(filePath)) {
//					Directory.CreateDirectory(filePath);
//				}

//				fileName = Guid.NewGuid()+"."+suffix;
//				string fileFullName = filePath+fileName;
//				using (FileStream fs = System.IO.File.Create(fileFullName))
//				{
//					file.CopyTo(fs);
//					fs.Flush();
//				}
//				filePathResultList.Add($"/src/Pictures/{fileName}");
//			}
//			string message = $"{files.Count} file(s) /{size} bytes uploaded successfully!";
//			return Json(Return_Helper.Success_Msg_Data_DCount_HttpCode(message, filePathResultList, filePathResultList.Count));
//		}

//	}

//}
