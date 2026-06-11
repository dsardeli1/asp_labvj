using Microsoft.AspNetCore.Mvc;
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

        public CommentsApiController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

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

        [HttpGet("task/{taskItemId:int}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByTask(int taskItemId)
        {
            var comments = await _taskRepository.GetCommentsByTaskIdAsync(taskItemId);
            return Ok(comments.Select(comment => comment.ToDTO()));
        }
    }
}