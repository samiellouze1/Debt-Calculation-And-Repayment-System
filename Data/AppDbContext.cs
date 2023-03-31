using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbContext: IdentityDbContext<USER>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating (ModelBuilder builder)
        {
            #region PAYMENT
            builder.Entity<PAYMENT>().HasOne(p => p.PaymentPlan).WithMany(pp => pp.Payments).HasForeignKey(p => p.PaymentPlanId);
            #endregion

            #region PAYMENTPLAN
            builder.Entity<PAYMENTPLAN>().HasOne(pp => pp.User).WithMany(u => u.PaymentPlans).HasForeignKey(pp => pp.UserId);
            builder.Entity<PAYMENTPLAN>().HasOne(pp => pp.UserRegister).WithMany(u => u.PaymentPlans1).HasForeignKey(pp => pp.RegUserId);
            #endregion

            #region USER
            builder.Entity<USER>().HasOne(u => u.User2).WithMany(u => u.UserRegister).HasForeignKey(u => u.RegUserId);
            builder.Entity<USER>().HasOne(u => u.KeyValue).WithMany(kv => kv.USERs).HasForeignKey(u=>u.KeyValue);
            builder.Entity<USER>().HasOne(u => u.KeyValue1).WithMany(kv => kv.USERs1).HasForeignKey(u => u.KeyValue1);
            builder.Entity<USER>().HasOne(u => u.KeyValue2).WithMany(kv => kv.USERs2).HasForeignKey(u => u.KeyValue2);
            builder.Entity<USER>().HasOne(u => u.KeyValue3).WithMany(kv => kv.USERs3).HasForeignKey(u => u.KeyValue3);
            #endregion

            #region SCOLARSHIPDEBT
            builder.Entity<SCOLARSHIPDEBT>().HasOne(sd => sd.User).WithMany(u => u.ScolarshipDebtsHeHas).HasForeignKey(sd => sd.UserId);
            builder.Entity<SCOLARSHIPDEBT>().HasOne(sd => sd.UserRegister).WithMany(u => u.ScolarshipDebtsHeRegistered).HasForeignKey(sd => sd.RegUserId);
            #endregion

            base.OnModelCreating(builder);
        }
        public DbSet<KEYVALUE> KEYVALUEs { get; set; }
        public DbSet<PAYMENT> PAYMENTs { get; set; }
        public DbSet<PAYMENTPLAN> PAYMENTPLANs { get; set; }
        public DbSet<SCOLARSHIPDEBT> SCOLARSHIPDEPTs { get; set; }
        public DbSet<USER> USERs { get; set; }
    }
}
