using Module.Domain.Base;
using Module.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Module.Domain.BusinessDomain
{
    public class Task : BaseModuleData
    {
        public Guid Id { get; set; }
        [NotMapped]
        public string Title => ModuleData.GetPropertyValue<string>(nameof(Title));

        [NotMapped]
        public Guid AssigndTo => ModuleData.GetPropertyValue<Guid>(nameof(AssigndTo));

        [NotMapped]
        public DateOnly DueDate => ModuleData.GetPropertyValue<DateOnly>(nameof(DueDate));
    }
}
