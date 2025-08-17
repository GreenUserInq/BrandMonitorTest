
using BrandMonitorTest.Data;
using BrandMonitorTest.Enums;
using BrandMonitorTest.Models;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace BrandMonitorTest.Services
{
    public class TaskWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TaskSettings _settings;

        public TaskWorker(IServiceProvider services, IOptions<TaskSettings> options)
        {
            _services = services;
            _settings = options.Value;
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

                    var runDelay = _settings.RuningDelayMinutes;
                    var finishDelay = _settings.FinishingDelayMinutes;

                    foreach ( var task in tasks )
                    {
                        if (task.State == TaskState.created &&
                            DateTime.UtcNow - task.UpdateTime >= TimeSpan.FromMinutes(runDelay))
                        {
                            task.State = TaskState.running;
                            task.UpdateTime = DateTime.UtcNow;
                        }
                        else if (
                            task.State == TaskState.running &&
                            DateTime.UtcNow - task.UpdateTime >= TimeSpan.FromMinutes(finishDelay))
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
