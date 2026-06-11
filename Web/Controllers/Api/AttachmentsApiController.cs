using Microsoft.AspNetCore.Mvc;
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

        public AttachmentsApiController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

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
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var attachment = request.ToEntity();
            var created = await _taskRepository.AddTaskAttachmentAsync(attachment);
            var result = (await _taskRepository.GetTaskAttachmentByIdAsync(created.Id)) ?? created;
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskAttachmentDto>> Update(int id, [FromBody] TaskAttachmentWriteDto request)
        {
            var existing = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var attachment = request.ToEntity();
            attachment.Id = id;

            var updated = await _taskRepository.UpdateTaskAttachmentAsync(attachment);
            if (!updated)
            {
                return NotFound();
            }

            var result = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            return Ok((result ?? attachment).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteTaskAttachmentAsync(id);
            if (!deleted)
            {
                return Conflict("The task attachment could not be deleted.");
            }

            return NoContent();
        }
    }
}