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
    }
}