namespace GameNet.DataBaseServise
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataBase : DbContext
    {
        public DataBase()
            : base("name=GNModel")
        {
        }

        public virtual DbSet<Accounts> accounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<Accounts>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}
