using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class ArticleCreateViewModel
    {
        public int CategoryId { get; set; }

        public string Image { get; set; }

        public int Status { get; set; }

        public DateTime? PublishDate { get; set; }

        public List<ArticleLanguageViewModel> Languages { get; set; }
    }
}
