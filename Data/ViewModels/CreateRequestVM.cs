﻿using Debt_Calculation_And_Repayment_System.Data.Annotation;
using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateRequestVM
    {
        [Required(ErrorMessage = "Peşin Ödenecek Tutar*")]
        [Range(0, double.MaxValue, ErrorMessage = "Peşin Ödenecek >=0 *")]
        [LessThanTotal("Total", ErrorMessage = "Peşin ödenecek miktar toplam tutardan küçük olmalı")]
        public decimal ToBePaidFull { get; set; }

        [Required(ErrorMessage = "Taksit Sayısı giriniz(Peşin ödemek için 0)")]
        [Range(0, 60, ErrorMessage = "[0-60]*")]
        public int NumOfMonths { get; set; }

        [Required(ErrorMessage = "Toplam Tutar*")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Taksitli Ödenecek Tutar*")]
        [Range(0, double.MaxValue, ErrorMessage = "Taksitli Ödenecek Tutar >=0*")]
        [LessThanTotal("Total", ErrorMessage = "Taksitli ödencek miktar toplam tutardan küçük olmalı")]
        public decimal ToBePaidInstallment { get; set; }
        public string studentId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FirstInstallmentDate { get; set; }
        public string? Description { get; set; }

    }
}
