using Microsoft.AspNetCore.Mvc;
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

        public TasksApiController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

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