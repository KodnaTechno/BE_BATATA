using System.Reflection;

namespace Hangfire.Api.Jobs
{
    public abstract class BaseJob
    {
        protected abstract string JobName { get; }
        protected abstract string CronExpression { get; }

        public void Define()
        {
            RecurringJob.AddOrUpdate(JobName, () => Execute(), CronExpression);
        }

        public abstract void Execute();

        protected bool IsHoliday(List<(DateTime from, DateTime to)> holidays, DateTime date)
        {
            return holidays.Any(holiday =>
                date.Date >= holiday.from.Date && date.Date <= holiday.to.Date);
        }
    }





}
