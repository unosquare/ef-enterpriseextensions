namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database
{
    using System.Data.Common;
    using System.Data.Entity;
    using Controllers;

    public class SampleDb : BusinessDbContext
    {
        public SampleDb() : base(nameof(SampleDb))
        {
        }

        public SampleDb(DbConnection connection, string userid) : base(connection, true)
        {
            UserId = userid;
            this.UseAuditTrail<SampleDb, AuditTrailEntry>(userid);
        }

        public string UserId { get; set; }

        public DbSet<AuditTrailEntry> AuditTrailEntries { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}