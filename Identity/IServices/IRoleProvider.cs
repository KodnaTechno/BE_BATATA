using System.Linq.Expressions;
using AppIdentity.Domain;
using AppIdentity.Resources;

namespace AppIdentity.IServices;

public interface IRoleProvider
{
    #region Roles
    public AppResult<AppRole> Add(AddRoleRes addRoleRes);
    
    public List<AppRole> AddRange(List<AddRoleRes> addRoleResList);
    public AppRole Get(string roleId);
    public AppRole GetByName(string roleId);
    public IEnumerable<AppRole> Get(Expression<AppRole> expression);
    public IEnumerable<AppRole> GetAll(Func<AppRole, bool>? predicate = null);
    public IEnumerable<AppRole> GetRolesByModuleId(int moduleId);
    public IEnumerable<AppRole> GetRolesByModuleIds(List<int> moduleIds);
    public IEnumerable<AppRole> GetRolesBySourceId(int sourceId);
    public Task<List<AppRole>> GetRolesBySourceIdAsync(int sourceId);
    public IEnumerable<AppRole> GetRolesBySourceIds(List<int> sourceIds);
    public IEnumerable<AppRole> GetRolesByModuleIdAndSourceId(int moduleId, int sourceId);
    public AppResult<AppRole> UpdateRole(string roleId, UpdateRoleRes updateRoleRes);
    public AppResult<AppRole> UpdateRoleExtraInfo(string roleId, List<KeyValuePair<string, string>> extraInfo);
    
    public AppResult<AppRole> DeleteRole(string roleId);
    public AppResult<AppRole> DeleteRoles(List<string> roleIds);
    #endregion
    
    #region Permission
    public AppResult<AppPermission> AddPermission(PermissionRes permissionRes);
    public List<AppPermission> AddPermissions(List<PermissionRes> permissionsRes);
    public List<AppPermission> AddPermissions(List<AppPermission> permissions);

    public AppResult<AppPermission> UpdatePermission(int permissionId, PermissionRes permissionRes);
    public AppResult<AppPermission> DeletePermission(int permissionId);
    public AppResult<AppPermission> DeletePermissions(List<int> permissions);
    public IEnumerable<AppPermission> GetPermissions(int moduleId);
    public IEnumerable<AppPermission> GetPermissionsbyCommandName(string commandName);
    public AppPermission GetPermission(int permissionId);
    public List<AppPermission> GetPermissions(Func<AppPermission, bool>? predicate = null);
    #endregion

    #region RolePermission
    public AppResult<AppRolePermission> AddRolePermission(string roleId, int permissionId);
    public bool IsRolePermissionExist(string roleId, int permissionId);
    public AppResult<AppRolePermission> DeleteRolePermission(string roleId, int permissionId);
    public AppResult<AppRolePermission> DeleteRolePermission(string rolePermissionId);
    public IEnumerable<AppRolePermission> GetRolePermissions(string roleId);
    
    public IEnumerable<AppRolePermission> GetRolePermissions(Func<AppRolePermission, bool>? predicate = null);
    public IEnumerable<AppRolePermission> GetRolePermissionsByModuleId(string roleId, int moduleId);
    public IEnumerable<AppRolePermission> GetRolePermissionsByModuleIds(List<int> moduleIds);
    #endregion
    
    #region GroupPermission
    public AppResult<AppGroupPermission> AddGroupPermission(int groupId, int permissionId);
    public AppResult<AppGroupPermission> DeleteGroupPermission(int groupId, int permissionId);
    public AppResult<AppGroupPermission> DeleteGroupPermission(int groupPermissionId);
    public IEnumerable<AppGroup> GetGroupsOnPermissions(int permissionId);
    public IEnumerable<AppGroupPermission> GetGroupsPermissions(int groupId);
    
    public IEnumerable<AppGroupPermission> GetGroupPermissions(Func<AppGroupPermission, bool>? predicate = null);
    
    #endregion

    public bool HasPermissionForRole(string roleId, string command, int moduleId);
    public bool HasPermissionForGroup(int groupId, string command, int moduleId);
    public bool HasPermissionForRoles(List<string> roleIds, string command, int moduleId);
    public bool HasPermissionForGroups(List<int> groupIds, string command, int moduleId);
    public IEnumerable<string> GetRolesByUserId(string userId);
    public IEnumerable<AppRole> GetAppRoles(Expression<Func<AppRole, bool>>? exp=null);
    public IEnumerable<string> GetRolesByUserId(string userId, int moduleId);
    public AppResult<AppUserRole> AddUserRole(string userId, string roleId);
    public AppResult<AppUserRole> RemoveUserRole(string userId, string roleId);
    public AppResult<AppUserRole> AddUserRole(AddAppUserRoleRes roleUserRes);
    public IEnumerable<AppRole> GetUserRolesByModuleId(string userId, int moduleId);
    public bool HasRoleForModule(string userId, int moduleId);

    public bool RemoveUserRolesByModuleId(int moduleId);

}