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
        void Add(News model);
        void Delete(int newsid);
        News Find(int id);
        News FindByTitle(string title);
        News FindByLinkSeo(string linkseo);
        void Edit(News model);
        ListNews Detail(int id);
        List<ListNews> NewsGetByCategory(string linkseo);

    }
}
