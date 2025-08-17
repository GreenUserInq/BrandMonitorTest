
using BrandMonitorTest.Data;
using BrandMonitorTest.Enums;

namespace BrandMonitorTest.Services
{
    public class TaskWorker : BackgroundService
    {
        private readonly IServiceProvider _services;

        public TaskWorker(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           while (!stoppingToken.IsCancellationRequested)
           {
                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

                    var tasks = db.Tasks.Where(o => o.State != TaskState.finished);

                    foreach ( var task in tasks )
                    {
                        if (task.State == TaskState.created)
                        {
                            task.State = TaskState.running;
                            task.UpdateTime = DateTime.UtcNow;
                        }
                        else if (
                            task.State == TaskState.running &&
                            DateTime.UtcNow - task.UpdateTime >= TimeSpan.FromMinutes(2))
                        {
                            task.State = TaskState.finished;
                            task.UpdateTime = DateTime.UtcNow;
                        }
                    }
                    db.SaveChanges();
                }
                catch
                {
                    //"логер в сделку не входил" 
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
