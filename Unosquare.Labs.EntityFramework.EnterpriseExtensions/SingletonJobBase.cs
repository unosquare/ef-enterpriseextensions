using System;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Log;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public abstract class SingletonJobBase<T, TDbContext, TParam> : JobBase<T, TDbContext, TParam>, IDisposable 
        where TDbContext : IBusinessDbContext where TParam : class where T : class
    {
        /// <summary>
        /// The static, singleton instance reference.
        /// </summary>
        protected static T _instance;

        /// <summary>
        /// Creates a new Job, you should call this constructor from base method in your implementation class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        /// <param name="emailHelper"></param>
        protected SingletonJobBase(TDbContext context, ILog log, IEmailHelper emailHelper = null) : base(context, log, emailHelper)
        {
        }

        /// <summary>
        /// Disposes the job
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the job
        /// </summary>
        /// <param name="disposeManaged"></param>
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
            get { return _instance ?? (_instance = Activator.CreateInstance(typeof(T), true) as T); }
            protected set { _instance = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class SingletonJobBase<T, TDbContext> : SingletonJobBase<T, TDbContext, object>
        where TDbContext : IBusinessDbContext where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        /// <param name="emailHelper"></param>
        protected SingletonJobBase(TDbContext context, ILog log, IEmailHelper emailHelper = null) : base(context, log, emailHelper)
        {
        }
    }
}
