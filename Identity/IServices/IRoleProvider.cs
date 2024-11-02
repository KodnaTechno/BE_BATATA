using System.Linq.Expressions;
using AppIdentity.Domain;
using AppIdentity.Resources;

namespace AppIdentity.IServices;

public interface IRoleProvider
{
    #region Roles
    public AppResult<AppRole> Add(AddRoleRes addRoleRes);
    
    public List<AppRole> AddRange(List<AddRoleRes> addRoleResList);
    public AppRole Get(Guid roleId);
    public AppRole GetByName(string rolename);
    public IEnumerable<AppRole> Get(Func<AppRole, bool>? predicate = null);
    public IEnumerable<AppRole> GetAll(Func<AppRole, bool>? predicate = null);
    public IEnumerable<AppRole> GetRolesByModuleId(Guid moduleId);
    public IEnumerable<AppRole> GetRolesByModuleIds(List<Guid> moduleIds);
    public IEnumerable<AppRole> GetRolesBySourceId(int sourceId);
    public Task<List<AppRole>> GetRolesBySourceIdAsync(int sourceId);
    public IEnumerable<AppRole> GetRolesBySourceIds(List<int> sourceIds);
    public IEnumerable<AppRole> GetRolesByModuleIdAndSourceId(Guid moduleId, int sourceId);
    public AppResult<AppRole> UpdateRole(Guid roleId, UpdateRoleRes updateRoleRes);
    public AppResult<AppRole> UpdateRoleExtraInfo(Guid roleId, List<KeyValuePair<string, string>> extraInfo);
    
    public AppResult<AppRole> DeleteRole(Guid roleId);
    public AppResult<AppRole> DeleteRoles(List<Guid> roleIds);
    #endregion
    
    #region Permission
    public AppResult<AppPermission> AddPermission(PermissionRes permissionRes);
    public List<AppPermission> AddPermissions(List<PermissionRes> permissionsRes);
    public List<AppPermission> AddPermissions(List<AppPermission> permissions);

    public AppResult<AppPermission> UpdatePermission(int permissionId, PermissionRes permissionRes);
    public AppResult<AppPermission> DeletePermission(int permissionId);
    public AppResult<AppPermission> DeletePermissions(List<int> permissions);
    public IEnumerable<AppPermission> GetPermissions(Guid moduleId);
    public IEnumerable<AppPermission> GetPermissionsbyCommandName(string commandName);
    public AppPermission GetPermission(int permissionId);
    public List<AppPermission> GetPermissions(Func<AppPermission, bool>? predicate = null);
    #endregion

    #region RolePermission
    public AppResult<AppRolePermission> AddRolePermission(Guid roleId, int permissionId);
    public bool IsRolePermissionExist(Guid roleId, int permissionId);
    public AppResult<AppRolePermission> DeleteRolePermission(Guid roleId, int permissionId);
    public AppResult<AppRolePermission> DeleteRolePermission(string rolePermissionId);
    public IEnumerable<AppRolePermission> GetRolePermissions(Guid roleId);
    
    public IEnumerable<AppRolePermission> GetRolePermissions(Func<AppRolePermission, bool>? predicate = null);
    public IEnumerable<AppRolePermission> GetRolePermissionsByModuleId(Guid roleId, Guid moduleId);
    public IEnumerable<AppRolePermission> GetRolePermissionsByModuleIds(List<Guid> moduleIds);
    #endregion
    
    #region GroupPermission
    public AppResult<AppGroupPermission> AddGroupPermission(int groupId, int permissionId);
    public AppResult<AppGroupPermission> DeleteGroupPermission(int groupId, int permissionId);
    public AppResult<AppGroupPermission> DeleteGroupPermission(int groupPermissionId);
    public IEnumerable<AppGroup> GetGroupsOnPermissions(int permissionId);
    public IEnumerable<AppGroupPermission> GetGroupsPermissions(int groupId);
    
    public IEnumerable<AppGroupPermission> GetGroupPermissions(Func<AppGroupPermission, bool>? predicate = null);
    
    #endregion

    public bool HasPermissionForRole(Guid roleId, string command, Guid moduleId);
    public bool HasPermissionForGroup(int groupId, string command, Guid moduleId);
    public bool HasPermissionForRoles(List<Guid> roleIds, string command, Guid moduleId);
    public bool HasPermissionForGroups(List<int> groupIds, string command, Guid moduleId);
    public IEnumerable<Guid> GetRolesByUserId(Guid userId);
    public IEnumerable<AppRole> GetAppRoles(Expression<Func<AppRole, bool>>? exp=null);
    public IEnumerable<Guid> GetRolesByUserId(Guid userId, Guid moduleId);
    public AppResult<AppUserRole> AddUserRole(Guid userId, Guid roleId);
    public AppResult<AppUserRole> RemoveUserRole(Guid userId, Guid roleId);
    public AppResult<AppUserRole> AddUserRole(AddAppUserRoleRes roleUserRes);
    public IEnumerable<AppRole> GetUserRolesByModuleId(Guid userId, Guid moduleId);
    public bool HasRoleForModule(Guid userId, Guid moduleId);

    public bool RemoveUserRolesByModuleId(Guid moduleId);
    IEnumerable<AppRole> GetRolesByWorkspaceModuleIds(List<Guid> moduleIds);
}