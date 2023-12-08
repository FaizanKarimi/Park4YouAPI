using System;
using Quartz;
using Quartz.Spi;

namespace BackgroundSchedulers
{
    public class JobFactory : IJobFactory
    {
        protected readonly IServiceProvider Container;

        public JobFactory(IServiceProvider container)
        {
            Container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new AutoParkingJob(Container);
        }

        public void ReturnJob(IJob job)
        {
            // i couldn't find a way to release services with your preferred DI, 
            // its up to you to google such things
        }
    }
}