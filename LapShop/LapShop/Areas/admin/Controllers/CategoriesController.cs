using Microsoft.AspNetCore.Mvc;
using LapShop.Models;
using LapShop.Bl;
namespace LapShop.Areas.admin.Controllers
{
    [Area("admin")]
    public class CategoriesController : Controller
    {
        ClsCategories oClsCategories = new ClsCategories();
        public IActionResult List()
        {
            ClsCategories oClsCategories = new ClsCategories();
            return View(oClsCategories.GetAll());
        }

        public IActionResult Edit(int? categoryId)
        {
            var category = new TbCategory();
            if (categoryId != null)
            {
                category = oClsCategories.GetById(Convert.ToInt32(category));
            }
             
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Save(TbCategory category, List<IFormFile> Files)
        {
            if (!ModelState.IsValid)
                return View("Edit",category);
            category.ImageName = await UploadImage(Files);
            oClsCategories.Save(category);

            return RedirectToAction("List");
        }
        public IActionResult Delete(int categoryId)
        {
            oClsCategories.Delete(categoryId);
            return RedirectToAction("List");
        }

        async Task<string> UploadImage(List<IFormFile> Files)
        {
            foreach (var file in Files)
            {
                if (file.Length > 0)
                {
                    string ImageName = Guid.NewGuid().ToString() + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".jpg";
                    var filePaths = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads/Categories", ImageName);
                    using (var stream = System.IO.File.Create(filePaths))
                    {
                        await file.CopyToAsync(stream);
                        return ImageName;
                    }
                }
            }
            return string.Empty;
        }
    }
}
