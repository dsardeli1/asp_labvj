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
    }
}