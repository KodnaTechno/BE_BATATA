using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using AppIdentity.Database;
using AppIdentity.Domain;
using AppIdentity.IServices;
using AppIdentity.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity.Services;

public class RoleProvider : IRoleProvider
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly AppIdentityDbContext _dbContext;

    public RoleProvider(RoleManager<AppRole> roleManager, AppIdentityDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    #region Roles
    public  AppResult<AppRole> Add(AddRoleRes addRoleRes)
    {
        var appRole = new AppRole
        {
            Name = addRoleRes.Name,
            NormalizedName = addRoleRes.NormalizedName, // addRoleRes.Name.Normalize(),
            ModuleId = addRoleRes.ModuleId,
            SourceId = addRoleRes.SourceId,
            ExtraInfo = addRoleRes.ExtraInfo == null ? new List<KeyValuePair<string, string>>() : addRoleRes.ExtraInfo,
            DisplayName = addRoleRes.DisplayName,
        };
        var result =  _roleManager.CreateAsync(appRole).Result;
        if (result.Succeeded is false) return new AppResult<AppRole> {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppRole> {Succeeded = true, Message = "", Data = appRole}; 
    }
    
    public  List<AppRole> AddRange(List<AddRoleRes> addRoleResList)
    {
        var appRoleList = addRoleResList.Select(addRoleRes => new AppRole
        {
            Name = addRoleRes.Name,
            NormalizedName = addRoleRes.NormalizedName,
            ModuleId = addRoleRes.ModuleId,
            SourceId = addRoleRes.SourceId,
            ExtraInfo = addRoleRes.ExtraInfo ?? new(),
            DisplayName = addRoleRes.DisplayName,
        }).ToList();
        
        _dbContext.AppRoles.AddRange(appRoleList);
        _dbContext.SaveChanges();
        
        return appRoleList; 
    }


    public AppRole Get(string roleId)
    {
        return _roleManager.FindByIdAsync(roleId).Result;
    }
    
    public AppRole GetByName(string name)
    {
        return _roleManager.FindByNameAsync(name).Result;
    }

    
    public IEnumerable<AppRole> Get(Expression<AppRole> expression)
    {
        return _roleManager.Roles.Where(expression);
    }
    
    public IEnumerable<AppRole> GetAll(Func<AppRole, bool>? predicate = null)
    {
        var allRoles =  _roleManager.Roles.AsNoTracking();

        if (predicate is not null)
        {
            return allRoles.Where(predicate);
        }

        return allRoles;
    }


    public IEnumerable<AppRole> GetRolesByModuleId(int moduleId)
    {
        return _roleManager.Roles.Where(r => r.ModuleId == moduleId);
    }

    public IEnumerable<AppRole> GetRolesByModuleIds(List<int> moduleIds)
    {
        return _roleManager.Roles.Where(r => r.ModuleId != null && moduleIds.Contains(r.ModuleId.Value));
    }

    public IEnumerable<AppRole> GetRolesBySourceId(int sourceId)
    {
        return _roleManager.Roles.Where(r => r.SourceId == sourceId);
    }
    public Task<List<AppRole>> GetRolesBySourceIdAsync(int sourceId)
    {
        return _roleManager.Roles.Where(r => r.SourceId == sourceId)
            .ToListAsync();
    }

    public IEnumerable<AppRole> GetRolesBySourceIds(List<int> sourceIds)
    {
        return _roleManager.Roles.Where(r => r.SourceId != null && sourceIds.Contains(r.SourceId.Value));
    }

    public IEnumerable<AppRole> GetRolesByModuleIdAndSourceId(int moduleId, int sourceId)
    {
        return _roleManager.Roles.Where(r => r.SourceId == sourceId && r.ModuleId == moduleId);
    }

    public AppResult<AppRole> UpdateRole(string roleId, UpdateRoleRes updateRoleRes)
    {
        var role = _roleManager.FindByIdAsync(roleId).Result;
        if(roleId == null) return new AppResult<AppRole> {Succeeded = false, Message = "Role not found", Data = null};
        role.Name = updateRoleRes.Name;
        role.NormalizedName = updateRoleRes.Name.Normalize();
        role.ModuleId = updateRoleRes.ModuleId;
        role.SourceId = updateRoleRes.SourceId;
        role.DisplayName = updateRoleRes.DisplayName;
        var result = _roleManager.UpdateAsync(role).Result;
        if (result.Succeeded is false) return new AppResult<AppRole> {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppRole> {Succeeded = true, Message = "", Data = role}; 
    }

    public AppResult<AppRole> UpdateRoleExtraInfo(string roleId, List<KeyValuePair<string, string>> extraInfo)
    {
        var role = _roleManager.FindByIdAsync(roleId).Result;
        if(roleId == null) return new AppResult<AppRole> {Succeeded = false, Message = "Role not found", Data = null};
        role.ExtraInfo = extraInfo;
        var result = _roleManager.UpdateAsync(role).Result;
        if (result.Succeeded is false) return new AppResult<AppRole> {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppRole> {Succeeded = true, Message = "", Data = role};
    }


    public AppResult<AppRole> DeleteRole(string roleId)
    {
        var role = _roleManager.FindByIdAsync(roleId).Result;
        if(roleId is null) return new AppResult<AppRole> {Succeeded = false, Message = "Role not found", Data = null};

        var appRolePermissions = _dbContext.AppRolePermissions.Where(x => x.RoleId == roleId);
        _dbContext.RemoveRange(appRolePermissions);
        
        var userRoles = _dbContext.AppUserRoles.Where(x => x.RoleId == roleId);
        _dbContext.RemoveRange(userRoles);

        _dbContext.Remove(role);

        _dbContext.SaveChanges();
        
        return new AppResult<AppRole> {Succeeded = true, Message = "", Data = role};
    }

    public AppResult<AppRole> DeleteRoles(List<string> roleIds)
    {
        var roles = _dbContext.AppRoles.Where(x => roleIds.Contains(x.Id)).ToList();
        if(!roles.Any()) return new AppResult<AppRole> {Succeeded = false, Message = "Roles not found", Data = null};

        var appRolePermissions = _dbContext.AppRolePermissions.Where(x => roleIds.Contains(x.RoleId));
        _dbContext.RemoveRange(appRolePermissions);
        
        var userRoles = _dbContext.AppUserRoles.Where(x => roleIds.Contains(x.RoleId));
        _dbContext.RemoveRange(userRoles);

        _dbContext.RemoveRange(roles);

        _dbContext.SaveChanges();
        
        return new AppResult<AppRole> {Succeeded = true, Message = "", Data = null};
    }
    #endregion

    #region Permissions
    
    public AppResult<AppPermission> AddPermission(PermissionRes permissionRes)
    {
        var permission = new AppPermission
        {
            DisplayName = permissionRes.DisplayName,
            ModuleId = permissionRes.ModuleId,
            Command = permissionRes.Command,
            ModuleType = permissionRes.ModuleType == null ? "" : permissionRes.ModuleType
        };
        var result = _dbContext.AppPermissions.Add(permission).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppPermission>(){Succeeded = true, Message = "", Data = result};
    }

    public List<AppPermission> AddPermissions(List<PermissionRes> permissionsRes)
    {
        var permissions = permissionsRes.Select(permissionRes => new AppPermission()
        {
            DisplayName = permissionRes.DisplayName,
            ModuleId = permissionRes.ModuleId,
            Command = permissionRes.Command,
            ModuleType = permissionRes.ModuleType == null ? "" : permissionRes.ModuleType
        });
        
        _dbContext.AddRange(permissions);
        //_dbContext.SaveChanges();
        
        return permissions.ToList();
    }
    
    public List<AppPermission> AddPermissions(List<AppPermission> permissions)
    {
        _dbContext.AppPermissions.AddRange(permissions);
        _dbContext.SaveChanges();
        
        return permissions.ToList();
    }


    public AppResult<AppPermission> UpdatePermission(int permissionId, PermissionRes permissionRes)
    {
        var permission = _dbContext.AppPermissions.Find(permissionId);
        if (permission == null) return new AppResult<AppPermission>(){Succeeded = false, Message = "Permission not found", Data = null};
        permission.DisplayName = permissionRes.DisplayName;
        permission.ModuleId = permissionRes.ModuleId;
        permission.Command = permissionRes.Command;
        var result = _dbContext.AppPermissions.Update(permission).Entity;
        return new AppResult<AppPermission>(){Succeeded = true, Message = "", Data = result};
    }

    public AppResult<AppPermission> DeletePermission(int permissionId)
    {
        var permission = _dbContext.AppPermissions.Find(permissionId);
        if (permission == null) return new AppResult<AppPermission>(){Succeeded = false, Message = "Permission not found", Data = null};
        
        var rolePermission = _dbContext.AppRolePermissions.Where(x => x.AppPermissionId == permissionId);
        _dbContext.RemoveRange(rolePermission);
        
        var groupPermissions = _dbContext.AppGroupPermissions.Where(x => x.AppPermissionId == permissionId);
        _dbContext.RemoveRange(groupPermissions);
        
        var result = _dbContext.AppPermissions.Remove(permission).Entity;
        _dbContext.SaveChanges();
        
        return new AppResult<AppPermission>(){Succeeded = true, Message = "", Data = result};
    }
    
    public AppResult<AppPermission> DeletePermissions(List<int> permissionIds)
    {
        var permissions = _dbContext.AppPermissions.Where(x => permissionIds.Contains(x.Id));
        if (!permissions.Any()) return new AppResult<AppPermission>(){Succeeded = false, Message = "Permission not found", Data = null};
        
        var rolePermission = _dbContext.AppRolePermissions.Where(x => permissionIds.Contains(x.AppPermissionId));
        _dbContext.RemoveRange(rolePermission);
        
        var groupPermissions = _dbContext.AppGroupPermissions.Where(x => permissionIds.Contains(x.AppPermissionId));
        _dbContext.RemoveRange(groupPermissions);

        _dbContext.AppPermissions.RemoveRange(permissions);
        
        _dbContext.SaveChanges();
        
        return new AppResult<AppPermission>(){Succeeded = true, Message = "", Data = null};
    }

    public IEnumerable<AppPermission> GetPermissions(int moduleId)
    {
        return _dbContext.AppPermissions
            .Include(x=>x.RolePermissions)            
            .Where(p => p.ModuleId == moduleId)
            .ToList();
    }

    public IEnumerable<AppPermission> GetPermissionsbyCommandName(string commandName)
    {
        return _dbContext.AppPermissions.Where(p => p.Command == commandName);
    }
    
    public AppPermission GetPermission(int permissionId)
    {
        return _dbContext.AppPermissions.Find(permissionId);
    }

    public List<AppPermission> GetPermissions(Func<AppPermission, bool>? predicate = null)
    {
        var result = _dbContext.AppPermissions.AsQueryable();
        if (predicate is null) return result.ToList();
        return result.Where(predicate).ToList();
    }
    
    #endregion

    #region Role Permissions

    public AppResult<AppRolePermission> AddRolePermission(string roleId, int permissionId)
    {
        var rolePermission = new AppRolePermission
        {
            RoleId = roleId,
            AppPermissionId = permissionId
        };
        var result = _dbContext.AppRolePermissions.Add(rolePermission).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppRolePermission>(){Succeeded = true, Message = "", Data = result};
    }

    public bool IsRolePermissionExist(string roleId, int permissionId)
    {
        var permission = _dbContext.AppRolePermissions.
            FirstOrDefault(rp => rp.AppPermissionId == permissionId && rp.RoleId == roleId);
        return permission != null ? true : false;            
    }
    public AppResult<AppRolePermission> DeleteRolePermission(string roleId, int permissionId)
    {
        var permission = _dbContext.AppRolePermissions.FirstOrDefault(rp => rp.AppPermissionId == permissionId 
                                                                            && rp.RoleId == roleId);
        if (permission == null) return new AppResult<AppRolePermission>(){Succeeded = false, Message = "Role Permission not found", Data = null};
        var result = _dbContext.AppRolePermissions.Remove(permission).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppRolePermission> {Succeeded = true, Message = null, Data = permission};
    }
    
    public AppResult<AppRolePermission> DeleteRolePermission(string rolePermissionId)
    {
        var permission = _dbContext.AppRolePermissions.Find(rolePermissionId);
        if (permission == null) return new AppResult<AppRolePermission>(){Succeeded = false, Message = "Role Permission not found", Data = null};
        var result = _dbContext.AppRolePermissions.Remove(permission).Entity;
        return new AppResult<AppRolePermission> {Succeeded = true, Message = null, Data = permission};
    }


    public IEnumerable<AppRolePermission> GetRolePermissions(string roleId)
    { 
        var rolePermissions = _dbContext
            .AppRolePermissions
            .AsNoTracking()
            .Include(x => x.AppPermission)
            .Where(rp => rp.RoleId == roleId);
        return rolePermissions;
    }

    public IEnumerable<AppRolePermission> GetRolePermissions(Func<AppRolePermission, bool>? predicate = null)
    {
        return _dbContext.AppRolePermissions.Where(predicate);
    }

    public IEnumerable<AppRolePermission> GetRolePermissionsByModuleId(string roleId, int moduleId)
    {
        var rolePermissions = _dbContext
            .AppRolePermissions
            .AsNoTracking()
            .Include(x => x.AppPermission)
            .Where(rp => rp.RoleId == roleId && rp.AppPermission.ModuleId == moduleId);
        return rolePermissions;
    }
    
    public IEnumerable<AppRolePermission> GetRolePermissionsByModuleIds(List<int> moduleIds)
    {
        var rolePermissions = _dbContext
            .AppRolePermissions.AsNoTracking().AsSplitQuery()
            .Include(x => x.AppPermission)
            .Where(rp =>  moduleIds.Contains(rp.AppPermission.ModuleId.Value));
        return rolePermissions;
    }

    
    #endregion

    #region Group Permissions
    
    public AppResult<AppGroupPermission> AddGroupPermission(int groupId, int permissionId)
    {
        var groupPermission = new AppGroupPermission()
        {
            GroupId = groupId,
            AppPermissionId = permissionId
        };
        var result = _dbContext.AppGroupPermissions.Add(groupPermission).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppGroupPermission>(){Succeeded = true, Message = "", Data = result};
    }

    public AppResult<AppGroupPermission> DeleteGroupPermission(int groupId, int permissionId)
    {
        var permission = _dbContext.AppGroupPermissions.FirstOrDefault(a => a.AppPermissionId == permissionId &&
                                                                            a.GroupId == groupId);
        if (permission == null) return new AppResult<AppGroupPermission>(){Succeeded = false, Message = "Group Permission not found", Data = null};
        var result = _dbContext.AppGroupPermissions.Remove(permission).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppGroupPermission> {Succeeded = true, Message = null, Data = permission};
    }
    
    public AppResult<AppGroupPermission> DeleteGroupPermission(int groupPermissionId)
    {
        var permission = _dbContext.AppGroupPermissions.Find(groupPermissionId);
        if (permission == null) return new AppResult<AppGroupPermission>(){Succeeded = false, Message = "Group Permission not found", Data = null};
        var result = _dbContext.AppGroupPermissions.Remove(permission).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppGroupPermission> {Succeeded = true, Message = null, Data = permission};
    }
    
    public IEnumerable<AppGroup> GetGroupsOnPermissions(int permissionId)
    {
        var groups = _dbContext
            .AppGroupPermissions
            .AsNoTracking()
            .Include(x => x.Group)
            .Where(gp => gp.AppPermissionId == permissionId)
            .Select(gp => gp.Group);
        return groups;
    }
    
    public IEnumerable<AppGroupPermission> GetGroupsPermissions(int groupId)
    {
        var appGroupPermissions = _dbContext
            .AppGroupPermissions
            .AsNoTracking()
            .Where(gp => gp.GroupId == groupId)
            .ToList();
        return appGroupPermissions;
    }

    public IEnumerable<AppGroupPermission> GetGroupPermissions(Func<AppGroupPermission, bool>? predicate = null)
    {
        if (predicate is null) return new List<AppGroupPermission>();

        var appGroupPermissions = _dbContext
            .AppGroupPermissions
            .Where(predicate);
        
        return appGroupPermissions;
    }
    

    #endregion

    public bool HasPermissionForRole(string roleId, string command, int moduleId)
    {
        return _dbContext.AppRolePermissions
            .Any(rp => rp.RoleId == roleId
                       && rp.AppPermission.Command == command
                       && rp.AppPermission.ModuleId == moduleId);
    }
    public bool HasPermissionForGroup(int groupId, string command, int moduleId)
    {
        return _dbContext.AppGroupPermissions
            .Any(rp => rp.GroupId == groupId
                       && rp.AppPermission.Command == command
                       && rp.AppPermission.ModuleId == moduleId);
    }

    public bool HasPermissionForRoles(List<string> roleIds, string command, int moduleId)
    {
        return _dbContext.AppRolePermissions
            .Any(rp => roleIds.Contains(rp.RoleId)
                       && rp.AppPermission.Command == command
                       && rp.AppPermission.ModuleId == moduleId);
    }

    public bool HasPermissionForGroups(List<int> groupIds, string command, int moduleId)
    {
        return _dbContext.AppGroupPermissions
            .Any(rp => groupIds.Contains(rp.GroupId)
                       && rp.AppPermission.Command == command
                       && rp.AppPermission.ModuleId == moduleId);
    }


    public IEnumerable<string> GetRolesByUserId(string userId, int moduleId)
    {
        var roles = _dbContext.Roles.Where(r => r.ModuleId == moduleId).Select(r=>r.Id).ToList();
        return _dbContext.AppUserRoles            
            .Where(x => x.UserId == userId && roles.Contains(x.RoleId))
            .Select(x => x.RoleId)
            .ToList();
    }
    public IEnumerable<string> GetRolesByUserId(string userId)
    {
        return _dbContext.AppUserRoles.Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToList();
    }
    
    public AppResult<AppUserRole> AddUserRole(string userId, string roleId)
    {
        var userRole = new AppUserRole()
        {
            UserId = userId,
            RoleId = roleId
        };
        var result = _dbContext.AppUserRoles.Add(userRole).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppUserRole>() {Succeeded = true, Message = "", Data = result};
    }

    public AppResult<AppUserRole> RemoveUserRole(string userId, string roleId)
    {
        var record = _dbContext.AppUserRoles
            .FirstOrDefault(x => x.UserId == userId && x.RoleId == roleId);
        if(record == null) return new AppResult<AppUserRole>(){Succeeded = true, Message = "User role not found", Data = null};
        var result = _dbContext.AppUserRoles.Remove(record).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppUserRole>() {Succeeded = true, Message = "", Data = result};
    }

    public AppResult<AppUserRole> AddUserRole(AddAppUserRoleRes roleUserRes)
    {
        var userRole = new AppUserRole()
        {
            UserId = roleUserRes.UserId,
            RoleId = roleUserRes.RoleId,
            ModuleId = roleUserRes.ModuleId
        };
        
        var result = _dbContext.AppUserRoles.Add(userRole).Entity;
        _dbContext.SaveChanges();
        return new AppResult<AppUserRole>() { Succeeded = true, Message = "", Data = result };
    }
    public IEnumerable<AppRole> GetUserRolesByModuleId(string userId, int moduleId)
    {
        var moduleRoles = _dbContext.AppUserRoles.Where(r => r.ModuleId == moduleId
        && r.UserId == userId).Select(r=>r.RoleId).ToList();
        return _roleManager.Roles.Where(r => moduleRoles.Contains(r.Id));
    }

    public bool HasRoleForModule(string userId, int moduleId)
    {
        return _dbContext.AppUserRoles.Any(r => r.ModuleId == moduleId
                                                && r.UserId == userId);
    }
    public bool RemoveUserRolesByModuleId(int moduleId)
    {
        var appUserRoles = _dbContext.AppUserRoles.Where(r => r.ModuleId == moduleId);
        _dbContext.RemoveRange(appUserRoles);
        return true;
    }

    public IEnumerable<AppRole> GetAppRoles(Expression<Func<AppRole, bool>> exp)
    {
        if(exp is null)
            return _dbContext.AppRoles.ToList();
       return _dbContext.AppRoles.Where(exp).ToList();
    }
}