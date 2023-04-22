using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.Annotation
{
    public class LessThanTotalAttribute : ValidationAttribute
    {
        private string _comparisonProperty;

        public LessThanTotalAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (decimal)value;

            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (comparisonPropertyInfo == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            var comparisonValue = (decimal)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (currentValue > comparisonValue)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be less than {comparisonPropertyInfo.Name}");
            }

            return ValidationResult.Success;
        }
    }
}