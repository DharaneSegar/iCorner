namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository class for handling operations related to categories in the database.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category> _categories;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public CategoryRepository(IMongoDatabase database)
        {
            _categories = database.GetCollection<Category>("Categories");
        }

        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>An enumerable collection of all categories.</returns>
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categories.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// Retrieves a single category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The category with the specified ID, or null if not found.</returns>
        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _categories.Find(c => c.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddCategoryAsync(Category category)
        {
            if (string.IsNullOrEmpty(category.Id))
            {
                category.Id = ObjectId.GenerateNewId().ToString();
            }


            await _categories.InsertOneAsync(category);
        }

        /// <summary>
        /// Updates an existing category in the database.
        /// </summary>
        /// <param name="category">The category to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateCategoryAsync(Category category)
        {
            var objectId = ObjectId.Parse(category.Id);
            await _categories.ReplaceOneAsync(c => c.Id == objectId.ToString(), category);
        }

        /// <summary>
        /// Retrieves all categories that are currently active.
        /// </summary>
        /// <returns>An enumerable collection of active categories.</returns>
        public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
        {
            var filter = Builders<Category>.Filter.Eq(c => c.IsActive, true);
            return await _categories.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Retrieves all categories that are currently inactive.
        /// </summary>
        /// <returns>An enumerable collection of inactive categories.</returns>
        public async Task<IEnumerable<Category>> GetAllInactiveCategoriesAsync()
        {
            var filter = Builders<Category>.Filter.Eq(c => c.IsActive, false);
            return await _categories.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Deactivates a category by setting its IsActive status to false.
        /// </summary>
        /// <param name="id">The ID of the category to deactivate.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeactivateCategoryAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<Category>.Update.Set(c => c.IsActive, false);
            await _categories.UpdateOneAsync(c => c.Id == objectId.ToString(), update);
        }

        /// <summary>
        /// Activates a category by setting its IsActive status to true.
        /// </summary>
        /// <param name="id">The ID of the category to activate.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ActivateCategoryAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<Category>.Update.Set(c => c.IsActive, true);
            await _categories.UpdateOneAsync(c => c.Id == objectId.ToString(), update);
        }

        /// <summary>
        /// Deletes a category from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteCategoryAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _categories.DeleteOneAsync(c => c.Id == objectId.ToString());
        }
    }
}
