using Microsoft.AspNetCore.Mvc;
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

        public HistoriesApiController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

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
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var history = request.ToEntity();
            if (history.ActionDate == default)
            {
                history.ActionDate = DateTime.UtcNow;
            }

            var created = await _taskRepository.AddTaskHistoryAsync(history);
            var result = (await _taskRepository.GetTaskHistoryByIdAsync(created.Id)) ?? created;
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskHistoryDto>> Update(int id, [FromBody] TaskHistoryWriteDto request)
        {
            var existing = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
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
                return NotFound();
            }

            var result = await _taskRepository.GetTaskHistoryByIdAsync(id);
            return Ok((result ?? history).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteTaskHistoryAsync(id);
            if (!deleted)
            {
                return Conflict("The task history could not be deleted.");
            }

            return NoContent();
        }
    }
}