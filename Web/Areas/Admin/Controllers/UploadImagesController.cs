using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
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
                    string fileOptimize = string.Empty;
                    ResizeAndCompress(fname, fileOptimize, 500);
                    file.SaveAs(fileOptimize);
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

        public static void ResizeAndCompress(string sourcePath, string outputPath, int height, long quality = 80)
        {
            using (var source = Image.FromFile(sourcePath))
            {
                int width = (int)(source.Width * ((float)height / source.Height));

                using (var bitmap = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;

                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        graphics.SmoothingMode = SmoothingMode.HighQuality;

                        graphics.DrawImage(source,0,0,width,height);
                    }

                    var encoder = GetJpegEncoder();

                    using (var parameters = new EncoderParameters(1))
                    {
                        parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                        bitmap.Save(outputPath,encoder,parameters);
                    }
                }
            }
        }

        private static ImageCodecInfo GetJpegEncoder()
        {
            return ImageCodecInfo.GetImageEncoders()
                .First(x => x.MimeType == "image/jpeg");
        }
    }
}