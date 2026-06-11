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
    }
}