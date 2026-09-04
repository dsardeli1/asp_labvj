using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManageApp.DTOs;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers.Api
{
    [ApiController]
    [Route("api/categories")]
    [Produces("application/json")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public CategoriesApiController(ITaskRepository taskRepository, ILogger<CategoriesApiController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        private readonly ILogger<CategoriesApiController> _logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _taskRepository.GetAllCategoriesAsync();
            return Ok(categories.Select(category => category.ToDTO()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category is null)
            {
                return NotFound();
            }

            return Ok(category.ToDTO());
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryWriteDto request)
        {
            var isUnique = await _taskRepository.IsCategoryNameUniqueAsync(request.Name);
            if (!isUnique)
            {
                _logger.LogWarning("Category creation rejected because name already exists.");
                return BadRequest($"A category named '{request.Name}' already exists.");
            }

            var created = await _taskRepository.AddCategoryAsync(request.ToEntity());
            _logger.LogInformation("Category {CategoryId} created through the API.", created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryWriteDto request)
        {
            var existing = await _taskRepository.GetCategoryByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Category update rejected because category {CategoryId} was not found.", id);
                return NotFound();
            }

            var isUnique = await _taskRepository.IsCategoryNameUniqueAsync(request.Name, id);
            if (!isUnique)
            {
                _logger.LogWarning("Category update rejected because name already exists for category {CategoryId}.", id);
                return BadRequest($"A category named '{request.Name}' already exists.");
            }

            var category = request.ToEntity();
            category.Id = id;
            category.CreatedDate = existing.CreatedDate;

            var updated = await _taskRepository.UpdateCategoryAsync(category);
            if (!updated)
            {
                _logger.LogWarning("Category update failed for category {CategoryId}.", id);
                return NotFound();
            }

            var result = await _taskRepository.GetCategoryByIdAsync(id);
            _logger.LogInformation("Category {CategoryId} updated through the API.", id);
            return Ok((result ?? category).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetCategoryByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Category deletion rejected because category {CategoryId} was not found.", id);
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteCategoryAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Category deletion conflict for category {CategoryId}.", id);
                return Conflict("This category cannot be deleted while tasks are still assigned to it.");
            }

            _logger.LogInformation("Category {CategoryId} deleted through the API.", id);
            return NoContent();
        }
    }
}