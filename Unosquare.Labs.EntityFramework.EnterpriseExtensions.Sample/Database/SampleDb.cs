using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Controllers;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database
{
    public class SampleDb : BusinessDbContext
    {
        public string UserId { get; set; }

        public SampleDb(DbConnection connection, string userid) : base(connection, true)
        {
            UserId = userid;
            this.UseAuditTrail<SampleDb, AuditTrailEntry>(userid);
        }

        public DbSet<AuditTrailEntry> AuditTrailEntrys { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}