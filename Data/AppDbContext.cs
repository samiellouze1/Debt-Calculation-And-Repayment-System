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
            builder.Entity<DEBTREGISTER>().HasOne(dr => dr.Request).WithOne(r => r.DebtRegister).HasForeignKey<REQUEST>(r => r.Id);

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

            base.OnModelCreating(builder);
        }

    }
}
