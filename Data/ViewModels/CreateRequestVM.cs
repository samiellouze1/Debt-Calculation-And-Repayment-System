﻿using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.Build.Framework;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateRequestVM
    {
        [Required]
        public decimal ToBePaidFull { get; set; }
        [Required]
        public int NumOfMonths { get; set;}
    }
}
