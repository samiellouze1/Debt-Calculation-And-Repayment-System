using Debt_Calculation_And_Repayment_System.Data.Annotation;
using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateDebtVM
    {
        [Required(ErrorMessage ="Tutar giriniz")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Başlangıç Tarihi *")]
        [DateNotInFuture(ErrorMessage = "Geçmiş bir tarih seçiniz")]
        public DateTime StartDate { get; set; }

        //[Required]
        //[DateGreaterThan("StartDate", ErrorMessage = "Bitiş Tarihi Başlangıç Tarihinden büyük olmalı")]
        //[DateNotInFuture(ErrorMessage = "Geçmiş bir tarih seçiniz")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage ="Bursiyer seçiniz")]
        public string StudentId { get; set; }
        public List<STUDENT>? Students { get; set; }
    }
}
