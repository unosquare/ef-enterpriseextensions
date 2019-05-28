namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Threading;
    using System.Linq;
    using System.Threading.Tasks;

    /// <inheritdoc cref="DbContext" />
    /// <summary>
    /// Creates a new DbContext with support to run BusinessControllers
    /// </summary>
    public abstract class BusinessDbContext : DbContext, IBusinessDbContext
    {
        private readonly List<IBusinessRulesController> _businessControllers = new List<IBusinessRulesController>();

        /// <summary>
        /// Instances a new DbContext with a connection name
        /// </summary>
        protected BusinessDbContext(string connectionName) : base(connectionName)
        {
        }

        /// <summary>
        /// Instances a new DbContext with a DbConnection
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        protected BusinessDbContext(DbConnection dbConnection, bool contextOwnsConnection)
            : base(dbConnection, contextOwnsConnection)
        {
        }

        /// <inheritdoc />
        public void AddController(IBusinessRulesController controller) => _businessControllers.Add(controller);

        /// <inheritdoc />
        public void RemoveController(IBusinessRulesController controller) => _businessControllers.Remove(controller);

        /// <inheritdoc />
        public bool ContainsController(IBusinessRulesController controller) =>
            _businessControllers.Contains(controller);

        /// <inheritdoc />
        public IBusinessRulesController GetInstance<T>() =>
            _businessControllers.FirstOrDefault(x => typeof(T) == x.GetType());

        private async Task RunBusinessRules()
        {
            foreach (var controller in _businessControllers)
            {
                await controller.RunBusinessRules();
            }
        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            RunBusinessRules().GetAwaiter().GetResult();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync()
        {
            await RunBusinessRules();
            return await base.SaveChangesAsync();
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            await RunBusinessRules();
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}