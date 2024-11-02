using Hangfire.Api.Jobs;
using System.Reflection;

namespace Hangfire.Api
{
    public static class JobContext
    {
        public static void ActivateHangfireJobs()
        {
            var baseJobType = typeof(BaseJob);
            var jobTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && baseJobType.IsAssignableFrom(t));

            foreach (var jobType in jobTypes)
            {
                var jobInstance = Activator.CreateInstance(jobType) as BaseJob;
                jobInstance?.Define();
            }
        }
    }
}
