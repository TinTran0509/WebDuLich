using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;
using Web.Resources;

namespace Web.Areas.Admin.Controllers
{
    public class NewsController : BaseController
    {

        private readonly INewsRepository newsRepository = new NewsRepository();
        private readonly ICategoryRepository categoryRepository = new CategoryRepository();
        private readonly ILanguageRepository languageRepository = new LanguageRepository();
        // GET: News
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(string title, int cateId, int pageIndex)
        { 
            List<NewsModel> newsModels = newsRepository.GetByPage(title, cateId, pageIndex, 20, out int total).ToList();
       
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/News/_ListData.cshtml", newsModels),
                totalPages = Math.Ceiling(((double)total / 20)),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var obj = newsRepository.Find(id);
            return View(obj);
        }

        [Authorize(Roles = "Add")]
        [HttpGet]
        public ActionResult Add()
        {
            tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x => x.IsDefault);
            if (language != null)
            {
                TempData["Categories"] = categoryRepository.GetAllTrans().Where(x => x.LangCode.Equals(language.LangCode)).ToList(); 
            }

            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<NewsLanguageViewModel> newsLanguageViewModels = new List<NewsLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                NewsLanguageViewModel newsLanguageViewModel = new NewsLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                newsLanguageViewModels.Add(newsLanguageViewModel);
            }
            var model = new NewsCreateViewModel
            {
                Languages = newsLanguageViewModels
            };
            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(NewsCreateViewModel model)
        {
            try
            {
                if(model.CategoryID == 0)
                {
                    return Json(new
                    { 
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn danh mục"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ảnh"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<NewsTran> newsTrans = new List<NewsTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(lang.Description))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm mô tả ngắn " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    NewsTran newsTran = new NewsTran
                    {
                        MetaTitle = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Description = lang.Description,
                        Contents = lang.Contents
                    };

                    newsTrans.Add(newsTran);
                }

                News news = new News
                {
                    CategoryId = model.CategoryID,
                    Image = model.Image,
                    CreatedBy = User.ID
                };

                int id = newsRepository.Add(news);

                foreach (var item in newsTrans)
                {
                    item.NewsID = id;
                    newsRepository.AddNewsTrans(item);
                }

                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Thêm mới thất bại "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            News news = newsRepository.Find(id);

            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            tbl_Languages language = tbl_Languages.FirstOrDefault(x => x.IsDefault);

            if (language != null)
            {
                TempData["Categories"] = categoryRepository.GetAllTrans().Where(x => x.LangCode.Equals(language.LangCode)).ToList();
            }

            List<NewsTran> lstNewsTrans = newsRepository.GetAllNewsTrans().Where(x => x.NewsID == id).ToList();

            List<NewsLanguageViewModel> newsLanguageViewModels = new List<NewsLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                NewsTran newsTran_Edit = lstNewsTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (newsTran_Edit != null)
                {
                    NewsLanguageViewModel newsLanguageViewModel = new NewsLanguageViewModel
                    {
                        ID = newsTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Title = newsTran_Edit.MetaTitle,
                        Description = newsTran_Edit.Description,
                        Contents = newsTran_Edit.Contents,
                    };
                    newsLanguageViewModels.Add(newsLanguageViewModel);
                }
            }

            var model = new NewsCreateViewModel
            {
                ID = id,
                CategoryID = news.CategoryId, 
                Image = news.Image, 
                Languages = newsLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NewsCreateViewModel model)
        {
            try
            {
                if (model.CategoryID == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn danh mục"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ảnh"
                    }, JsonRequestBehavior.AllowGet);
                } 

                List<NewsTran> newsTrans = new List<NewsTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName, 
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(lang.Description))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm mô tả ngắn " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    NewsTran newsTranEdit = new NewsTran
                    {
                        ID = lang.ID,
                        MetaTitle = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        Description = lang.Description,
                        Contents = lang.Contents
                    };
                    newsTrans.Add(newsTranEdit);
                }

                News news = new News
                {
                    ID = model.ID,
                    CategoryId = model.CategoryID,
                    Image = model.Image,
                    ModifiedBy = User.ID
                };

                newsRepository.Edit(news, newsTrans);

                return Json(new {
                    IsSuccess = true,
                    Messenger = "Cập nhật thành công",
                    JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { 
                    IsSuccess = false,
                    Messenger = "Cập nhật thất bại", 
                    JsonRequestBehavior.AllowGet });
            }
        }

        [Authorize(Roles = "Delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                newsRepository.Delete(id);
                return Json(new { IsSuccess = true, Messenger = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Messenger = "Xóa thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase upload)
        {
            if (upload == null || upload.ContentLength == 0)
            {
                return Json(new
                {
                    uploaded = false,
                    error = new
                    {
                        message = "Không nhận được file."
                    }
                });
            }

            string extension = Path.GetExtension(upload.FileName);

            string[] allowed = {".jpg",".jpeg",".png",".gif",".webp"};

            if (!allowed.Contains(extension.ToLower()))
            {
                return Json(new
                {
                    uploaded = false,
                    error = new
                    {
                        message = "Định dạng ảnh không hợp lệ."
                    }
                });
            }

            // Lưu file temp
            string fTemp = Path.Combine(Server.MapPath("~/Upload/Temp/"), upload.FileName);
            upload.SaveAs(fTemp);

            // Đường dẫn tới đích lưu
            string fileCompress = Path.Combine(Server.MapPath("~/Upload/Post/"), upload.FileName);

            byte[] original = System.IO.File.ReadAllBytes(fTemp);

            // Nén ảnh tới 80%
            byte[] compressed = CompressImage(original, 80);

            // Lưu ảnh đã nén
            System.IO.File.WriteAllBytes(fileCompress, compressed);

            // Xóa ảnh lưu tạm
            System.IO.File.Delete(fTemp);
  
            return Json(new
            {
                uploaded = true,
                url = Url.Content("~/Upload/Post/" + upload.FileName)
            });
        }

        public static byte[] CompressImage(byte[] imageBytes, long quality)
        {
            using (var input = new MemoryStream(imageBytes))
            using (var image = Image.FromStream(input))
            using (var output = new MemoryStream())
            {
                ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);

                var encoderParams = new EncoderParameters(1);

                encoderParams.Param[0] = new EncoderParameter(
                    System.Drawing.Imaging.Encoder.Quality,
                    quality
                );

                image.Save(output, jpgEncoder, encoderParams);

                return output.ToArray();
            }
        }
    }
}