using System;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.ObjectModel
{
    /// <summary>
    /// Represents an AuditTrail Entry
    /// </summary>
    public interface IAuditTrailEntry
    {
        /// <summary>
        /// The user identifier
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// The table's name
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// The action
        /// </summary>
        int Action { get; set; }

        /// <summary>
        /// AuditTrail data in JSON
        /// </summary>
        string JsonBody { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        DateTime DateCreated { get; set; }
    }
}
