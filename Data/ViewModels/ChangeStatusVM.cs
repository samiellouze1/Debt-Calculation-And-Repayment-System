using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class ChangeStatusVM
    {
        public string Id { get; set; }
        public string Type { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? FirstInstallmentDate { get; set; }
        public List<STUDENTSTATUSTYPE>? statuses { get; set; }
        public List<PROGRAMTYPE>? programes { get; set; }

        public STUDENT student { get; set; }
    }
}
