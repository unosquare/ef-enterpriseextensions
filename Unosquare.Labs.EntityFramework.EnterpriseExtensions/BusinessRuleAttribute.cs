using System;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Defines a combination of actions in a CRUD pattern
    /// </summary>
    [Flags]
    public enum ActionFlags
    {
        /// <summary>
        /// None action
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Create action
        /// </summary>
        Create = 0x1,
        /// <summary>
        /// Update action
        /// </summary>
        Update = 0x2,
        /// <summary>
        /// Delete action
        /// </summary>
        Delete = 0x4,
    }

    /// <summary>
    /// Decorate methods with this attribute to execute business rules that match the following signature:
    /// public void MethodName(T entity)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BusinessRuleAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the entity types the decorated method handles.
        /// </summary>
        /// <value>
        /// The entity types.
        /// </value>
        public Type[] EntityTypes { get; protected set; }

        /// <summary>
        /// Gets or sets the action flags that the decorated method handles.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public ActionFlags Action { get; protected set; }

        /// <summary>
        /// Specifies the types and actions that a method handles as a business rule
        /// </summary>
        /// <param name="entityTypes">The entity types the method can handle.</param>
        /// <param name="actionFlags">The action flags the method can handle.</param>
        public BusinessRuleAttribute(Type[] entityTypes, ActionFlags actionFlags)
        {
            this.EntityTypes = entityTypes;
            this.Action = actionFlags;
        }

        /// <summary>
        /// Specifies the types and actions that a method handles as a business rule
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="actionFlags">The action flags the method can handle..</param>
        public BusinessRuleAttribute(Type entityType, ActionFlags actionFlags)
            : this(new[] { entityType }, actionFlags)
        {
        }
    }
}
