using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Creates a job with multiple running options
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type, it must inherent IBusinessDbContext</typeparam>
    /// <typeparam name="TParam">The job input parameter type</typeparam>
    public abstract class JobBase<TDbContext, TParam>
        where TDbContext : IBusinessDbContext
        where TParam : class
    {
        /// <summary>
        /// The BusinessDbContext instance
        /// </summary>
        protected readonly TDbContext Context;

        /// <summary>
        /// Invalid constructor, you must call the constructor with parameters
        /// </summary>
        protected JobBase()
        {
            throw new InvalidOperationException("You need to call constructor with parameters in your implementation");
        }

        /// <summary>
        /// Creates a new Job, you should call this constructor from base method in your implementation class
        /// </summary>
        /// <param name="context"></param>
        protected JobBase(TDbContext context)
        {
            if (context == null)
                throw new ArgumentException("You must pass a DbContext", nameof(context));

            Context = context;
        }

        /// <summary>
        /// Check if it's running
        /// </summary>
        public virtual bool IsRunning { get; protected set; }

        /// <summary>
        /// The job work task
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        protected abstract Task DoWork(TParam argument, CancellationToken ct);

        /// <summary>
        /// Defines the condition to run job in background
        /// </summary>
        /// <returns></returns>
        protected abstract bool BackgroundCondition();

        /// <summary>
        /// Runs the job as a BackgroundWorker
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="idleTime"></param>
        /// <param name="argument"></param>
        public async void RunBackgroundWork(CancellationToken ct, TimeSpan idleTime, TParam argument = null)
        {
            try
            {
                while (ct.IsCancellationRequested == false)
                {
                    while (BackgroundCondition() == false)
                    {
                        await Task.Delay(idleTime, ct);
                    }

                    await RunAsync(argument, ct);
                    await Task.Delay(idleTime, ct);
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore only TaskCanceledException
            }
        }

        /// <summary>
        /// Runs the job as a BackgroundWorker with default idle time of 1 minute
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="argument"></param>
        public void RunBackgroundWork(CancellationToken ct = default(CancellationToken), TParam argument = null)
        {
            RunBackgroundWork(ct, TimeSpan.FromMinutes(1), argument);
        }

        /// <summary>
        /// The Job execution Start Date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The Job last execution End Date
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Executes the job, you shouldn't call this method directly
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        public async Task RunAsync(TParam argument, CancellationToken ct = default(CancellationToken))
        {
            if (IsRunning) return;

            EndDate = null;
            StartDate = DateTime.UtcNow;
            IsRunning = true;

            try
            {
                await DoWork(argument, ct);
            }
            finally
            {
                IsRunning = false;
                EndDate = DateTime.UtcNow;
                StartDate = null;
            }
        }
    }

    /// <summary>
    /// Creates a job with multiple running options
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type, it must inherent IBusinessDbContext</typeparam>
    public abstract class JobBase<TDbContext> : JobBase<TDbContext, object>
        where TDbContext : IBusinessDbContext
    {
        /// <summary>
        /// Creates a new Job, you should call this constructor from base method in your implementation class
        /// </summary>
        /// <param name="context"></param>
        protected JobBase(TDbContext context) : base(context)
        {
        }
    }
}