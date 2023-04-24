using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbContext : IdentityDbContext<USER>
    {
        public DbSet<USER> USERs { get; set; }
        public DbSet<STUDENT> STUDENTs { get; set; }
        public DbSet<STAFFMEMBER> STAFFMEMBERs { get; set; }
        public DbSet<PAYMENT> PAYMENTs { get; set; }
        public DbSet<INSTALLMENT> INSTALLMENTs { get; set; }
        public DbSet<DEBT> DEBTs { get; set; }
        public DbSet<PROGRAMTYPE> PROGRAMTYPESs { get; set; }
        public DbSet<REQUEST> REQUESTs { get; set; }
        public DbSet<DEBTREGISTER> DEBTREGISTERs { get; set; }
        public DbSet<STUDENTSTATUSTYPE> STUDENTSTATUSTYPEs { get; set; }
        public DbSet<EMAILTEMPLATE> EMAILTEMPLATEs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region onetoone
            builder.Entity<STUDENT>().HasOne(s => s.StaffMember).WithMany(s => s.Students).OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<DEBTREGISTER>().HasOne(dr => dr.Student).WithOne(s => s.DebtRegister).HasForeignKey<DEBTREGISTER>(dr => dr.StudentId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<REQUEST>().HasOne(r => r.DebtRegister).WithMany(r => r.Requests).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DEBT>().HasOne(r => r.DebtRegister).WithMany(r => r.Debts).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<INSTALLMENT>().HasOne(r => r.DebtRegister).WithMany(r => r.Installments).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<PAYMENT>().HasOne(r => r.DebtRegister).WithMany(r => r.Payments).OnDelete(DeleteBehavior.Cascade);
            #endregion



            #region precisions
            builder.Entity<DEBT>().Property(d => d.Amount).HasPrecision(10, 2);

            builder.Entity<DEBTREGISTER>().Property(dr => dr.Amount).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.InterestAmount).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.Total).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.ToBePaid).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.TotalCash).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.TotalInstallment).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.ToBePaidCash).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.ToBePaidInstallment).HasPrecision(10, 2);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.InterestRate).HasPrecision(10, 2);

            builder.Entity<INSTALLMENT>().Property(i=>i.AmountAfterInterest).HasPrecision(10, 2);
            builder.Entity<INSTALLMENT>().Property(i=>i.InitialAmount).HasPrecision(10, 2);
            builder.Entity<INSTALLMENT>().Property(i=>i.Rest).HasPrecision(10, 2);

            builder.Entity<PAYMENT>().Property(p => p.Sum).HasPrecision(10, 2);
            builder.Entity<PAYMENT>().Property(p => p.Paid).HasPrecision(10, 2);
            builder.Entity<PAYMENT>().Property(p => p.PrincipalAmount).HasPrecision(10, 2);
            builder.Entity<PAYMENT>().Property(p => p.InterestAmount).HasPrecision(10, 2);

            builder.Entity<REQUEST>().Property(r => r.ToBePaidFull).HasPrecision(10, 2);
            builder.Entity<REQUEST>().Property(r => r.ToBePaidInstallment).HasPrecision(10, 2);

            #endregion
            base.OnModelCreating(builder);
        }

    }
}
