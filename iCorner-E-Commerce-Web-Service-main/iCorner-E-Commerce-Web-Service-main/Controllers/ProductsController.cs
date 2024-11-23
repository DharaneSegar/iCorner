using EAD.Models;
using EAD.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    // Route configuration for the Products API controller
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Constructor to initialize repositories
        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>List of all products.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Retrieves products by category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to filter products.</param>
        /// <returns>List of products in the specified category.</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(string categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <param name="imageFile">Optional image file for the product.</param>
        /// <returns>Created result with the newly added product.</returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product, [FromForm] IFormFile? imageFile)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(product.CategoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            product.CategoryName = category.Name;

            await _productRepository.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Uploads an image for a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="imageFile">The image file to upload.</param>
        /// <returns>OK result with the image path.</returns>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] string productId, [FromForm] IFormFile imageFile)
        {
            if (string.IsNullOrEmpty(productId))
                return BadRequest("Product ID is required.");

            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded.");

            var imagePath = await SaveImageLocallyAsync(imageFile);

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound("Product not found.");

            product.ImagePath = imagePath;
            await _productRepository.UpdateProductAsync(product);

            return Ok(new { imagePath });
        }

        /// <summary>
        /// Saves the uploaded image to the local filesystem.
        /// </summary>
        /// <param name="imageFile">The image file to save.</param>
        /// <returns>The path to the saved image.</ret
        private async Task<string> SaveImageLocallyAsync(IFormFile imageFile)
        {
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="product">The updated product data.</param>
        /// <returns>OK result with the updated product.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            if (id != product.Id) return BadRequest();

            var category = await _categoryRepository.GetCategoryByIdAsync(product.CategoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            product.CategoryName = category.Name;

            await _productRepository.UpdateProductAsync(product);
            return Ok(product);
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>OK result with the ID of the deleted product.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productRepository.DeleteProductAsync(id);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Deactivates a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to deactivate.</param>
        /// <returns>OK result with the ID of the deactivated product.</returns>

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeactivateProductAsync(id);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Activates a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to activate.</param>
        /// <returns>OK result with the ID of the activated product.</returns>
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.ActivateProductAsync(id);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Retrieves all products with stock quantity greater than 0.
        /// </summary>
        /// <returns>List of available products.</returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            var availableProducts = await _productRepository.GetAvailableProductsAsync();
            return Ok(availableProducts);
        }

    }
}

