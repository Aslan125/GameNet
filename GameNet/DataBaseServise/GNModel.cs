namespace GameNet.DataBaseServise
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GNModel : DbContext
    {
        public GNModel()
            : base("name=GNModel")
        {
        }

        public virtual DbSet<accounts> accounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<accounts>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<accounts>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}
