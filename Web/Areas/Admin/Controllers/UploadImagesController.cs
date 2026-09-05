using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    public class UploadImagesController : Controller
    {
        public ActionResult UploadImages()
        {
            return PartialView();
        }
        public string ProcessUpload()
        {
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname;
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }
                    fname = Path.Combine(Server.MapPath("~/Upload/Images/"), fname);
                    file.SaveAs(fname);
                    return "/Upload/Images/" + file.FileName;
                }
            }
            return "";
        }

        [HttpGet]
        public void DeleteImg(string fileImg)
        {
            var arr = fileImg.Split('/');
            var fileName = arr[arr.Length - 1];
            string fname = Path.Combine(Server.MapPath("~/Upload/Images/"), fileName);
            System.IO.File.Delete(fname);
        }
    }
}