using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManageApp.DTOs;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers.Api
{
    [ApiController]
    [Route("api/attachments")]
    [Produces("application/json")]
    public class AttachmentsApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public AttachmentsApiController(ITaskRepository taskRepository, ILogger<AttachmentsApiController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        private readonly ILogger<AttachmentsApiController> _logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAttachmentDto>>> GetAll()
        {
            var attachments = await _taskRepository.GetAllTaskAttachmentsAsync();
            return Ok(attachments.Select(attachment => attachment.ToDTO()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskAttachmentDto>> GetById(int id)
        {
            var attachment = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (attachment is null)
            {
                return NotFound();
            }

            return Ok(attachment.ToDTO());
        }

        [HttpPost]
        public async Task<ActionResult<TaskAttachmentDto>> Create([FromBody] TaskAttachmentWriteDto request)
        {
            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                _logger.LogWarning("Attachment creation rejected because task {TaskId} was not found.", request.TaskItemId);
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var attachment = request.ToEntity();
            var created = await _taskRepository.AddTaskAttachmentAsync(attachment);
            var result = (await _taskRepository.GetTaskAttachmentByIdAsync(created.Id)) ?? created;
            _logger.LogInformation("Attachment {AttachmentId} created through the API.", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskAttachmentDto>> Update(int id, [FromBody] TaskAttachmentWriteDto request)
        {
            var existing = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Attachment update rejected because attachment {AttachmentId} was not found.", id);
                return NotFound();
            }

            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                _logger.LogWarning("Attachment update rejected because task {TaskId} was not found.", request.TaskItemId);
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var attachment = request.ToEntity();
            attachment.Id = id;

            var updated = await _taskRepository.UpdateTaskAttachmentAsync(attachment);
            if (!updated)
            {
                _logger.LogWarning("Attachment update failed for attachment {AttachmentId}.", id);
                return NotFound();
            }

            var result = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            _logger.LogInformation("Attachment {AttachmentId} updated through the API.", id);
            return Ok((result ?? attachment).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Attachment deletion rejected because attachment {AttachmentId} was not found.", id);
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteTaskAttachmentAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Attachment deletion failed for attachment {AttachmentId}.", id);
                return Conflict("The task attachment could not be deleted.");
            }

            _logger.LogInformation("Attachment {AttachmentId} deleted through the API.", id);
            return NoContent();
        }
    }
}