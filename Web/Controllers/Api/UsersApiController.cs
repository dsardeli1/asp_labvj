using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public UsersApiController(ITaskRepository taskRepository, ILogger<UsersApiController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        private readonly ILogger<UsersApiController> _logger;

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
            _logger.LogInformation("User {UserId} created through the API.", created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UserWriteDto request)
        {
            var existing = await _taskRepository.GetUserByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("User update rejected because user {UserId} was not found.", id);
                return NotFound();
            }

            var user = request.ToEntity();
            user.Id = id;

            var updated = await _taskRepository.UpdateUserAsync(user);
            if (!updated)
            {
                _logger.LogWarning("User update failed for user {UserId}.", id);
                return NotFound();
            }

            var result = await _taskRepository.GetUserByIdAsync(id);
            _logger.LogInformation("User {UserId} updated through the API.", id);
            return Ok((result ?? user).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetUserByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("User deletion rejected because user {UserId} was not found.", id);
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteUserAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("User deletion conflict for user {UserId}.", id);
                return Conflict("The user cannot be deleted while tasks are still assigned to them.");
            }

            _logger.LogInformation("User {UserId} deleted through the API.", id);
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