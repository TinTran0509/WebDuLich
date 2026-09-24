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
                    // Lưu file temp
                    string fTemp = Path.Combine(Server.MapPath("~/Upload/Temp/"), fname);
                    file.SaveAs(fTemp);

                    // Resize file temp với kích thước 1600
                    string fileResize = Path.Combine(Server.MapPath("~/Upload/Images/"), fname);

                    //using (var image = Image.FromStream(file.InputStream))
                    //{
                    //    SaveResizeJpg(image, fileResize, 1600, 80);
                    //}

                    //System.IO.File.Delete(fTemp);

                    using (var stream = ResizeAndCompressJpg(file.InputStream,1920,80))
                    {  
                        //string path = Server.MapPath("~/Uploads/" + fileName);

                        using (var fileStream = System.IO.File.Create(fileResize))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                     
                    //ResizeAndCompress(fTemp, fileResize, 1600);
                    //file.SaveAs(fileResize);
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
            return ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg");
        }

        private static void SaveResizeJpg(Image source, string path, int maxWidth, long quality)
        {
            int width = source.Width;
            int height = source.Height;

            // Chỉ resize nếu ảnh lớn hơn maxWidth
            if (width > maxWidth)
            {
                height = (int)(height * ((float)maxWidth / width));

                width = maxWidth;
            }

            using (var bitmap = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CompositingQuality =
                        CompositingQuality.HighQuality;

                    graphics.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                    graphics.SmoothingMode =
                        SmoothingMode.HighQuality;

                    graphics.PixelOffsetMode =
                        PixelOffsetMode.HighQuality;

                    graphics.DrawImage(source, 0, 0, width, height);
                }

                SaveJpg(bitmap, path, quality);
            }
        }

        private static void SaveJpg(Image image, string path, long quality)
        {
            var encoder = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg");

            using (var parameters = new EncoderParameters(1))
            {
                parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                image.Save(path, encoder, parameters);
            }
        }

        public static MemoryStream ResizeAndCompressJpg(Stream input, int targetWidth = 1920, long quality = 80)
        {
            using (var source = Image.FromStream(input))
            {
                // Tính chiều cao theo tỷ lệ
                int targetHeight = (int)Math.Round(
                    (double)source.Height / source.Width * targetWidth
                );

                using (var bitmap = new Bitmap(targetWidth, targetHeight))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(Color.White);

                        // Chất lượng resize
                        graphics.CompositingMode =
                            CompositingMode.SourceCopy;

                        graphics.CompositingQuality =
                            CompositingQuality.HighQuality;

                        graphics.InterpolationMode =
                            InterpolationMode.HighQualityBicubic;

                        graphics.SmoothingMode =
                            SmoothingMode.HighQuality;

                        graphics.PixelOffsetMode =
                            PixelOffsetMode.HighQuality;

                        graphics.DrawImage(
                            source,
                            new Rectangle(0,0,targetWidth,targetHeight)
                        );
                    }

                    // Tạo stream
                    var stream = new MemoryStream();

                    // JPEG encoder
                    var encoder = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg");

                    using (var parameters = new EncoderParameters(1))
                    {
                        parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                        bitmap.Save(stream, encoder, parameters);
                    }

                    stream.Position = 0;

                    return stream;
                }
            }
        }
    }
}