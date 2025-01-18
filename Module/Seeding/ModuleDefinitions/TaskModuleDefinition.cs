using AppCommon.DTOs;
using AppCommon.EnumShared;
using AppCommon.GlobalHelpers;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;

namespace Module.Seeding.ModuleDefinitions
{
    public class TaskModuleDefinition : BaseModuleDefinition
    {
        public override Domain.Schema.Module GetModule()
        {
            return new Domain.Schema.Module
            {
                Id = SystemModuleConstants.Basic.TaskModule.Id,
                ApplicationId = SystemApplicationConstants.AssetManagementApplicationId,
                Title = new TranslatableValue { Ar = "Tasks", En = "المهام"}.AsText(),
                Name = nameof(Domain.BusinessDomain.Task),
                Domain = typeof(Domain.BusinessDomain.Task).FullName,
                Type = ModuleTypeEnum.Basic,
                IsActive = true,
                Order = 1,
                Details = new TranslatableValue { Ar = "Details", En = "تفاصيل" }.AsText(),
                CreatedAt = _seedDate,
                CreatedBy = _systemUserId
            };
        }

        public override IEnumerable<Property> GetProperties()
        {
            var moduleId = SystemModuleConstants.Basic.TaskModule.Id;

            var properties = new List<Property>
            {
                 CreateSystemProperty(
                    SystemModuleConstants.Basic.TaskModule.Properties.TitleId,
                    new TranslatableValue { En = "Title", Ar = "العنوان" },
                    "Task_Title",
                    ViewTypeEnum.Text,
                    DataTypeEnum.String,
                    1,
                    null,
                    nameof(Domain.BusinessDomain.Task.Title),
                    nameof(Domain.BusinessDomain.Task.Title),
                    moduleId,
                    true
                ),

                 CreateSystemProperty(
                    SystemModuleConstants.Basic.TaskModule.Properties.AssignedToId,
                    new TranslatableValue { En = "Assigned To", Ar = "مسند إلى" },
                    "Task_AssignedTo",
                    ViewTypeEnum.User,
                    DataTypeEnum.Guid,
                    2,
                    null,
                    nameof(Domain.BusinessDomain.Task.AssigndTo),
                    nameof(Domain.BusinessDomain.Task.AssigndTo),
                    moduleId
                ),

                CreateSystemProperty(
                    SystemModuleConstants.Basic.TaskModule.Properties.DueDateId,
                    new TranslatableValue { En = "Due Date", Ar = "تاريخ الاستحقاق" },
                    "Task_DueDate",
                    ViewTypeEnum.Date,
                    DataTypeEnum.DateOnly,
                    3,
                    null,
                    nameof(Domain.BusinessDomain.Task.DueDate),
                    nameof(Domain.BusinessDomain.Task.DueDate),
                    moduleId
                )
            };

            var commonProperties = GetCommonProperties(moduleId, "Task",
                SystemModuleConstants.Basic.TaskModule.Properties.CreatedAtId,
                SystemModuleConstants.Basic.TaskModule.Properties.CreatedById,
                SystemModuleConstants.Basic.TaskModule.Properties.UpdatedAtId,
                SystemModuleConstants.Basic.TaskModule.Properties.UpdatedById,
                SystemModuleConstants.Basic.TaskModule.Properties.DeletedAtId,
                SystemModuleConstants.Basic.TaskModule.Properties.DeletedById,
                startingOrder: 4);

            properties.AddRange(commonProperties);

            return properties;
        }
    }

}
