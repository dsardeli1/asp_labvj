using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManageApp.DTOs;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers.Api
{
    [ApiController]
    [Route("api/tasks")]
    [Produces("application/json")]
    public class TasksApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksApiController(ITaskRepository taskRepository, ILogger<TasksApiController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        private readonly ILogger<TasksApiController> _logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return Ok(tasks.Select(task => task.ToDTO()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskItemDto>> GetById(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task is null)
            {
                return NotFound();
            }

            return Ok(task.ToDTO());
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> Create([FromBody] TaskItemWriteDto request)
        {
            var user = await _taskRepository.GetUserByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning("Task creation rejected because user {UserId} was not found.", request.UserId);
                return BadRequest($"User {request.UserId} was not found.");
            }

            var category = await _taskRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
            {
                _logger.LogWarning("Task creation rejected because category {CategoryId} was not found.", request.CategoryId);
                return BadRequest($"Category {request.CategoryId} was not found.");
            }

            var created = await _taskRepository.AddTaskAsync(request.ToEntity());
            var response = (await _taskRepository.GetTaskByIdAsync(created.Id)) ?? created;
            _logger.LogInformation("Task {TaskId} created through the API.", response.Id);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskItemDto>> Update(int id, [FromBody] TaskItemWriteDto request)
        {
            var existing = await _taskRepository.GetTaskByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Task update rejected because task {TaskId} was not found.", id);
                return NotFound();
            }

            var user = await _taskRepository.GetUserByIdAsync(request.UserId);
            if (user is null)
            {
                return BadRequest($"User {request.UserId} was not found.");
            }

            var category = await _taskRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
            {
                return BadRequest($"Category {request.CategoryId} was not found.");
            }

            var updatedTask = request.ToEntity();
            updatedTask.Id = id;

            var updated = await _taskRepository.UpdateTaskAsync(updatedTask);
            if (!updated)
            {
                _logger.LogWarning("Task update failed for task {TaskId}.", id);
                return NotFound();
            }

            var result = (await _taskRepository.GetTaskByIdAsync(id)) ?? updatedTask;
            _logger.LogInformation("Task {TaskId} updated through the API.", id);
            return Ok(result.ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetTaskByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Task deletion rejected because task {TaskId} was not found.", id);
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteTaskAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Task deletion failed for task {TaskId}.", id);
                return Conflict("The task could not be deleted.");
            }

            _logger.LogInformation("Task {TaskId} deleted through the API.", id);
            return NoContent();
        }

        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetCompleted()
        {
            var tasks = await _taskRepository.GetCompletedTasksAsync();
            return Ok(tasks.Select(task => task.ToDTO()));
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetPending()
        {
            var tasks = await _taskRepository.GetPendingTasksAsync();
            return Ok(tasks.Select(task => task.ToDTO()));
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetByCategory(int categoryId)
        {
            var tasks = await _taskRepository.GetTasksByCategoryAsync(categoryId);
            return Ok(tasks.Select(task => task.ToDTO()));
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetByUser(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserAsync(userId);
            return Ok(tasks.Select(task => task.ToDTO()));
        }
    }
}