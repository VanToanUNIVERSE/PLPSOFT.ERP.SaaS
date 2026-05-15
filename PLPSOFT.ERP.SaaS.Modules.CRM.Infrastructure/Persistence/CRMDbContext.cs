using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence
{
    public class CRMDbContext : DbContext
    {
        public CRMDbContext(DbContextOptions<CRMDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerSchedule> CustomerSchedules { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SystemType> SystemTypes { get; set; }
        public DbSet<SystemTypeValue> SystemTypeValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CustomerSchedule: nhiều FK cùng trỏ về Users → tắt cascade delete tránh vòng lặp
            modelBuilder.Entity<CustomerSchedule>(entity =>
            {
                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedToUser)
                      .WithMany()
                      .HasForeignKey(e => e.AssignedToUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ScheduleType)
                      .WithMany()
                      .HasForeignKey(e => e.ScheduleTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                      .WithMany()
                      .HasForeignKey(e => e.StatusID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Global query filter: bỏ qua các bản ghi đã xóa mềm
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}
