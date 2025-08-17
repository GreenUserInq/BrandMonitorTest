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
            var test = new EmptyTask
            {
                Id = Guid.NewGuid(),
                UpdateTime = DateTime.UtcNow,
                State = TaskState.created
            };

            _db.Tasks.Add(test);
            await _db.SaveChangesAsync();

            return Accepted(new { test.Id });
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(string id)
        {
            if (!Guid.TryParse(id, out var taskId))
            {
                return NotFound();
            }

            var task = _db.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(new { status = task.State.ToString() });
        }
    }
}
