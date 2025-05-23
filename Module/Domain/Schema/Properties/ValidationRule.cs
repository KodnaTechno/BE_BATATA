﻿using AppCommon.EnumShared;

namespace Module.Domain.Schema.Properties
{
    public class ValidationRule
    {
        public Guid Id { get; set; }
        public RuleTypeEnum RuleType { get; set; }
        public string RuleValue { get; set; }
        public string Configuration { get; set; }
        public string ErrorMessage { get; set; }
        public Guid PropertyId { get; set; }
        public virtual Property Property { get; set; }
    }
}
