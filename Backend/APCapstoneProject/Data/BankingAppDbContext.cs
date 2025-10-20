using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Data
{
    public class BankingAppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ClientUser> ClientUsers { get; set; }
        public DbSet<BankUser> BankUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }

        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<ProofType> ProofTypes { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SalaryDisbursement> SalaryDisbursements { get; set; }
        public DbSet<SalaryDisbursementDetail> SalaryDisbursementDetails { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public BankingAppDbContext(DbContextOptions options) : base(options) { }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Fix all decimal precision (optional, keeps warnings away)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

            // --- Global FIX: Remove all cascade deletes to prevent multiple cascade paths
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }



    }
}
