using Microsoft.AspNetCore.Mvc;
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

        public CategoriesApiController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

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
                return BadRequest($"A category named '{request.Name}' already exists.");
            }

            var created = await _taskRepository.AddCategoryAsync(request.ToEntity());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryWriteDto request)
        {
            var existing = await _taskRepository.GetCategoryByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var isUnique = await _taskRepository.IsCategoryNameUniqueAsync(request.Name, id);
            if (!isUnique)
            {
                return BadRequest($"A category named '{request.Name}' already exists.");
            }

            var category = request.ToEntity();
            category.Id = id;
            category.CreatedDate = existing.CreatedDate;

            var updated = await _taskRepository.UpdateCategoryAsync(category);
            if (!updated)
            {
                return NotFound();
            }

            var result = await _taskRepository.GetCategoryByIdAsync(id);
            return Ok((result ?? category).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetCategoryByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return Conflict("This category cannot be deleted while tasks are still assigned to it.");
            }

            return NoContent();
        }
    }
}