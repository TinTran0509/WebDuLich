using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;

namespace Web.Repository.Entity
{
    public class SliderRepository : ISliderRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "SliderImages";
        public void Add(Slider obj)
        {
            _entities.Sliders.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Sliders.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Slider obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public Slider Find(int id)
        {
            return _entities.Sliders.Find(id);
        }

        public List<Slider> GetAll()
        { 
            return _entities.Sliders.ToList();
        }
    }
}
