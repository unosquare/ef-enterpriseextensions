using System;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Log;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    public abstract class JobBase<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// The static, singleton instance reference.
        /// </summary>
        protected static T _instance;

        protected readonly IBusinessDbContext Context;
        protected readonly ILog Log;
        protected readonly IEmailHelper EmailHelper;

        protected JobBase()
        {
            throw new InvalidOperationException("You need to call constructor with params in your implementation");
        }

        protected JobBase(IBusinessDbContext context, ILog log, IEmailHelper emailHelper = null)
        {
            Context = context;
            Log = log;
            EmailHelper = emailHelper;
        }

        public virtual bool IsRunning { get; protected set; }

        protected abstract void DoWork(object argument);

        protected abstract bool BackgroundCondition();

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

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public void RunAsync(object argument)
        {
            if (IsRunning) return;

            EndDate = null;
            StartDate = DateTime.UtcNow;
            IsRunning = true;

            Task.Run(() => InternalRun(argument));
        }

        public bool Run(object argument)
        {
            if (IsRunning) return false;

            EndDate = null;
            StartDate = DateTime.UtcNow;
            IsRunning = true;
            InternalRun(argument);

            return true;
        }

        protected void InternalRun(object argument)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                // free managed resources
                if (_instance != null)
                {
                    _instance = null;
                }
            }
        }

        /// <summary>
        /// Gets the instance that this singleton represents.
        /// If the instance is null, it is constructed ans assigned when this member is accessed.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static T Instance
        {
            get { return _instance ?? (_instance = Activator.CreateInstance(typeof (T), true) as T); }
            protected set { _instance = value; }
        }
    }
}