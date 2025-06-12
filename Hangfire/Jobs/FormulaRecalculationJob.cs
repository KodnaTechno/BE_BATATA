using Module.Service;
using System;

namespace Hangfire.Api.Jobs
{
    public class FormulaRecalculationJob : BaseJob
    {
        private readonly FormulaCalculationService _formulaService;
        public FormulaRecalculationJob(FormulaCalculationService formulaService)
        {
            _formulaService = formulaService;
        }
        protected override string JobName => "FormulaRecalculationJob";
        protected override string CronExpression => "0 2 * * *"; // Every day at 2 AM

        public override void Execute()
        {
            // Fire and forget, or await if you make Execute async
            _formulaService.RecalculateAllSystemPropertiesAsync().GetAwaiter().GetResult();
        }
    }
}
