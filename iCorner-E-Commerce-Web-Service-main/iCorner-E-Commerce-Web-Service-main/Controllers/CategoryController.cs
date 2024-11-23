using Microsoft.AspNetCore.Mvc;
using EAD.Models;
using EAD.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    // Route configuration for the Category API controller
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryRepository">The category repository for data operations.</param>
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="category">The category object to add.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null.");
            }

            await _categoryRepository.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the list of categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="category">The updated category data.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] Category category)
        {
            if (category.Name == null || id == null)
            {
                return BadRequest("Category data is invalid.");
            }

            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
            return Ok(existingCategory);
        }

        /// <summary>
        /// Deactivates a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to deactivate.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateCategory(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeactivateCategoryAsync(id);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Activates a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to activate.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateCategory(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.ActivateCategoryAsync(id);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Retrieves all active categories.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the list of active categories.</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveCategories()
        {
            var categories = await _categoryRepository.GetAllActiveCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Retrieves all inactive categories.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the list of inactive categories.</returns>
        [HttpGet("deactive")]
        public async Task<IActionResult> GetAllInactiveCategories()
        {
            var categories = await _categoryRepository.GetAllInactiveCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            return Ok(new { Id = id });
        }
    }
}
