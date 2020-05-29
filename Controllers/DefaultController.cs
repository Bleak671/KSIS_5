using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApplication1.Controllers
{
    [Controller]
    [Route("")]
    public class DefaultController : Controller
    {
        private string root = @"E:/4sem/WebApplication1/WebApplication1/wwwroot";

        [HttpPut("{*filename}")]
        public ActionResult Put(IFormFileCollection inputFile, string filename)
        {
            string fullpath = root + @"/" + filename;
            try
            {
                using (var fileStream = new FileStream(root + @"/" + filename, FileMode.Create))
                {
                    inputFile[0].CopyTo(fileStream);
                }
                return Ok("Записано!");
            }
            catch { return NotFound(); }
        }

        [HttpGet()]
        [HttpGet("{*filename}")]
        public ActionResult GetFile(string filename)
        {
            string fullpath = root + @"/" + filename;
            if (isFile(filename))
            {
                if (!System.IO.File.Exists(fullpath))
                    return NotFound();
                try
                {
                    FileStream fs = new FileStream(fullpath, FileMode.Open);
                    return File(fs, "application/unknown", rightVersion(filename));
                }
                catch { return BadRequest(); }
            }
            else
            {
                try
                {
                    IReadOnlyCollection<string> fileColl = Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(fullpath);
                    IReadOnlyCollection<string> dirColl = Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(fullpath);
                    List<string> allFiles = new List<string>(dirColl);
                    allFiles.AddRange(fileColl);
                    return Ok(allFiles);
                }
                catch { return BadRequest(); }

            }
        }

        [HttpHead("{*filename}")]
        public ActionResult GetFileInfo(string filename)
        {
            string fullpath = root + @"/" + filename;
            if (!System.IO.File.Exists(fullpath))
                return NotFound();
            try
            {
                FileInfo fileInfo = Microsoft.VisualBasic.FileIO.FileSystem.GetFileInfo(fullpath);
                Response.Headers.Append("Full name",fileInfo.FullName);
                Response.Headers.Append("Length", fileInfo.Length.ToString());
                return Ok();
            }
            catch { return NotFound(); }
        }

        [HttpDelete("{*filename}")]
        public ActionResult DeleteFile(string filename)
        {
            string fullpath = root + @"/" + filename;
            if (!System.IO.File.Exists(fullpath))
                return NotFound();
            try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(fullpath);
                return Ok("Удалено!");
            }
            catch { return NotFound(); }
        }

        private bool isFile(string str)
        {
            try
            {
                if (str == null)
                    return false;
                int index = str.LastIndexOf(@"/") + 1;
                string substr = str.Substring(index, str.Length - index);
                if ((substr.Contains(".")) && (substr.IndexOf(".") == substr.LastIndexOf(".")))
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        private string rightVersion(string str)
        {
            if (str == null)
                return str;
            int index = str.LastIndexOf(@"/") + 1;
            string substr = str.Substring(index, str.Length - index);
            return substr;
        }
    }
}