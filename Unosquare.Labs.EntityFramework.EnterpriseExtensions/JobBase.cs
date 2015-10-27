using System;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Log;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Creates a job with multiple running options
    /// </summary>
    /// <typeparam name="T">The Job type</typeparam>
    /// <typeparam name="TDbContext">The DbContext type, it must inherent IBusinessDbContext</typeparam>
    /// <typeparam name="TParam">The job input parameter type</typeparam>
    public abstract class JobBase<T, TDbContext, TParam> where T : class
        where TDbContext : IBusinessDbContext
        where TParam : class
    {
        /// <summary>
        /// The BusinessDbContext instance
        /// </summary>
        protected readonly TDbContext Context;

        /// <summary>
        /// The logger instance
        /// </summary>
        protected readonly ILog Log;

        /// <summary>
        /// The email helper instance
        /// </summary>
        protected readonly IEmailHelper EmailHelper;

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
        /// <param name="log"></param>
        /// <param name="emailHelper"></param>
        protected JobBase(TDbContext context, ILog log, IEmailHelper emailHelper = null)
        {
            Context = context;
            Log = log;
            EmailHelper = emailHelper;
        }

        /// <summary>
        /// Check if it's running
        /// </summary>
        public virtual bool IsRunning { get; protected set; }

        /// <summary>
        /// The job work task
        /// </summary>
        /// <param name="argument"></param>
        protected abstract void DoWork(TParam argument);

        /// <summary>
        /// Defines the condition to run job in background
        /// </summary>
        /// <returns></returns>
        protected abstract bool BackgroundCondition();

        /// <summary>
        /// Runs the job as a BackgroundWorker
        /// </summary>
        /// <param name="ct"></param>
        public async void RunBackgroundWork(CancellationToken ct)
        {
            while (ct.IsCancellationRequested == false)
            {
                while (BackgroundCondition() == false)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), ct);
                }

                EndDate = null;
                StartDate = DateTime.UtcNow;
                IsRunning = true;
                DoWork(null);
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
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
        /// Runs the job in a thread if it isn't running
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public Task RunAsync(TParam argument)
        {
            if (IsRunning) return null;

            EndDate = null;
            StartDate = DateTime.UtcNow;
            IsRunning = true;

            return Task.Run(() => InternalRun(argument));
        }

        /// <summary>
        /// Runs the job if it isn't running
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool Run(TParam argument)
        {
            if (IsRunning) return false;

            EndDate = null;
            StartDate = DateTime.UtcNow;
            IsRunning = true;
            InternalRun(argument);

            return true;
        }

        /// <summary>
        /// Executes the job, you shouldn't call this method directly
        /// </summary>
        /// <param name="argument"></param>
        private void InternalRun(TParam argument)
        {
            try
            {
                DoWork(argument);
            }
            catch (Exception ex)
            {
                Log.Error("InternalRun", ex);
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
    /// <typeparam name="T">The Job type</typeparam>
    /// <typeparam name="TDbContext">The DbContext type, it must inherent IBusinessDbContext</typeparam>
    public abstract class JobBase<T, TDbContext> : JobBase<T, TDbContext, object> where T : class
        where TDbContext : IBusinessDbContext
    {
        /// <summary>
        /// Creates a new Job, you should call this constructor from base method in your implementation class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        /// <param name="emailHelper"></param>
        protected JobBase(TDbContext context, ILog log, IEmailHelper emailHelper = null)
            : base(context, log, emailHelper)
        {
        }
    }
}