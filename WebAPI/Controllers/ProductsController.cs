using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using WebAPI; 

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        
        private static Dictionary<int, ProductModel> staticProducts = new Dictionary<int, ProductModel>
        {
            { 1, new ProductModel { Id = 1, Name = "Phone", Price = 599.99m } },
            { 2, new ProductModel { Id = 2, Name = "Apples", Price = 1.99m } },
            { 3, new ProductModel { Id = 3, Name = "Interior Doors", Price = 199.99m } }
        };

        private readonly ApplicationDbContext _context;
        private readonly IValidator<ProductModel> _validator;

        public ProductsController(ApplicationDbContext context, IValidator<ProductModel> validator)
        {
            _context = context;
            _validator = validator;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllProducts()
        {
            var productsFromDb = await _context.Products.ToListAsync();
            return Ok(productsFromDb);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductById(int id)
        {
            var productFromDb = await _context.Products.FindAsync(id);

            if (productFromDb == null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(productFromDb);
        }

        
        [HttpPost]
        public async Task<ActionResult<ProductModel>> AddProduct(ProductModel product)
        {
            
            ValidationResult result = _validator.Validate(product);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        
        [HttpGet("static")]
        public ActionResult<IEnumerable<ProductModel>> GetStaticProducts()
        {
            return Ok(staticProducts.Values);
        }
    }

    
    public class ProductValidator : AbstractValidator<ProductModel>
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