using AppCommon.DTOs;
using Module.Domain.Schema;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;

namespace Module.Service.DefaultSetupService
{
    public class DefaultWorkspaceSetupService : IDefaultWorkspaceSetupService
    {
        private readonly ModuleDbContext _context;

        public DefaultWorkspaceSetupService(ModuleDbContext context)
        {
            _context = context;
        }

        public void AddDefaultActions(Guid workspaceId, Guid userId)
        {
            var workspace = _context.Workspaces.Find(workspaceId);
            if (workspace == null) return;

            var now = DateTime.UtcNow;

            var defaultActions = new List<AppAction>
            {
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "اضافة", En = "Create" },
                    Description = new TranslatableValue {
                        Ar = "إنشاء مساحة العمل",
                        En = "Create workspace"
                    },
                    Type = ActionType.Create,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "تعديل", En = "Update" },
                    Description = new TranslatableValue {
                        Ar = "تحديث مساحة العمل",
                        En = "Update workspace"
                    },
                    Type = ActionType.Update,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "حذف", En = "Delete" },
                    Description = new TranslatableValue {
                        Ar = "حذف مساحة العمل",
                        En = "Delete workspace"
                    },
                    Type = ActionType.Delete,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "معاينة", En = "Read" },
                    Description = new TranslatableValue {
                        Ar = "قراءة مساحة العمل",
                        En = "Read workspace"
                    },
                    Type = ActionType.Read,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                }
            };

            _context.AppActions.AddRange(defaultActions);
            _context.SaveChanges();
        }
        public void AddDefaultProperties(Guid workspaceId, Guid userId)
        {
            var workspace = _context.Workspaces.Find(workspaceId);
            if (workspace == null) return;

            var now = DateTime.UtcNow;

            var defaultProperties = new List<Property>
            {
                new() {
                    Title = SharedPropertyConfigurations.Common.CreatedAtTitle,
                    Key = workspace.Key + "_" + SharedPropertyConfigurations.Common.CreatedAt.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.CreatedAt.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "تاريخ إنشاء مساحة العمل",
                        En = "Date the workspace was created"
                    },
                    ViewType = SharedPropertyConfigurations.Common.CreatedAt.ViewType,
                    DataType = SharedPropertyConfigurations.Common.CreatedAt.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.CreatedAt.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.CreatedAt.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.CreatedByTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.CreatedBy.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.CreatedBy.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "المستخدم الذي أنشأ مساحة العمل",
                        En = "User who created the workspace"
                    },
                    ViewType = SharedPropertyConfigurations.Common.CreatedBy.ViewType,
                    DataType = SharedPropertyConfigurations.Common.CreatedBy.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.CreatedBy.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.CreatedBy.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.UpdatedAtTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.UpdatedAt.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.UpdatedAt.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "تاريخ آخر تحديث",
                        En = "Last update date"
                    },
                    ViewType = SharedPropertyConfigurations.Common.UpdatedAt.ViewType,
                    DataType = SharedPropertyConfigurations.Common.UpdatedAt.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.UpdatedAt.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.UpdatedAt.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.UpdatedByTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.UpdatedBy.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.UpdatedBy.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "آخر مستخدم قام بتحديث مساحة العمل",
                        En = "Last user who updated the workspace"
                    },
                    ViewType = SharedPropertyConfigurations.Common.UpdatedBy.ViewType,
                    DataType = SharedPropertyConfigurations.Common.UpdatedBy.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.UpdatedBy.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.UpdatedBy.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.DeletedAtTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.DeletedAt.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.DeletedAt.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "تاريخ حذف مساحة العمل",
                        En = "Date the workspace was deleted"
                    },
                    ViewType = SharedPropertyConfigurations.Common.DeletedAt.ViewType,
                    DataType = SharedPropertyConfigurations.Common.DeletedAt.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.DeletedAt.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.DeletedAt.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.DeletedByTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.DeletedBy.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.DeletedBy.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "المستخدم الذي قام بحذف مساحة العمل",
                        En = "User who deleted the workspace"
                    },
                    ViewType = SharedPropertyConfigurations.Common.DeletedBy.ViewType,
                    DataType = SharedPropertyConfigurations.Common.DeletedBy.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.DeletedBy.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.DeletedBy.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                }
            };

            _context.Properties.AddRange(defaultProperties);
            _context.SaveChanges();
        }
    }
}
