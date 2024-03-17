using Microsoft.AspNetCore.Mvc;
using ShopWebApplication.Models;

namespace ShopWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ShopContext _context;

        public ChartController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]

        public JsonResult JsonData()
        {
            var categories = _context.Categories.ToList();
            var categoryClothes = new List<object>();
            categoryClothes.Add(new[] { "Категорія", "Кількість товарів" });
            foreach(var c in categories)
            {
                categoryClothes.Add(new object[] { c.CategoryName, c.Products.Count()});
            }

            return new JsonResult(categoryClothes);
        }

    }
}
