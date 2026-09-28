using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class VideoController : BaseController
    {
        readonly IUserAdminRepository _userAdminRepository = new UserAdminRepository();
        readonly IIntroductionRepository introductionRepository = new IntroductionRepository();
        //
        // GET: /Admin/Video/

        public JsonResult Upload()
        {
            if (Request.Files.Count <= 0)
                return Json(new { status = false, msg = "Bạn chưa chọn file." });
            var videoExtention = new[] { "avi", "mp4", "flv", "wmv", "mov" };
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
            double maxFileSize = 4 * 2048 * 2048;
            if (section != null)
            {
                maxFileSize = section.MaxRequestLength;
            }
            var now = DateTime.Now;
            var objInfo = new FileInfo();
            try
            {
                //  Get all files from Request object  
                var files = Request.Files[0];
                var path = ConfigUpload.TargetUpload;
                var fileData = files;
                var extention = GetExtention(fileData.FileName);
                if (!videoExtention.Contains(extention))
                {
                    return Json(new { status = false, msg = "Video upload không đúng định dạng cho phép." });
                }
                #region KIỂM TRA KÍCH THƯỚC FILE
                var fileSize = fileData.ContentLength;
                if (fileSize > (maxFileSize))
                {
                    return Json(new { status = false, msg = "tập tin này vượt quá dung lượng cho phép" });
                }
                if (fileSize == 0)
                {
                    return Json(new { status = false, msg = "kiểm tra lại tập tin này" });
                }
                #endregion
                #region TẠO THƯ MỤC CHỨA FILES UPLOAD
                path = string.Format("{0}/{1}", path, "Videos");
                if (!Directory.Exists(Server.MapPath(path)))
                {
                    Directory.CreateDirectory(Server.MapPath(path));
                }
                //path = string.Format("{0}/{1}/{2}", path, now.Year, now.Month);
                //if (!Directory.Exists(Server.MapPath(path)))
                //{
                //    Directory.CreateDirectory(Server.MapPath(path));
                //}
                path = string.Format("{0}/{1}", path, "Introduct");
                if (!Directory.Exists(Server.MapPath(path)))
                {
                    Directory.CreateDirectory(Server.MapPath(path));
                }
                #endregion
                #region COPY FILE VÀO THƯ MỤC

                var newName = string.Format("{0}-{1}", HelperEncryptor.Md5Hash(DateTime.Now.ToString()), fileData.FileName);
                fileData.SaveAs(string.Format("{0}/{1}", Server.MapPath(path), newName));
                #endregion
                objInfo = new FileInfo
                {
                    FileName = string.Format("{0}/{1}", path, newName),
                    FileNameOriginal = fileData.FileName,
                    FileSize = fileData.ContentLength < 1024 ? string.Format("{0} Bytes", (fileData.ContentLength)) : string.Format("{0} KB", (fileData.ContentLength / 1024))
                };
            }
            catch (Exception)
            {
                return Json(new { status = false, msg = "Upload không thành công", file = objInfo });
            }
            return Json(new { status = true, msg = "Upload thành công", file = objInfo });
        }

        public class FileInfo
        {
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string FileNameOriginal { get; set; }
        }
        public string GetExtention(string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                var arr = file.Split('.');
                if (arr.Length > 0)
                {
                    return arr.Last();
                }
            }
            return "";
        }

        [HttpPost]
        public JsonResult Delete(int id, string fPath)
        {
            try
            {
                string fullPath = Path.Combine(Server.MapPath("~/Upload/Videos/Introduct"), fPath);
                if (System.IO.File.Exists(fullPath))
                {
                    introductionRepository.UpdateVideo(id);
                    System.IO.File.Delete(fullPath);
                    return Json(new { status = true, msg = "Xóa thành công." });
                }
                else
                {
                    return Json(new { status = false, msg = "File không tồn tại." });
                }    
            }
            catch (Exception)
            { 
                return Json(new { status = false, msg = "Xóa thất bại." });
            }
           
        }
    }
}
