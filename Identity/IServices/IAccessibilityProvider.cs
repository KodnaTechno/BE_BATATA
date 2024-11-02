using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AppIdentity.Domain;
using AppIdentity.Resources;
using AppIdentity.Services;

namespace AppIdentity.IServices;

public interface IAccessibilityProvider
{
    public IEnumerable<AppAccessibility> GetAccessibility(Expression<Func<AppAccessibility, bool>> predicate);
    AppAccessibility GetAccessibility(string moduleKey);
    
    AppAccessibility GetAccessibility(int accessibilityId);
    public IEnumerable<AppAccessibility> GetAccessibility();
    public IEnumerable<AppAccessibility> GetAccessibilitiesByGroup(List<int> groupIds);
    public IEnumerable<AppGroup> GetAccessibilitiesGroups(int accessibilitId);

    public AppAccessibility AddAccessibility(AppAccessibilityRes accessibility);
    public AppAccessibility UpdateAccessibility(int id, AppAccessibilityRes accessibility);
    public AppAccessibility DeleteAccessibility(int id);
    public AppAccessibility AddGroupToAccessibility(int accessibilityId, int groupId);
    public AppAccessibility RemoveGroupFromAccessibility(int accessibilityId, int groupId);
    public AppAccessibility AddGroupToAccessibility(string accessibilityKey, int groupId);
    public AppAccessibility RemoveGroupFromAccessibility(string accessibilityKey, int groupId);
    public AppAccessibility AddGroupsToAccessibility(int accessibilityId, List<int> groupIds);
    public AppAccessibility RemoveGroupsFromAccessibility(int accessibilityId, List<int> groupIds);
    public IEnumerable<AppGroup> GetAccessibilityGroups(int accessibilityId);
    public IEnumerable<AppGroup> GetAccessibilityGroups(string accessibilityKey);
    public IEnumerable<AppUser> GetAccessibilityUsers(int accessibilityId);
    public IEnumerable<AppUser> GetAccessibilityUsers(string accessibilityKey);
    
    public IEnumerable<AccessibilityPermissions> GetAccessibilityPermissionsForUser(string username);
    public IEnumerable<AccessibilityPermissions> GetAccessibilityPermissionsForUserByUserId(Guid userId);
    bool HasUserPermissionForModule(Guid userId, string moduleKey);
}