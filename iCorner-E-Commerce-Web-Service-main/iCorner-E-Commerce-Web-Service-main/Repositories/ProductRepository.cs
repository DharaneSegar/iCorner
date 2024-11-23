namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Repository class for handling CRUD operations for Products in MongoDB.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Category> _categories;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public ProductRepository(IMongoDatabase database)
        {
            _products = database.GetCollection<Product>("Products");
            _categories = database.GetCollection<Category>("Categories");
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>An enumerable of all products.</returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// Retrieves a single product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with the specified ID, or null if not found.</returns>
        public async Task<Product> GetProductByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _products.Find(p => p.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to add.</param>

        public async Task AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = ObjectId.GenerateNewId().ToString(); // Assign new ObjectId
            }

            await _products.InsertOneAsync(product);
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product to update.</param>
        public async Task UpdateProductAsync(Product product)
        {
            var objectId = ObjectId.Parse(product.Id); // Ensure the ID is treated as ObjectId
            await _products.ReplaceOneAsync(p => p.Id == objectId.ToString(), product);
        }

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown if stock is not empty or there are pending orders.</exception>
        public async Task DeleteProductAsync(string id)
        {
            var objectId = ObjectId.Parse(id); // Convert string ID to ObjectId

            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            /* if (product != null && product.StockQuantity < 10)
            {
                throw new InvalidOperationException("Stock is not empty.");
            } */

            bool hasPendingOrders = await CheckPendingOrdersAsync(id);
            if (hasPendingOrders)
            {
                throw new InvalidOperationException("Cannot remove stock for products with pending orders.");
            }

            await _products.DeleteOneAsync(p => p.Id == objectId.ToString());
        }

        /// <summary>
        /// Checks if there are pending orders for a product.
        /// </summary>
        /// <param name="productId">The product ID to check.</param>
        /// <returns>True if there are pending orders, false otherwise.</returns>
        private async Task<bool> CheckPendingOrdersAsync(string productId)
        {
            // Implement logic to check if there are pending orders for the given product
            // This could involve checking an Orders collection or similar.
            return false; // Placeholder
        }

        /// <summary>
        /// Deactivates a product by setting its IsActive status to false.
        /// </summary>
        /// <param name="id">The ID of the product to deactivate.</param>
        public async Task DeactivateProductAsync(string id)
        {
            var objectId = ObjectId.Parse(id); // Convert string ID to ObjectId
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            await _products.UpdateOneAsync(p => p.Id == objectId.ToString(), update);
        }

        /// <summary>
        /// Activates a product by setting its IsActive status to true.
        /// </summary>
        /// <param name="id">The ID of the product to activate.</param>
        public async Task ActivateProductAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            await _products.UpdateOneAsync(p => p.Id == objectId.ToString(), update);
        }

        /// <summary>
        /// Retrieves all products that belong to a specific category.
        /// </summary>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>An enumerable of products in the specified category.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the category does not exist.</exception>
        public async Task<IEnumerable<Product>> GetAllProductsWithCategoriesAsync()
        {
            var products = await _products.Find(_ => true).ToListAsync();
            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();

            var categories = await _categories.Find(c => categoryIds.Contains(c.Id)).ToListAsync();
            var categoryDict = categories.ToDictionary(c => c.Id);

            foreach (var product in products)
            {
                if (categoryDict.TryGetValue(product.CategoryId, out var category))
                {
                    product.CategoryName = category.Name; 
                }
            }

            return products;
        }

        /// <summary>
        /// Retrieves all products that belong to a specific category.
        /// </summary>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>An enumerable of products in the specified category.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the category does not exist.</exception>
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryId)
        {
            var objectId = ObjectId.Parse(categoryId);
            var categoryExists = await _categories.Find(c => c.Id == objectId.ToString()).AnyAsync();

            if (!categoryExists)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return await _products.Find(p => p.CategoryId == objectId.ToString()).ToListAsync();
        }

        /// <summary>
        /// Retrieves all products with stock quantity greater than 0.
        /// </summary>
        /// <returns>An enumerable collection of available products.</returns>
        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _products.Find(p => p.StockQuantity > 0).ToListAsync();
        }


    }
}
