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
        public DbSet<REQUEST> REQUESTs { get; set; }
        public DbSet<DEBTREGISTER> DEBTREGISTERs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<STUDENT>().HasOne(s => s.StaffMember).WithMany(s => s.Students).OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<STUDENT>().HasOne(s => s.DebtRegister).WithOne(dr => dr.Student).HasForeignKey<DEBTREGISTER>(dr => dr.Id).OnDelete(DeleteBehavior.ClientSetNull);            builder.Entity<DEBTREGISTER>().HasOne(dr => dr.Request).WithOne(r => r.DebtRegister).HasForeignKey<REQUEST>(r => r.Id).OnDelete(DeleteBehavior.ClientSetNull);


            builder.Entity<STAFFMEMBER>().Navigation(sm => sm.Students).AutoInclude();

            builder.Entity<STUDENT>().Navigation(s => s.StaffMember).AutoInclude();
            builder.Entity<STUDENT>().Navigation(s => s.Payments).AutoInclude();
            builder.Entity<STUDENT>().Navigation(s => s.DebtRegister).AutoInclude();

            builder.Entity<DEBTREGISTER>().Navigation(dr => dr.Student).AutoInclude();
            builder.Entity<DEBTREGISTER>().Navigation(dr => dr.Request).AutoInclude();
            builder.Entity<DEBTREGISTER>().Navigation(dr => dr.Debts).AutoInclude();

            builder.Entity<REQUEST>().Navigation(r => r.DebtRegister).AutoInclude();

            builder.Entity<DEBT>().Navigation(d => d.DebtRegister).AutoInclude();
            builder.Entity<DEBT>().Navigation(d => d.Installments).AutoInclude();

            builder.Entity<INSTALLMENT>().Navigation(i => i.Debt).AutoInclude();

            builder.Entity<DEBT>().Property(d => d.InitialAmount).HasPrecision(18, 4);
            builder.Entity<DEBT>().Property(d => d.InterestRate).HasPrecision(18, 4);

            builder.Entity<DEBTREGISTER>().Property(dr => dr.DebtsTotal).HasPrecision(18, 4);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.PaidCash).HasPrecision(18, 4);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.PaidInstallment).HasPrecision(18, 4);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.NotPaidInstallment).HasPrecision(18, 4);
            builder.Entity<DEBTREGISTER>().Property(dr => dr.InterestRate).HasPrecision(18, 4);

            builder.Entity<INSTALLMENT>().Property(i=>i.InitialAmount).HasPrecision(18, 4);
            builder.Entity<INSTALLMENT>().Property(i=>i.AmountAfterInterest).HasPrecision(18, 4);

            builder.Entity<PAYMENT>().Property(p => p.Sum).HasPrecision(18, 4);

            builder.Entity<REQUEST>().Property(r => r.PaidFull).HasPrecision(18, 4);
            builder.Entity<REQUEST>().Property(r => r.InterestRate).HasPrecision(18, 4);



            base.OnModelCreating(builder);
        }

    }
}
