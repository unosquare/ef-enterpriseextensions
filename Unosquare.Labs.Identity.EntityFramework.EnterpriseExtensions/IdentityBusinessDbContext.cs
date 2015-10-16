using System.Collections.Generic;
using System.Data.Common;
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
        /// Save Changes and run all the business controllers
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            foreach (var controller in _businessControllers)
                controller.RunBusinessRules();

            return base.SaveChanges();
        }
    }

    /// <summary>
    /// Creates a new IdentityDbContext with support to run BusinessControllers
    /// </summary>
    public abstract class IdentityBusinessDbContext : IdentityBusinessDbContext<IdentityUser>
    {
    }
}