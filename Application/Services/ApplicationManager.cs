using AppCommon.EnumShared;
using AppIdentity.Domain;
using AppIdentity.Domain.Enums;
using AppIdentity.IServices;
using AppIdentity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ApplicationManager
    {
        private readonly IRoleProvider _roleProvider;

        public ApplicationManager(IRoleProvider roleProvider)
        {
            _roleProvider = roleProvider;
        }
        public async Task LinkRolesBasedOnTeamModules(List<Module.Domain.Schema.WorkspaceModule> workSpaceModules)
        {
            //Make Sure All Current Modules Are Team Modules
            workSpaceModules = workSpaceModules.Where(x => x.Module.Type == ModuleTypeEnum.Team).ToList();

            //Create New Roles 
            var roles = workSpaceModules.Select(m => new AppIdentity.Resources.AddRoleRes
            {
                DisplayName = new Translatable { Ar = $"{m.Workspace.Title} {m.Module.Name}" , En = $"{m.Workspace.Title} {m.Module.Name}" },
                Name = $"{m.Workspace.Title}_{m.Module.Name}",
                NormalizedName = m.Module.Name,
                ModuleId = m.Id,
                ModuleType = RoleModulesEnum.WorkspaceModule
            }).ToList();
            _roleProvider.AddRange(roles);
        }

        public async Task UnLinkRolesBasedOnTeamModules(List<Module.Domain.Schema.WorkspaceModule> workSpaceModules)
        {
            //Make Sure All Current Modules Are Team Modules
            workSpaceModules = workSpaceModules.Where(x => x.Module.Type == ModuleTypeEnum.Team).ToList();

            //Fetch Linked Roles
            var Ids = workSpaceModules.Select(x => x.Id).ToList();
            var roles = _roleProvider.GetRolesByWorkspaceModuleIds(Ids);
            var roleIds = roles.Select(x => x.Id).ToList();
            _roleProvider.DeleteRoles(roleIds);
        }
    }
}
