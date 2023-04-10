using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbContext: IdentityDbContext<USER>
    {
        public DbSet<USER> USERs { get; set; }
        public DbSet<STUDENT> STUDENTs { get; set; }
        public DbSet<STAFFMEMBER> STAFFMEMBERs { get; set; }
        public DbSet<PAYMENTPLAN> PAYMENTPLANs { get; set; }
        public DbSet<PAYMENTPLANINSTALLMENT> PAYMENTPLANINSTALLMENTs { get; set; }
        public DbSet<PAYMENTPLANFULL> PAYMENTPLANFULLs { get; set; }
        public DbSet<INSTALLMENT> INSTALLMENTs { get; set; }
        public DbSet<DEBT> DEBTs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating (ModelBuilder builder)
        {
            builder.Entity<STUDENT>().HasOne(s => s.StaffMember).WithMany(sm => sm.Students).HasForeignKey(s => s.StaffMemberId).OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<DEBT>().HasOne(d => d.Student).WithMany(s => s.Debts).HasForeignKey(d => d.StudentId);
            builder.Entity<PAYMENTPLAN>().HasOne(pp => pp.Debt).WithMany(d => d.PaymentPlans).HasForeignKey(pp => pp.DebtId);
            builder.Entity<INSTALLMENT>().HasOne(i => i.PaymentPlanInstallment).WithMany(ppi => ppi.Installments).HasForeignKey(i => i.PaymentPlanInstallmentId);
            
            builder.Entity<STAFFMEMBER>().Navigation(sm => sm.Students).AutoInclude();
            builder.Entity<STUDENT>().Navigation(s => s.Debts).AutoInclude();
            builder.Entity<DEBT>().Navigation(d => d.PaymentPlans).AutoInclude();
            builder.Entity<PAYMENTPLANINSTALLMENT>().Navigation(pi => pi.Installments).AutoInclude();


            builder.Entity<DEBT>().Property(d => d.InitialAmount).HasPrecision(18, 4);
            builder.Entity<DEBT>().Property(d => d.InterestRate).HasPrecision(4, 3);
            builder.Entity<PAYMENTPLAN>().Property(pp=>pp.Amount).HasPrecision(18, 4);
            builder.Entity<INSTALLMENT>().Property(i => i.Amount).HasPrecision(18, 4);

            base.OnModelCreating(builder);
        }

    }
}
