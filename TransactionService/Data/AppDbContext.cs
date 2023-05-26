using Microsoft.EntityFrameworkCore;
using TransactionService.Models;

namespace TransactionService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TransactionModel> Transactions => Set<TransactionModel>();
        public DbSet<TransactionStatusModel> TransactionStatuses => Set<TransactionStatusModel>();
        public DbSet<TransactionTypeModel> TransactionTypes => Set<TransactionTypeModel>();
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TransactionStatusModel>()
            .HasData(
                new TransactionStatusModel(){
                    TransactionStatusId = 1,
                    Name = "pending"
                },
                new TransactionStatusModel(){
                    TransactionStatusId = 2,
                    Name = "approved"
                },
                new TransactionStatusModel(){
                    TransactionStatusId = 3,
                    Name = "rejected"
                }
            );

            modelBuilder.Entity<TransactionTypeModel>()
            .HasData(
                new TransactionTypeModel(){
                    TransactionTypeId = 1,
                    Name = "payment"
                },
                new TransactionTypeModel(){
                    TransactionTypeId = 2,
                    Name = "credit"
                }
            );
        }

    }
}