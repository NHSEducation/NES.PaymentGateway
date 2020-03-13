using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using PaymentGateway.Model.PaymentGateway.Maps;


namespace PaymentGateway.Model.PaymentGateway.Context
{
    public class PGContext : DbContext, IDbContext
    {
        #region properties

        public DbSet<PaymentOrder> PaymentOrders { get; set; }
        public DbSet<PaymentTransactionLog> PaymentTransactionLogs { get; set; }
        public DbSet<RegisteredUser> RegisterUsers { get; set; }
        public DbSet<RegisteredApplication> RegisterApplications { get; set; }

        #endregion

        #region constructors

        public PGContext() : base("name=PaymentGatewayDb")
        {
        }

        public PGContext(string connectionStringName) : base(connectionStringName)
        {
        }

        #endregion

        #region overrides

        /// <summary>
        /// When creating the model, add any custom entity maps to the configuration
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // add entity maps to configuration
            modelBuilder.Configurations.Add(new PaymentOrderMap());
            modelBuilder.Configurations.Add(new PaymentTransactionLogMap());
            modelBuilder.Configurations.Add(new RegisteredApplicationMap());
            modelBuilder.Configurations.Add(new RegisteredUserMap());

            base.OnModelCreating(modelBuilder);
        }

        #endregion

        #region methods

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        #endregion
    }
}
