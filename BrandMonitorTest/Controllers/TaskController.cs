using BrandMonitorTest.Data;
using BrandMonitorTest.Enums;
using BrandMonitorTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrandMonitorTest.Controllers
{
    [ApiController]
    [Route("task")]
    public class TaskController : ControllerBase
    {
        private readonly TaskDbContext _db;

        public TaskController(TaskDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Создает пустую задачу и возвращает ее Id (GUID)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTask()
        {
            var task = new EmptyTask
            {
                Id = Guid.NewGuid(),
                UpdateTime = DateTime.UtcNow,
                State = TaskState.created
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return Accepted(new { task.Id });
        }

        /// <summary>
        /// Если задача с таким ID существует, возвращает её статус
        /// </summary>
        /// <param name="id"> GUID </param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(string id)
        {
            if (!Guid.TryParse(id, out var taskId))
            {
                return BadRequest(); // только код сообщение в задаче не указано
            }

            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(new { status = task.State.ToString() });
        }

    }
}
