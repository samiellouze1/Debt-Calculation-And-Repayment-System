namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateRequestVM
    {
        public decimal ToBePaidFull { get; set; }
        public decimal ToBePaidInstallment { get; set; }
        public int NumOfMonths { get; set;}
        public decimal InterestRate { get; set; }
        public string Status { get; set; }
    }
}
