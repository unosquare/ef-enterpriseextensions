using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Represents a Business Rules Controller
    /// </summary>
    public interface IBusinessRulesController
    {
        /// <summary>
        /// Handles the SavingChanges event of the context object.
        /// </summary>
        void RunBusinessRules();
    }

    /// <summary>
    /// Provides a base implementation of a business rules controller class
    /// Decorate methods in a derived class with the BusinessRuleAttribute to execute business rules for the given entity types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BusinessRulesController<T> : IBusinessRulesController
        where T : DbContext
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public T Context { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRulesController&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected BusinessRulesController(T context)
        {
            Context = context;
        }

        /// <summary>
        /// Handles the SavingChanges event of the context object.
        /// </summary>
        public void RunBusinessRules()
        {
            var methodInfoSet = GetType().GetMethods().Where(m => m.ReturnType == typeof(void) && m.IsPublic
                                                                  && !m.IsConstructor &&
                                                                  m.GetCustomAttributes(typeof(BusinessRuleAttribute),
                                                                                        true).Any()).ToArray();

            ExecuteBusinessRulesMethods(EntityState.Added, ActionFlags.Create, methodInfoSet);
            ExecuteBusinessRulesMethods(EntityState.Modified, ActionFlags.Update, methodInfoSet);
            ExecuteBusinessRulesMethods(EntityState.Deleted, ActionFlags.Delete, methodInfoSet);
        }

        /// <summary>
        /// Returns the entity type
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Type GetEntityType(object entity)
        {
            var entityType = entity.GetType();
            if (entityType.BaseType != null && entityType.Namespace == Common.DynamicProxiesNamespace)
                entityType = entityType.BaseType;

            return entityType;
        }

        /// <summary>
        /// Executes the business rules methods via reflection and method invocation.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="action">The action.</param>
        /// <param name="methodInfoSet">The method info set.</param>
        private void ExecuteBusinessRulesMethods(EntityState state, ActionFlags action, MethodInfo[] methodInfoSet)
        {
            var selfTrackingEntries = Context.ChangeTracker.Entries().Where(x => x.State == state);

            foreach (var entry in selfTrackingEntries)
            {
                var entity = entry.Entity;
                if (entity == null) continue;
                var entityType = entity.GetType();

                if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
                    entityType = entityType.BaseType;

                var methods = methodInfoSet.Where(m => m.GetCustomAttributes(typeof(BusinessRuleAttribute), true)
                                                        .Select(a => a as BusinessRuleAttribute)
                                                        .Any(
                                                            b =>
                                                            b.EntityTypes.Any(
                                                                t => t == entityType) &&
                                                            (b.Action & action) == action));

                foreach (var methodInfo in methods)
                {
                    methodInfo.Invoke(this, new[] { entity });
                }
            }
        }
    }
}
