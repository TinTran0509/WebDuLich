using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace Web.Areas.Admin.Controllers
{
    public class UploadFileController : Controller
    {
        public ActionResult UploadImage(string controlName, string controlValue)
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

                    string fPath = Path.Combine(Server.MapPath("~/Upload/Images/"), fname);
                    file.SaveAs(fPath);
                    //string fileOptimize = Path.Combine(Server.MapPath("~/Upload/ImageOptimize/"), fname);
                      
                    //byte[] data = CompressJpg(fPath, 70);

                    //System.IO.File.WriteAllBytes(fileOptimize, data);

                    //file.SaveAs(fileOptimize);
                    return "/Upload/Images/" + file.FileName;
                }
            }
            return "";
        }

        public static byte[] CompressJpg(string filePath, long quality)
        {
            using (var image = Image.FromFile(filePath))
            using (var ms = new MemoryStream())
            {
                var encoder = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg");

                using (var encoderParams = new EncoderParameters(1))
                {
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality,quality);
                    image.Save(ms,encoder,encoderParams);
                } 
                return ms.ToArray();
            }
        }  
    }
}