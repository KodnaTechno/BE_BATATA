namespace Module.Domain.Shared
{
    public enum RuleTypeEnum
    {
        // General rules
        Required,
        MaxLength,
        MinLength,

        // Numeric rules
        GreaterThan,
        LessThan,
        Between,
        IsNumeric,
        IsInteger,
        IsPositive,
        IsNegative,
        IsZero,

        // Text rules
        Contains,
        StartsWith,
        EndsWith,
        IsEmail,
        IsPhoneNumber,
        MatchesRegex,

        // Date rules
        BeforeDate,
        AfterDate,
        BetweenDates,
        IsDate,

        // Boolean rules
        IsTrue,
        IsFalse,

        // Domain rule
        InDomain,

        // Attachment rules
        MaxFileSize,
        AllowedFileExtensions,

        // Other rules
        IsUnique

    }
}
