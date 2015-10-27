using System;
using System.Threading;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Log;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    public class SampleJob : JobBase<SampleJob, SampleDb>
    {
        private static int _counter = 0;

        public SampleJob(SampleDb context, ILog log, IEmailHelper emailHelper = null) : base(context, log, emailHelper)
        {
        }

        protected override void DoWork(object argument)
        {
            var value = ++_counter;

            // Doing nothing yay!
            Console.WriteLine("SampleJob starting {0} job", value);
            Thread.Sleep(TimeSpan.FromMinutes(1));
            Console.WriteLine("SampleJob ending {0} job", value);
        }

        protected override bool BackgroundCondition()
        {
            return DateTime.Now.Minute%2 == 0;
        }
    }

    public class SingletonSampleJob : SingletonJobBase<SingletonSampleJob, SampleDb>
    {
        public SingletonSampleJob() : base(null, new SimpleConsoleLog())
        {

        }

        protected override void DoWork(object argument)
        {
            // Doing nothing yay!
            Console.WriteLine("SingletonSampleJob starting");
            Thread.Sleep(TimeSpan.FromMinutes(5));
            Console.WriteLine("SingletonSampleJob ending");
        }

        protected override bool BackgroundCondition()
        {
            return DateTime.Now.Minute%2 == 0;
        }
    }
}