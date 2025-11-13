using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APCapstoneProject.Data
{
    public class BankingAppDbContext : DbContext
    {
        public BankingAppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ClientUser> ClientUsers { get; set; }
        public DbSet<BankUser> BankUsers { get; set; }
 
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<ProofType> ProofTypes { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }

        public DbSet<ReportRecord> ReportRecords { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<SalaryDisbursement> SalaryDisbursements { get; set; }
        public DbSet<SalaryDisbursementDetail> SalaryDisbursementDetails { get; set; }

        public DbSet<Status> Statuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Precision fix for all decimals ---
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

            //  Restrict cascade delete behavior 
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // SEED DATA SECTION


            //User Roles
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(ur => ur.UserRoleId).ValueGeneratedNever();
                entity.HasData(
                    new UserRole { UserRoleId = 0, Role = Role.SUPER_ADMIN },
                    new UserRole { UserRoleId = 1, Role = Role.BANK_USER },
                    new UserRole { UserRoleId = 2, Role = Role.CLIENT_USER }
                );
            });

            //Statuses (for users, accounts, etc.)
            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(s => s.StatusId).ValueGeneratedNever();
                entity.HasData(
                    new Status { StatusId = 0, StatusEnum = StatusEnum.PENDING },
                    new Status { StatusId = 1, StatusEnum = StatusEnum.APPROVED },
                    new Status { StatusId = 2, StatusEnum = StatusEnum.REJECTED }
                );
            });

            // proof types
            modelBuilder.Entity<ProofType>(entity =>
            {
                entity.Property(pt => pt.ProofTypeId).ValueGeneratedNever();
                entity.HasData(
                    new ProofType { ProofTypeId = 0, Type = DocProofType.BUSINESS_REGISTRATION },
                    new ProofType { ProofTypeId = 1, Type = DocProofType.TAX_ID_PROOF },
                    new ProofType { ProofTypeId = 2, Type = DocProofType.PROOF_OF_ADDRESS },
                    new ProofType { ProofTypeId = 3, Type = DocProofType.OTHER }
                );
            });


            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.Property(t => t.TransactionTypeId).ValueGeneratedNever();
                entity.HasData(
                    new TransactionType { TransactionTypeId = 0, Type = TxnType.CREDIT },
                    new TransactionType { TransactionTypeId = 1, Type = TxnType.DEBIT }
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
