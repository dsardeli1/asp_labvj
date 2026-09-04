using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManageApp.DTOs;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers.Api
{
    [ApiController]
    [Route("api/comments")]
    [Produces("application/json")]
    public class CommentsApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public CommentsApiController(ITaskRepository taskRepository, ILogger<CommentsApiController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        private readonly ILogger<CommentsApiController> _logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetAll()
        {
            var comments = await _taskRepository.GetAllCommentsAsync();
            return Ok(comments.Select(comment => comment.ToDTO()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> GetById(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment is null)
            {
                return NotFound();
            }

            return Ok(comment.ToDTO());
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> Create([FromBody] CommentWriteDto request)
        {
            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                _logger.LogWarning("Comment creation rejected because task {TaskId} was not found.", request.TaskItemId);
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var user = await _taskRepository.GetUserByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning("Comment creation rejected because user {UserId} was not found.", request.UserId);
                return BadRequest($"User {request.UserId} was not found.");
            }

            var comment = request.ToEntity();
            comment.CreatedDate = DateTime.UtcNow;
            comment.IsEdited = false;

            var created = await _taskRepository.AddCommentAsync(comment);
            var result = (await _taskRepository.GetCommentByIdAsync(created.Id)) ?? created;
            _logger.LogInformation("Comment {CommentId} created through the API.", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToDTO());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CommentDto>> Update(int id, [FromBody] CommentWriteDto request)
        {
            var existing = await _taskRepository.GetCommentByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Comment update rejected because comment {CommentId} was not found.", id);
                return NotFound();
            }

            var task = await _taskRepository.GetTaskByIdAsync(request.TaskItemId);
            if (task is null)
            {
                _logger.LogWarning("Comment update rejected because task {TaskId} was not found.", request.TaskItemId);
                return BadRequest($"Task {request.TaskItemId} was not found.");
            }

            var user = await _taskRepository.GetUserByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning("Comment update rejected because user {UserId} was not found.", request.UserId);
                return BadRequest($"User {request.UserId} was not found.");
            }

            var comment = request.ToEntity();
            comment.Id = id;

            var updated = await _taskRepository.UpdateCommentAsync(comment);
            if (!updated)
            {
                _logger.LogWarning("Comment update failed for comment {CommentId}.", id);
                return NotFound();
            }

            var result = await _taskRepository.GetCommentByIdAsync(id);
            _logger.LogInformation("Comment {CommentId} updated through the API.", id);
            return Ok((result ?? comment).ToDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _taskRepository.GetCommentByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Comment deletion rejected because comment {CommentId} was not found.", id);
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteCommentAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Comment deletion failed for comment {CommentId}.", id);
                return Conflict("The comment could not be deleted.");
            }

            _logger.LogInformation("Comment {CommentId} deleted through the API.", id);
            return NoContent();
        }

        [HttpGet("task/{taskItemId:int}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByTask(int taskItemId)
        {
            var comments = await _taskRepository.GetCommentsByTaskIdAsync(taskItemId);
            return Ok(comments.Select(comment => comment.ToDTO()));
        }
    }
}