namespace EAD.Repositories
{
    using EAD.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Category Repository, defining operations for managing categories in the system.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>An enumerable collection of all categories.</returns>
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        /// <summary>
        /// Retrieves a single category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The category with the specified ID, or null if not found.</returns>
        Task<Category> GetCategoryByIdAsync(string id);

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddCategoryAsync(Category category);

        /// <summary>
        /// Updates an existing category in the database.
        /// </summary>
        /// <param name="category">The category to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateCategoryAsync(Category category);

        /// <summary>
        /// Deletes a category from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteCategoryAsync(string id);

        /// <summary>
        /// Deactivates a category by setting its IsActive status to false.
        /// </summary>
        /// <param name="id">The ID of the category to deactivate.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeactivateCategoryAsync(string id);

        /// <summary>
        /// Activates a category by setting its IsActive status to true.
        /// </summary>
        /// <param name="id">The ID of the category to activate.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ActivateCategoryAsync(string id);

        /// <summary>
        /// Retrieves all categories that are currently active.
        /// </summary>
        /// <returns>An enumerable collection of active categories.</returns>
        Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();

        /// <summary>
        /// Retrieves all categories that are currently inactive.
        /// </summary>
        /// <returns>An enumerable collection of inactive categories.</returns>
        Task<IEnumerable<Category>> GetAllInactiveCategoriesAsync();
    }
}
