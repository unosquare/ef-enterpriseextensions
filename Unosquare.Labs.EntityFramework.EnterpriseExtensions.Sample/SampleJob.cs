using System;
using System.Threading;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    public class SampleJob : JobBase<SampleJob, SampleDb>
    {
        public SampleJob() : base(null, new SimpleConsoleLog())
        {
            
        }

        protected override void DoWork(object argument)
        {
            // Doing nothing yay!
            Thread.Sleep(TimeSpan.FromMinutes(5));
        }

        protected override bool BackgroundCondition()
        {
            return DateTime.Now.Minute%2 == 0;
        }
    }
}
