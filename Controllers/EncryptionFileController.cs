using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pojjaman2_EncryptionFile.Interfaces;

namespace Pojjaman2_EncryptionFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionFileController : ControllerBase
    {
        private readonly IServiceEncryptionFile _ServEncryption;
        private IWebHostEnvironment _hostingEnvironment;

        public EncryptionFileController(IServiceEncryptionFile ServEncryption, IWebHostEnvironment environment)
        {
            this._ServEncryption = ServEncryption;
            _hostingEnvironment = environment;

        }

        // GET api/encryptionfile
        [HttpGet("")]
        public ActionResult DownloadExportFileSendBank(string fileName, string extension)
        {
            string RootPath = _hostingEnvironment.ContentRootPath;
            string contentPathFile = string.Format(@"{0}{1}", RootPath, @"\FileEncryption\" + fileName + @"."+extension);
            if (System.IO.File.Exists(contentPathFile))
            {
                byte[] fileBytes = GetFile(contentPathFile);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format("{0}", fileName + @"."+ extension));
            }
            return Content("not found file");
        }

        // POST api/encryptionfile
        [HttpPost("")]
        public async Task<ActionResult> Poststring(EncryptionFile model)
        {
            try
            {
               
                string result = String.Join("\r\n", model.outputLines);
                string res = await _ServEncryption.Encryption(model.FileName, result);
                
                //string contentPath = "";
                //string path = @"\asset\Encryption\" + model.FileName + @".enc";
                //string contentPathFile = string.Format(@"{0}{1}", contentPath, path);
                //if (System.IO.File.Exists(contentPathFile))
                //{
                //    byte[] fileBytes = GetFile(contentPathFile);

                //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format("{0}", model.FileName + @".enc"));
                //}
                return Content(res);
            }
            catch (Exception ex)
            {
                return Content("not found file\n"+ ex);
            }
        }
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
            {
                throw new System.IO.IOException(s);
            }
            fs.Close();
            return data;
        }
        
    }
}