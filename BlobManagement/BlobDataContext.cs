using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sepidar.BlobManagement
{
    public class BlobDataContext : DbContext
    {
        //static BlobDataContext()
        //{
        //    Database.SetInitializer<BlobDataContext>(null);
        //}

        //public BlobDataContext()
        //    : base("Blobs")
        //{
        //}

        public DbSet<Blob> Blobs { get; set; }

        public DbSet<BlobView> BlobViews { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Blob>()
        //        .HasKey(t => t.Id);
        //    modelBuilder.Entity<Blob>().
        //        Property(d => d.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        //    modelBuilder.Entity<BlobView>()
        //        .HasKey(t => t.Id);

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
