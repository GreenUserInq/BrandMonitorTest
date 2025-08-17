using BrandMonitorTest.Enums;

namespace BrandMonitorTest.Models
{
    public class EmptyTask
    {
        public Guid Id { get; set; }
        public DateTime UpdateTime { get; set; }
        public TaskState State { get; set; }
    }
}
