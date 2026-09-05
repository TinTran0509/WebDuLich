using CMS.IRepository;
using CMS.Reporitory;
using System;
using System.Linq;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Model;

namespace Web.Areas.Admin.Controllers
{
    public class HeaderController : BaseController
    {
        IHeaderRepository headerRepository = new HeaderRepository();
        // GET: Menu
        [Authorize]
        public ActionResult Add()
        {
            var model = headerRepository.GetAll().FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(int id,string content)
        {
            try
            {
                if (id == 0)
                    headerRepository.Add(content);
                else
                {
                    Header footer = new Header();
                    footer.ID = id;
                    footer.Contents = content;
                    headerRepository.Edit(footer);
                }
                return Json(new { IsSuccess = true, Message = "Lưu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false,Message = "Lưu thất bại" },JsonRequestBehavior.AllowGet);
            }
        }
    }
}