namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// Creates a new IdentityDbContext with support to run BusinessControllers
    /// </summary>
    public abstract class IdentityBusinessDbContext<TUser> : IdentityDbContext<TUser>, IBusinessDbContext
        where TUser : IdentityUser
    {
        private readonly List<IBusinessRulesController> _businessControllers = new List<IBusinessRulesController>();

        /// <summary>
        /// Instances a new DbContext with a connection name
        /// </summary>
        protected IdentityBusinessDbContext(string connectionName)
            : base(connectionName)
        {
        }

        /// <summary>
        /// Instances a new DbContext with a connection name and flag
        /// </summary>
        protected IdentityBusinessDbContext(string connectionName, bool throwIfV1Schema)
            : base(connectionName, throwIfV1Schema)
        {
        }

        /// <summary>
        /// Instances a new DbContext with a DbConnection
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        protected IdentityBusinessDbContext(DbConnection dbConnection, bool contextOwnsConnection)
            : base(dbConnection, contextOwnsConnection)
        {
        }

        /// <inheritdoc />
        public void AddController(IBusinessRulesController controller) => _businessControllers.Add(controller);

        /// <inheritdoc />
        public void RemoveController(IBusinessRulesController controller) => _businessControllers.Remove(controller);

        /// <inheritdoc />
        public bool ContainsController(IBusinessRulesController controller) => _businessControllers.Contains(controller);

        /// <inheritdoc />
        public IBusinessRulesController GetInstance<T>() =>
            _businessControllers.FirstOrDefault(x => typeof(T) == x.GetType());

        private void RunBusinessRules()
        {
            foreach (var controller in _businessControllers)
            {
                controller.RunBusinessRules();
            }
        }

        /// <summary>
        /// Save Changes and run all the business controllers
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            RunBusinessRules();
            return base.SaveChanges();
        }

        /// <summary>
        /// Save Changes Async and run all the business controllers
        /// </summary>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync()
        {
            RunBusinessRules();
            return base.SaveChangesAsync();
        }

        /// <summary>
        /// Save Changes Async and run all the business controllers
        /// </summary>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            RunBusinessRules();
            return base.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Creates a new IdentityDbContext with support to run BusinessControllers
    /// </summary>
    public abstract class IdentityBusinessDbContext : IdentityBusinessDbContext<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityBusinessDbContext"/> class.
        /// </summary>
        /// <param name="connectionName"></param>
        protected IdentityBusinessDbContext(string connectionName)
            : base(connectionName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityBusinessDbContext"/> class.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="throwIfV1Schema"></param>
        protected IdentityBusinessDbContext(string connectionName, bool throwIfV1Schema)
            : base(connectionName, throwIfV1Schema)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityBusinessDbContext"/> class.
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        protected IdentityBusinessDbContext(DbConnection dbConnection, bool contextOwnsConnection)
            : base(dbConnection, contextOwnsConnection)
        {
        }
    }
}
