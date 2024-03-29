﻿using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.Annotation
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (value is DateTime currentValue && comparisonValue is DateTime comparisonDate)
            {
                if (currentValue <= comparisonDate)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
