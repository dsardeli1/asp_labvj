using Microsoft.AspNetCore.Mvc;
using TaskManageApp.DTOs;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers.Api
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UsersApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public UsersApiController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            return Ok(users.Select(user => user.ToDTO()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _taskRepository.GetUserByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(user.ToDTO());
        }

        [HttpGet("with-tasks")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetWithTasks()
        {
            var users = await _taskRepository.GetUsersWithTasksAsync();
            return Ok(users.Select(user => user.ToDTO()));
        }

        [HttpGet("without-tasks")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetWithoutTasks()
        {
            var users = await _taskRepository.GetUsersWithoutTasksAsync();
            return Ok(users.Select(user => user.ToDTO()));
        }
    }
}