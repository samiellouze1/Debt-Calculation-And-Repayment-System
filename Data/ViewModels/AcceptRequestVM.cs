namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class AcceptRequestVM
    {
        public string Id { get; set; }
        public bool Accept { get; set; }
        public decimal ToBePaidFull { get; set; }
        public decimal ToBePaidInstallment { get; set; }
        public decimal NumOfMonths { get; set; }
        public decimal ToBePaidEachMonth { get; set; }
        public decimal Total { get; set; }
    }
}
