using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManageApp.DTOs;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers.Api
{
    [ApiController]
    [Route("api/histories")]
    [Produces("application/json")]
    public class HistoriesApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public HistoriesApiController(ITaskRepository taskRepository, ILogger<HistoriesApiController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        private readonly ILogger<HistoriesApiController> _logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskHistoryDto>>> GetAll()
        {
            var histories = await _taskRepository.GetAllTaskHistoriesAsync();
            return Ok(histories.Select(history => history.ToDTO()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskHistoryDto>> GetById(int id)
        {
            var history = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (history is null)
            {
                return NotFound();
            }

            return Ok(history.ToDTO());
        }

        [HttpPost]
        public async Task<ActionResult<TaskHistoryDto>> Create([FromBody] TaskHistoryWriteDto request)
        {
            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                _logger.LogWarning("History creation rejected because task {TaskId} was not found.", request.TaskItemId);
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var history = request.ToEntity();
            if (history.ActionDate == default)
            {
                history.ActionDate = DateTime.UtcNow;
            }

            var created = await _taskRepository.AddTaskHistoryAsync(history);
            var result = (await _taskRepository.GetTaskHistoryByIdAsync(created.Id)) ?? created;
            _logger.LogInformation("History {HistoryId} created through the API.", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskHistoryDto>> Update(int id, [FromBody] TaskHistoryWriteDto request)
        {
            var existing = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("History update rejected because history {HistoryId} was not found.", id);
                return NotFound();
            }

            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                _logger.LogWarning("History update rejected because task {TaskId} was not found.", request.TaskItemId);
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var history = request.ToEntity();
            history.Id = id;
            if (history.ActionDate == default)
            {
                history.ActionDate = existing.ActionDate;
            }

            var updated = await _taskRepository.UpdateTaskHistoryAsync(history);
            if (!updated)
            {
                _logger.LogWarning("History update failed for history {HistoryId}.", id);
                return NotFound();
            }

            var result = await _taskRepository.GetTaskHistoryByIdAsync(id);
            _logger.LogInformation("History {HistoryId} updated through the API.", id);
            return Ok((result ?? history).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("History deletion rejected because history {HistoryId} was not found.", id);
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteTaskHistoryAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("History deletion failed for history {HistoryId}.", id);
                return Conflict("The task history could not be deleted.");
            }

            _logger.LogInformation("History {HistoryId} deleted through the API.", id);
            return NoContent();
        }
    }
}