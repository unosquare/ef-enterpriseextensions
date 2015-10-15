using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Newtonsoft.Json;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.ObjectModel;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Controllers
{
    public class AuditTrailController<T, TEntity> : BusinessRulesController<T>
        where T : DbContext
    {
        private readonly List<Type> _validCreateTypes = new List<Type>();
        private readonly List<Type> _validUpdateTypes = new List<Type>();
        private readonly List<Type> _validDeleteTypes = new List<Type>();
        private readonly string _currentUserId;

        public AuditTrailController(T context, string currentUserId) : base(context)
        {
            _currentUserId = currentUserId;
        }

        /// <summary>
        /// Add type to action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="type"></param>
        public void AddTypes(ActionFlags action, Type type)
        {
            AddTypes(action, new[] {type});
        }

        /// <summary>
        /// Adds types to action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="types"></param>
        public void AddTypes(ActionFlags action, Type[] types)
        {
            switch (action)
            {
                case ActionFlags.None:
                    break;
                case ActionFlags.Create:
                    _validCreateTypes.AddRange(types);
                    break;
                case ActionFlags.Update:
                    _validUpdateTypes.AddRange(types);
                    break;
                case ActionFlags.Delete:
                    _validDeleteTypes.AddRange(types);
                    break;
            }
        }

        [BusinessRule(ActionFlags.Create)]
        public virtual void OnEntityCreated(object entity)
        {
            var entityType = GetEntityType(entity);

            if (_validCreateTypes.Contains(entityType) == false && _validCreateTypes.Any()) return;

            AuditEntry(ActionFlags.Create, entity, entityType.Name);
        }

        [BusinessRule(ActionFlags.Update)]
        public virtual void OnEntityUpdated(object entity)
        {
            var entityType = GetEntityType(entity);

            if (_validUpdateTypes.Contains(entityType) == false && _validUpdateTypes.Any()) return;

            AuditEntry(ActionFlags.Update, entity, entityType.Name);
        }

        [BusinessRule(ActionFlags.Delete)]
        public virtual void OnEntityDeleted(object entity)
        {
            var entityType = GetEntityType(entity);

            if (_validDeleteTypes.Contains(entityType) == false && _validDeleteTypes.Any()) return;

            AuditEntry(ActionFlags.Delete, entity, entityType.Name);
        }

        private void AuditEntry(ActionFlags flag, object entity, string name)
        {
            if (string.IsNullOrWhiteSpace(_currentUserId)) return;

            var entityState =
                ((IObjectContextAdapter) Context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entity);

            var instance = (IAuditTrailEntry) Activator.CreateInstance<TEntity>();
            instance.TableName = name;
            instance.DateCreated = DateTime.UtcNow;
            instance.Action = (int) flag;
            instance.UserId = _currentUserId;

            if (flag != ActionFlags.Delete)
                instance.JsonBody = JsonConvert.SerializeObject(ToDictionary(entityState.CurrentValues));

            Context.Entry(instance).State = EntityState.Added;
        }

        private static Dictionary<string, object> ToDictionary(IDataRecord record)
        {
            var result = new Dictionary<string, object>();

            for (var keyIndex = 0; keyIndex < record.FieldCount; keyIndex++)
            {
                var fieldType = record.GetFieldType(keyIndex);

                if (fieldType == typeof (byte[]))
                    result[record.GetName(keyIndex)] = "(Blob)";
                else if (Common.PrimitiveTypes.Contains(fieldType))
                    result[record.GetName(keyIndex)] = record.GetValue(keyIndex);
            }

            return result;
        }
    }
}