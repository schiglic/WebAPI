using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Список продуктів з ID та назвами
        private readonly Dictionary<int, string> products = new Dictionary<int, string>
        {
            { 1, "Phone" },
            { 2, "Apples" },
            { 3, "Interior Doors" }
        };

        // Метод для отримання всіх продуктів
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAllProducts()
        {
            return Ok(products.Values);
        }

        // Метод для отримання продукту за ID
        [HttpGet("{id}")]
        public ActionResult<string> GetProductById(int id)
        {
            if (products.TryGetValue(id, out var product))
            {
                return Ok(product);
            }
            else
            {
                return NotFound($"Product with ID {id} not found");
            }
        }
    }
}
