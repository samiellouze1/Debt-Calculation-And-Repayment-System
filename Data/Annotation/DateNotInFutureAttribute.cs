using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.Annotation
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = Convert.ToDateTime(value);
            if (date > DateTime.Now)
            {
                return new ValidationResult("The date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
