using System;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    public class SampleJob : JobBase<SampleDb>
    {
        private static int _counter = 0;

        public SampleJob(SampleDb context) : base(context)
        {
        }

        protected override async Task DoWork(object argument, CancellationToken ct)
        {
            var value = ++_counter;

            // Doing nothing yay!
            Console.WriteLine("SampleJob starting {0} job", value);
            await Task.Delay(TimeSpan.FromMinutes(1), ct);
            Console.WriteLine("SampleJob ending {0} job", value);
        }

        protected override bool BackgroundCondition()
        {
            return DateTime.Now.Minute%2 == 0;
        }
    }

    public class SingletonSampleJob : SingletonJobBase<SingletonSampleJob, SampleDb>
    {
        public static bool Onetime;

        public SingletonSampleJob() : base(new SampleDb())
        {

        }

        protected override async Task DoWork(object argument, CancellationToken ct)
        {
            // Doing nothing yay!
            Console.WriteLine("SingletonSampleJob starting");
            await Task.Delay(TimeSpan.FromMinutes(5), ct);
            Console.WriteLine("SingletonSampleJob ending");
        }

        protected override bool BackgroundCondition()
        {
            if (Onetime) return false;

            Onetime = true;
            return true;
        }
    }
}