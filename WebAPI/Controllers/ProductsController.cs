using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Список продуктів з ID, назвами та цінами
        private static Dictionary<int, Product> products = new Dictionary<int, Product>
        {
            { 1, new Product { Name = "Phone", Price = 599.99m } },
            { 2, new Product { Name = "Apples", Price = 1.99m } },
            { 3, new Product { Name = "Interior Doors", Price = 199.99m } }
        };

        // Валідатор для моделі Product
        private readonly IValidator<Product> _validator;

        public ProductsController(IValidator<Product> validator)
        {
            _validator = validator;
        }

        // Метод для отримання всіх продуктів
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            return Ok(products.Values);
        }

        // Метод для отримання продукту за ID
        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
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

        // Метод для додавання нового продукту (з валідацією)
        [HttpPost]
        public ActionResult<IEnumerable<Product>> AddProduct(Product product)
        {
            // Валідація моделі
            ValidationResult result = _validator.Validate(product);

            if (!result.IsValid)
            {
                // Повертаємо помилки валідації
                return BadRequest(result.Errors);
            }

            // Додаємо продукт до списку
            int newId = products.Count + 1;
            products.Add(newId, product);

            // Повертаємо оновлений список продуктів
            return Ok(products.Values);
        }
    }

    // Модель Product
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // Валідатор для моделі Product
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    }
}