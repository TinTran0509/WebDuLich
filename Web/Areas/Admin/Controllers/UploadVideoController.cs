using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Core;
namespace Web.Areas.Admin.Controllers
{
    public class UploadVideoController : Controller
    {
        public ActionResult UploadVideo(string controlName, string controlValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlValue = controlValue;
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
                    var arr = fname.Split('.');
                    var fFormat = arr[arr.Length-1];
                    arr = arr.Take(arr.Count() - 1).ToArray();
                    string name = string.Join("",arr); ;
                    name = HelperString.RemoveMark(name)+"."+ fFormat;
                    fname = Path.Combine(Server.MapPath("~/Upload/Videos/"), name);
                    file.SaveAs(fname);
                    return name;
                }
            }
            return "";
        }
    }
}