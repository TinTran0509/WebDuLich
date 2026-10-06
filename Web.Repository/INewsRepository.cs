using System;
using System.Collections.Generic;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface INewsRepository
    {
        IEnumerable<ListNews> ListAll(string keyWord, int status);
        IEnumerable<News> GetAll();
        IEnumerable<NewsTran> GetAllNewsTrans();
        int Add(News model);
        void AddNewsTrans(NewsTran model);
        void Delete(int newsid);
        News Find(int id);
        News FindByTitle(string title);
        News FindByLinkSeo(string linkseo);
        void Edit(News obj, List<NewsTran> newsTrans);
        IEnumerable<NewsModel> GetByPage(string title, int cateId, int pageIndex, int pageSize, out int total);

    }
}
