using AppCommon.EnumShared;

namespace AppCommon.DTOs.Modules
{
    public  class ValidationRuleDto
    {
        public Guid Id { get; set; }
        public RuleTypeEnum RuleType { get; set; }
        public string RuleValue { get; set; }
        public string Configuration { get; set; }
        public string ErrorMessage { get; set; }
        public Guid PropertyId { get; set; }
    }
}
