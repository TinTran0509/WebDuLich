using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Web.Models;

namespace Web.Commons
{
    public static class ImageHelper
    {
        /// <summary>
        /// Upload ảnh và tạo thumbnail + detail
        /// </summary>
        public static ImageUploadResult SaveImage(HttpPostedFileBase file, string folderPath, string fileName)
        {
            if (file == null || file.ContentLength <= 0)
                throw new Exception("File không hợp lệ.");

            if (!file.ContentType.StartsWith("image/"))
                throw new Exception("File không phải hình ảnh.");

            Directory.CreateDirectory(folderPath);

            using (var image = Image.FromStream(file.InputStream))
            {
                // Thumbnail
                string thumbnailName = fileName + "_thumb.jpg";
                string thumbnailPath = Path.Combine(folderPath, thumbnailName);

                SaveResizeJpg(image, thumbnailPath, 600, 75);

                // Detail
                string detailName = fileName + ".jpg";
                string detailPath = Path.Combine(folderPath, detailName);

                SaveResizeJpg(image, detailPath, 1600, 80);

                return new ImageUploadResult
                {
                    Thumbnail = thumbnailName,
                    Detail = detailName
                };
            }
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
    }
}