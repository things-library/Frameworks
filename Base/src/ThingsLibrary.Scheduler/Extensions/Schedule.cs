using NCrontab;
using ThingsLibrary.DataType;

namespace ThingsLibrary.Scheduler.Extensions
{
    public static class ScheduleExtensions
    {
        public static DateTime GetNextIncrement(this DataType.Schedule schedule)
        {
            return GetNextIncrement(schedule, DateTime.UtcNow);            
        }

        public static DateTime GetNextIncrement(this DataType.Schedule schedule, DateTime dateTime)
        {
            if (schedule.TTL != null)
            {
                return dateTime.Add(schedule.TTL.Value);
            }
            else if (string.IsNullOrEmpty(schedule.Cron))
            {
                var cronSchedule = CrontabSchedule.Parse(schedule.Cron);

                return cronSchedule.GetNextOccurrence(dateTime);
            }
            else
            {
                throw new ArgumentException("Invalid schedule. Must have a TTL OR CRON Schedule.");
            }
        }
    }
}