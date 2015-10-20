﻿using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Creates a new IdentityDbContext with support to run BusinessControllers
    /// </summary>
    public abstract class IdentityBusinessDbContext<TUser> : IdentityDbContext<TUser>, IBusinessDbContext
        where TUser : IdentityUser
    {
        private readonly List<IBusinessRulesController> _businessControllers = new List<IBusinessRulesController>();

        /// <summary>
        /// Instances a new DbContext
        /// </summary>
        protected IdentityBusinessDbContext() : base()
        {
        }

        /// <summary>
        /// Instances a new DbContext with a connection name
        /// </summary>
        protected IdentityBusinessDbContext(string connectionName) : base(connectionName)
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

        /// <summary>
        /// Adds a new BusinessController to the DbContext
        /// </summary>
        /// <param name="controller"></param>
        public void AddController(IBusinessRulesController controller)
        {
            _businessControllers.Add(controller);
        }

        /// <summary>
        /// Removes a new BusinessController to the DbContext
        /// </summary>
        /// <param name="controller"></param>
        public void RemoveController(IBusinessRulesController controller)
        {
            _businessControllers.Remove(controller);
        }

        /// <summary>
        /// Checks if a BusinessController exists
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool ContainsController(IBusinessRulesController controller)
        {
            return _businessControllers.Contains(controller);
        }

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
    }
}