using AppCommon.EnumShared;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;
using System.Text.RegularExpressions;

namespace Module.Service
{
    public class ValidationService
    {
        public ValidationResult Validate(Property property, object value)
        {
            var validationResult = new ValidationResult();
            foreach (var rule in property.ValidationRules)
            {
                switch (rule.RuleType)
                {
                    case RuleTypeEnum.Required:
                        validationResult.AddResult(IsRequired(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.MaxLength:
                        validationResult.AddResult(MaxLength(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.MinLength:
                        validationResult.AddResult(MinLength(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.GreaterThan:
                        validationResult.AddResult(GreaterThan(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.LessThan:
                        validationResult.AddResult(LessThan(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.Between:
                        validationResult.AddResult(Between(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsNumeric:
                        validationResult.AddResult(IsNumeric(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsInteger:
                        validationResult.AddResult(IsInteger(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsPositive:
                        validationResult.AddResult(IsPositive(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsNegative:
                        validationResult.AddResult(IsNegative(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsZero:
                        validationResult.AddResult(IsZero(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.Contains:
                        validationResult.AddResult(Contains(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.StartsWith:
                        validationResult.AddResult(StartsWith(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.EndsWith:
                        validationResult.AddResult(EndsWith(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsEmail:
                        validationResult.AddResult(IsEmail(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsPhoneNumber:
                        validationResult.AddResult(IsPhoneNumber(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.MatchesRegex:
                        validationResult.AddResult(MatchesRegex(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.BeforeDate:
                        validationResult.AddResult(BeforeDate(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.AfterDate:
                        validationResult.AddResult(AfterDate(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.BetweenDates:
                        validationResult.AddResult(BetweenDates(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsDate:
                        validationResult.AddResult(IsDate(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsTrue:
                        validationResult.AddResult(IsTrue(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsFalse:
                        validationResult.AddResult(IsFalse(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.InDomain:
                        validationResult.AddResult(InDomain(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.MaxFileSize:
                        validationResult.AddResult(MaxFileSize(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.AllowedFileExtensions:
                        validationResult.AddResult(AllowedFileExtensions(value, rule), rule.ErrorMessage);
                        break;
                    case RuleTypeEnum.IsUnique:
                        validationResult.AddResult(IsUnique(value, rule), rule.ErrorMessage);
                        break;
                    default:
                        throw new NotSupportedException($"Rule type '{rule.RuleType}' is not supported.");
                }
            }
            return validationResult;
        }


        private static bool MaxLength(object value, ValidationRule rule)
        {
            var str = value as string;
            return str != null && str.Length <= int.Parse(rule.Configuration);
        }

        private static bool MinLength(object value, ValidationRule rule)
        {
            var str = value as string;
            return str != null && str.Length >= int.Parse(rule.Configuration);
        }

        private static bool GreaterThan(object value, ValidationRule rule)
        {
            if (value is IComparable comparableValue)
            {
                return comparableValue.CompareTo(rule.Configuration) > 0;
            }
            return false;
        }

        private static bool LessThan(object value, ValidationRule rule)
        {
            if (value is IComparable comparableValue)
            {
                return comparableValue.CompareTo(rule.Configuration) < 0;
            }
            return false;
        }

        private static bool Between(object value, ValidationRule rule)
        {
            if (value is IComparable comparableValue)
            {
                var limits = rule.Configuration.Split(';');
                return comparableValue.CompareTo(limits[0]) > 0 && comparableValue.CompareTo(limits[1]) < 0;
            }
            return false;
        }

        private static bool InDomain(object value, ValidationRule rule)
        {
            var domainValues = rule.Configuration.Split(';');
            return domainValues.Contains(value.ToString());
        }

        private static bool IsRequired(object value, ValidationRule rule)
        {
            return value != null && !string.IsNullOrEmpty(value.ToString());
        }

        // Numeric rules
        private static bool IsNumeric(object value, ValidationRule rule)
        {
            return double.TryParse(value.ToString(), out _);
        }

        private static bool IsInteger(object value, ValidationRule rule)
        {
            return int.TryParse(value.ToString(), out _);
        }

        private static bool IsPositive(object value, ValidationRule rule)
        {
            if (double.TryParse(value.ToString(), out double numericValue))
            {
                return numericValue > 0;
            }
            return false;
        }

        private static bool IsNegative(object value, ValidationRule rule)
        {
            if (double.TryParse(value.ToString(), out double numericValue))
            {
                return numericValue < 0;
            }
            return false;
        }

        private static bool IsZero(object value, ValidationRule rule)
        {
            if (double.TryParse(value.ToString(), out double numericValue))
            {
                return numericValue == 0;
            }
            return false;
        }

        // Text rules
        private static bool Contains(object value, ValidationRule rule)
        {
            return value.ToString().Contains(rule.RuleValue);
        }

        private static bool StartsWith(object value, ValidationRule rule)
        {
            return value.ToString().StartsWith(rule.RuleValue);
        }

        private static  bool EndsWith(object value, ValidationRule rule)
        {
            return value.ToString().EndsWith(rule.RuleValue);
        }

        private static bool IsEmail(object value, ValidationRule rule)
        {
            var emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
            return emailRegex.IsMatch(value.ToString());
        }

        private static bool IsPhoneNumber(object value, ValidationRule rule)
        {
            var phoneNumberRegex = new Regex(@"^(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$");
            return phoneNumberRegex.IsMatch(value.ToString());
        }

        private static bool MatchesRegex(object value, ValidationRule rule)
        {
            var regex = new Regex(rule.RuleValue);
            return regex.IsMatch(value.ToString());
        }

        // Date rules
        private static bool BeforeDate(object value, ValidationRule rule)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
            {
                if (DateTime.TryParse(rule.RuleValue, out DateTime thresholdDate))
                {
                    return dateValue < thresholdDate;
                }
            }
            return false;
        }

        private static bool AfterDate(object value, ValidationRule rule)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
            {
                if (DateTime.TryParse(rule.RuleValue, out DateTime thresholdDate))
                {
                    return dateValue > thresholdDate;
                }
            }
            return false;
        }

        private static bool BetweenDates(object value, ValidationRule rule)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
            {
                var dates = rule.RuleValue.Split(',').Select(DateTime.Parse).ToList();
                if (dates.Count == 2)
                {
                    return dateValue > dates[0] && dateValue < dates[1];
                }
            }
            return false;
        }

        private static bool IsDate(object value, ValidationRule rule)
        {
            return DateTime.TryParse(value.ToString(), out _);
        }

        // Boolean rules
        private static bool IsTrue(object value, ValidationRule rule)
        {
            return value is bool booleanValue && booleanValue;
        }

        private bool IsFalse(object value, ValidationRule rule)
        {
            return value is bool booleanValue && !booleanValue;
        }

        // Domain rule already defined earlier in previous example

        // Attachment rules: these are more complex, you would need access to the actual file for this
        // You need to adjust these methods based on how your attachments are structured in your application
        private static bool MaxFileSize(object value, ValidationRule rule)
        {
            throw new NotImplementedException("You need to implement this based on your application.");
        }

        private static bool AllowedFileExtensions(object value, ValidationRule rule)
        {
            throw new NotImplementedException("You need to implement this based on your application.");
        }

        // Other rules: this is complex and requires more context on how uniqueness should be checked
        private static bool IsUnique(object value, ValidationRule rule)
        {
            throw new NotImplementedException("You need to implement this based on your application.");
        }
    }


    public class ValidationResult
    {
        public List<string> Errors { get; } = new List<string>();

        public bool IsValid => Errors.Count == 0;

        public void AddResult(bool isValid, string errorMessage)
        {

            if (!isValid)
            {
                Errors.Add(errorMessage);
            }
        }
    }

}
