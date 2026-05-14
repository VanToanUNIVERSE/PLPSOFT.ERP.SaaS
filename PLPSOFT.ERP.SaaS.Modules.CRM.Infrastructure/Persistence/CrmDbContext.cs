using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence
{
    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
        {
        }

        // ===== CRM Schema =====
        public DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }

        // ===== Shared / dbo Schema =====
        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SystemType> SystemTypes { get; set; }
        public DbSet<SystemTypeValue> SystemTypeValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== dbo Schema =====
            modelBuilder.Entity<Company>(e =>
            {
                e.ToTable("Companies", "dbo");
                e.HasKey(x => x.CompanyID);
                e.Property(x => x.CompanyCode).HasMaxLength(50).IsRequired();
                e.Property(x => x.CompanyName).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<Branch>(e =>
            {
                e.ToTable("Branches", "dbo");
                e.HasKey(x => x.BranchID);
                e.Property(x => x.BranchCode).HasMaxLength(50).IsRequired();
                e.Property(x => x.BranchName).HasMaxLength(255).IsRequired();
                e.HasOne(x => x.Company)
                 .WithMany()
                 .HasForeignKey(x => x.CompanyID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("Customers", "dbo");
                e.HasKey(x => x.CustomerID);
                e.Property(x => x.CustomerCode).HasMaxLength(50).IsRequired();
                e.Property(x => x.CustomerName).HasMaxLength(255).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(20);
                e.Property(x => x.Email).HasMaxLength(100);
                e.HasOne(x => x.Company)
                 .WithMany()
                 .HasForeignKey(x => x.CompanyID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users", "dbo");
                e.HasKey(x => x.UserID);
                e.Property(x => x.UserName).HasMaxLength(100).IsRequired();
                e.Property(x => x.FullName).HasMaxLength(255).IsRequired();
                e.HasOne(x => x.Company)
                 .WithMany()
                 .HasForeignKey(x => x.CompanyID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SalesInvoice>(e =>
            {
                e.ToTable("SalesInvoices", "dbo");
                e.HasKey(x => x.InvoiceID);
                e.Property(x => x.InvoiceCode).HasMaxLength(50).IsRequired();
                e.Property(x => x.TotalAmount).HasPrecision(18, 2);
                e.HasOne(x => x.Customer)
                 .WithMany()
                 .HasForeignKey(x => x.CustomerID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SystemType>(e =>
            {
                e.ToTable("SystemTypes", "dbo");
                e.HasKey(x => x.TypeID);
                e.Property(x => x.TypeCode).HasMaxLength(50).IsRequired();
                e.Property(x => x.TypeName).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<SystemTypeValue>(e =>
            {
                e.ToTable("SystemTypeValues", "dbo");
                e.HasKey(x => x.TypeValueID);
                e.Property(x => x.ValueCode).HasMaxLength(50).IsRequired();
                e.Property(x => x.ValueName).HasMaxLength(100).IsRequired();
                e.HasOne(x => x.Type)
                 .WithMany(x => x.Values)
                 .HasForeignKey(x => x.TypeID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== crm Schema =====
            modelBuilder.Entity<CustomerFeedback>(e =>
            {
                e.ToTable("CustomerFeedbacks", "crm");
                e.HasKey(x => x.FeedbackID);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Content).IsRequired();
                e.Property(x => x.Rating).HasDefaultValue(5);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("sysdatetime()");
                e.Property(x => x.IsDeleted).HasDefaultValue(false);

                e.HasOne(x => x.Company)
                 .WithMany()
                 .HasForeignKey(x => x.CompanyID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Branch)
                 .WithMany()
                 .HasForeignKey(x => x.BranchID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Customer)
                 .WithMany()
                 .HasForeignKey(x => x.CustomerID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Invoice)
                 .WithMany()
                 .HasForeignKey(x => x.InvoiceID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.FeedbackType)
                 .WithMany()
                 .HasForeignKey(x => x.FeedbackTypeID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Priority)
                 .WithMany()
                 .HasForeignKey(x => x.PriorityID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Status)
                 .WithMany()
                 .HasForeignKey(x => x.StatusID)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.AssignedToUser)
                 .WithMany()
                 .HasForeignKey(x => x.AssignedToUserID)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
