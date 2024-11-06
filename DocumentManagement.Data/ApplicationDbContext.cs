using DocumentManagement.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<DocumentDetails> DocumentDetails { get; set; }
        public DbSet<Documents> Documents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documents>()
          .HasOne(d => d.DocumentDetails)
          .WithMany(dd => dd.Documents)
          .HasForeignKey(d => d.DocumentDetailId)
          .IsRequired();
        }
    }
}
