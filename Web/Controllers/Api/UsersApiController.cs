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

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserWriteDto request)
        {
            var created = await _taskRepository.AddUserAsync(request.ToEntity());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UserWriteDto request)
        {
            var existing = await _taskRepository.GetUserByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var user = request.ToEntity();
            user.Id = id;

            var updated = await _taskRepository.UpdateUserAsync(user);
            if (!updated)
            {
                return NotFound();
            }

            var result = await _taskRepository.GetUserByIdAsync(id);
            return Ok((result ?? user).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetUserByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteUserAsync(id);
            if (!deleted)
            {
                return Conflict("The user cannot be deleted while tasks are still assigned to them.");
            }

            return NoContent();
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