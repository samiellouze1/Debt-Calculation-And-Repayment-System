using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbContext: IdentityDbContext<USER>
    {
        public DbSet<PAYMENT> PAYMENTs { get; set; }
        public DbSet<PAYMENTPLAN> PAYMENTPLANs { get; set; }
        public DbSet<SCOLARSHIPDEBT> SCOLARSHIPDEPTs { get; set; }
        public DbSet<USER> USERs { get; set; }
        public DbSet<STUDENT> STUDENTs { get; set; }
        public DbSet<STAFFMEMBER> STAFFMEMBERs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating (ModelBuilder builder)
        {
            #region PAYMENT
            builder.Entity<PAYMENT>().Property(p => p.Total).HasPrecision(18, 4);
            builder.Entity<PAYMENT>().Property(s => s.Amount).HasPrecision(18,4);
            builder.Entity<PAYMENT>().Property(p => p.OverdueAmount).HasPrecision(18, 4);
            builder.Entity<PAYMENT>().HasOne(p => p.ScolarshipDebt).WithMany(sd => sd.Payments).HasForeignKey(p => p.ScolarshipDebtId);

            #endregion

            #region PAYMENTPLAN
            builder.Entity<PAYMENTPLAN>().Property(p => p.PrincipalAmount).HasPrecision(18,4);
            builder.Entity<PAYMENTPLAN>().Property(p => p.MonthlyTotalAmount).HasPrecision(18, 4);
            builder.Entity<PAYMENTPLAN>().Property(p => p.InterestAmount).HasPrecision(18, 4);
            builder.Entity<PAYMENTPLAN>().HasOne(pp => pp.Payment).WithMany(p => p.PaymentPlans).HasForeignKey("PaymentId");
            #endregion

            #region STUDENT
            builder.Entity<STUDENT>().HasOne(u => u.StaffMember).WithMany(u => u.Students).HasForeignKey(u => u.StaffMemberId).OnDelete(DeleteBehavior.ClientSetNull);
            #endregion

            #region SCOLARSHIPDEBT
            builder.Entity<SCOLARSHIPDEBT>().Property(s => s.Interest).HasPrecision(4, 3);
            builder.Entity<SCOLARSHIPDEBT>().Property(s => s.Rate).HasPrecision(4, 3);
            builder.Entity<SCOLARSHIPDEBT>().Property(sd => sd.Amount).HasPrecision(18, 4);
            builder.Entity<SCOLARSHIPDEBT>().HasOne(sd => sd.Student).WithMany(u => u.ScolarshipDebts).HasForeignKey(sd => sd.StudentId);
            #endregion

            base.OnModelCreating(builder);
        }

    }
}
