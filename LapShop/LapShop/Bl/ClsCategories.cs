using LapShop.Models;
namespace LapShop.Bl
{
    public class ClsCategories
    {
        public List<TbCategory> GetAll()
        {
            try
            {
                LapShopContext context = new LapShopContext();
                var lstCategories = context.TbCategories.ToList();
                return lstCategories;
            }
            catch
            {
                return new List<TbCategory>();
            }
           
        }

        public TbCategory GetById(int id)
        {
            try
            {
                LapShopContext context = new LapShopContext();
                var category = context.TbCategories.FirstOrDefault(a => a.CategoryId == id);
                return category;
            }
            catch
            {
                return new TbCategory();
            }
        }

        public bool Save(TbCategory category) 
        {
            try
            {
                LapShopContext context = new LapShopContext();
                if (category.CategoryId == 0)
                {
                    category.CreatedBy = "1";
                    category.CreatedDate = DateTime.Now;
                    context.TbCategories.Add(category);
                }
                else
                {
                    category.UpdatedBy = "1";
                    category.UpdatedDate = DateTime.Now;
                    context.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool Delete(int id)
        {
            try
            {
                LapShopContext context = new LapShopContext();
                var category = GetById(id);
                context.TbCategories.Remove(category);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
