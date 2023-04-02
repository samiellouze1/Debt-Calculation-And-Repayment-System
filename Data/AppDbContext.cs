using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbContext: IdentityDbContext<USER>
    {
        public DbSet<KEYVALUE> KEYVALUEs { get; set; }
        public DbSet<PAYMENT> PAYMENTs { get; set; }
        public DbSet<PAYMENTPLAN> PAYMENTPLANs { get; set; }
        public DbSet<SCOLARSHIPDEBT> SCOLARSHIPDEPTs { get; set; }
        public DbSet<USER> USERs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating (ModelBuilder builder)
        {
            #region SCOLARSHIPDEBT
            builder.Entity<SCOLARSHIPDEBT>().Property(s => s.Interest).HasPrecision(4,3);
            builder.Entity<SCOLARSHIPDEBT>().Property(s => s.Rate).HasPrecision(4,3);
            #endregion

            #region PAYMENT
            builder.Entity<PAYMENT>().Property(p => p.Total).HasPrecision(18, 4);
            builder.Entity<PAYMENT>().Property(s => s.Amount).HasPrecision(18,4);
            builder.Entity<PAYMENT>().Property(p => p.OverdueAmount).HasPrecision(18, 4);
            builder.Entity<PAYMENT>().HasOne(p => p.PaymentPlan).WithMany(pp => pp.Payments).HasForeignKey(p => p.PaymentPlanId);
            #endregion

            #region PAYMENTPLAN
            builder.Entity<PAYMENTPLAN>().Property(p => p.PrincipalAmount).HasPrecision(18,4);
            builder.Entity<PAYMENTPLAN>().Property(p => p.MonthlyTotalAmount).HasPrecision(18, 4);
            builder.Entity<PAYMENTPLAN>().Property(p => p.InterestAmount).HasPrecision(18, 4);
            builder.Entity<PAYMENTPLAN>().HasOne(pp => pp.User).WithMany(u => u.PaymentPlans).HasForeignKey(pp => pp.UserId).OnDelete(DeleteBehavior.ClientNoAction);
            builder.Entity<PAYMENTPLAN>().HasOne(pp => pp.UserRegister).WithMany(u => u.PaymentPlans1).HasForeignKey(pp => pp.RegUserId).OnDelete(DeleteBehavior.ClientNoAction);
            #endregion

            #region USER
            builder.Entity<USER>().Property(u => u.Rate).HasPrecision(4, 3);
            builder.Entity<USER>().HasOne(u => u.User2).WithMany(u => u.UserRegister).HasForeignKey(u => u.RegUserId).OnDelete(DeleteBehavior.ClientNoAction);
            builder.Entity<USER>().HasOne(u => u.KeyValue).WithMany(kv => kv.USERs).HasForeignKey(u => u.KeyValueId);
            builder.Entity<USER>().HasOne(u => u.KeyValue1).WithMany(kv => kv.USERs1).HasForeignKey(u => u.KeyValueId1).OnDelete(DeleteBehavior.ClientNoAction);
            builder.Entity<USER>().HasOne(u => u.KeyValue2).WithMany(kv => kv.USERs2).HasForeignKey(u => u.KeyValueId2).OnDelete(DeleteBehavior.ClientNoAction);
            builder.Entity<USER>().HasOne(u => u.KeyValue3).WithMany(kv => kv.USERs3).HasForeignKey(u => u.KeyValueId3).OnDelete(DeleteBehavior.ClientNoAction);
            #endregion

            #region SCOLARSHIPDEBT
            builder.Entity<SCOLARSHIPDEBT>().Property(sd => sd.Amount).HasPrecision(18, 4);
            builder.Entity<SCOLARSHIPDEBT>().HasOne(sd => sd.User).WithMany(u => u.ScolarshipDebtsHeHas).HasForeignKey(sd => sd.UserId).OnDelete(DeleteBehavior.ClientNoAction);
            builder.Entity<SCOLARSHIPDEBT>().HasOne(sd => sd.UserRegister).WithMany(u => u.ScolarshipDebtsHeRegistered).HasForeignKey(sd => sd.RegUserId).OnDelete(DeleteBehavior.ClientNoAction);
            #endregion

            base.OnModelCreating(builder);
        }

    }
}
