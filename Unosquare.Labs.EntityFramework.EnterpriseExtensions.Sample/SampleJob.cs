using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    public class SampleJob : JobBase<SampleJob>
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
