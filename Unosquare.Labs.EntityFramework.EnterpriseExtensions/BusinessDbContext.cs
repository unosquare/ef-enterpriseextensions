using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Creates a new DbContext with support to run BusinessControllers
    /// </summary>
    public abstract class BusinessDbContext : DbContext
    {
        private List<IBusinessRulesController> BusinessControllers = new List<IBusinessRulesController>();
        private DbConnection dbConnection;

        /// <summary>
        /// Instances a new DbContext
        /// </summary>
        protected BusinessDbContext() : base()
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

        /// <summary>
        /// Adds a new BusinessController to the DbContext
        /// </summary>
        /// <param name="controller"></param>
        public void AddController(IBusinessRulesController controller)
        {
            BusinessControllers.Add(controller);
        }

        /// <summary>
        /// Removes a new BusinessController to the DbContext
        /// </summary>
        /// <param name="controller"></param>
        public void RemoveController(IBusinessRulesController controller)
        {
            BusinessControllers.Remove(controller);
        }

        /// <summary>
        /// Save Changes and run all the business controllers
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            foreach (var controller in BusinessControllers)
                controller.RunBusinessRules();

            return base.SaveChanges();
        }
    }
}