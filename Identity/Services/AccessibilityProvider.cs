using System.Linq.Expressions;
using AppIdentity.Database;
using AppIdentity.Domain;
using AppIdentity.IServices;
using AppIdentity.Resources;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity.Services;

public class AccessibilityProvider : IAccessibilityProvider
{
    private readonly AppIdentityDbContext _dbContext;

    public AccessibilityProvider(AppIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<AppAccessibility> GetAccessibility(Expression<Func<AppAccessibility, bool>> predicate)
    {
        return _dbContext.AppAccessibilities.Where(predicate);
    }

    public IEnumerable<AppAccessibility> GetAccessibilitiesByGroup(List<int> groupIds)
    {
        return _dbContext.AppAccessibilityGroups
            .Include(a=>a.AppAccessibility)
            .Where(a => groupIds.Contains( a.AppGroupId)).Select(a=>a.AppAccessibility).ToList();
    }


    public IEnumerable<AppGroup> GetAccessibilitiesGroups(int accessibilitId)
    {
        return _dbContext.AppAccessibilityGroups
            .Include(a => a.AppGroup)
            .Where(a => a.AppAccessibilityId == accessibilitId).Select(a => a.AppGroup).ToList();
    }

    public AppAccessibility GetAccessibility(string moduleKey)
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .FirstOrDefault(a => a.ModuleKey == moduleKey);
    }
    
    public AppAccessibility GetAccessibility(int accessibilityId)
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .FirstOrDefault(a => a.Id == accessibilityId);
    }


    public IEnumerable<AppAccessibility> GetAccessibility()
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup);
    }
    public AppAccessibility AddAccessibility(AppAccessibilityRes accessibilityRes)
    {
        var accessibility = new AppAccessibility
        {
            ModuleKey = accessibilityRes.ModuleKey,
            ModuleName = accessibilityRes.ModuleName,
            Category = accessibilityRes.Category,
            CategoryName = accessibilityRes.CategoryName,
        };
        _dbContext.AppAccessibilities.Add(accessibility);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility UpdateAccessibility(int id, AppAccessibilityRes accessibilityRes)
    {
        var accessibility = _dbContext.AppAccessibilities.Find(id);
        if (accessibility == null) throw new Exception("Accessibility Not found");
        accessibility.ModuleKey = accessibilityRes.ModuleKey;
        accessibility.ModuleName = accessibilityRes.ModuleName;
        accessibility.Category = accessibilityRes.Category;
        accessibility.CategoryName = accessibilityRes.CategoryName;
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility DeleteAccessibility(int id)
    {
        var accessibility = _dbContext.AppAccessibilities.Find(id);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        _dbContext.AppAccessibilities.Remove(accessibility);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility AddGroupToAccessibility(int accessibilityId, int groupId)
    {
        var accessibility = _dbContext.AppAccessibilities.Find(accessibilityId);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        var group = _dbContext.AppGroups.Find(groupId);
        if (group == null) throw new Exception("Group Not found"); ;
        var accessibilityGroup = new AppAccessibilityGroup
        {
            AppAccessibilityId = accessibilityId,
            AppGroupId = groupId
        };
        _dbContext.AppAccessibilityGroups.Add(accessibilityGroup);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility RemoveGroupFromAccessibility(int accessibilityId, int groupId)
    {
        var accessibility = _dbContext.AppAccessibilities.Find(accessibilityId);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        var group = _dbContext.AppGroups.Find(groupId);
        if (group == null) throw new Exception("Group Not found"); ;
        var accessibilityGroup = _dbContext.AppAccessibilityGroups.FirstOrDefault(x => x.AppAccessibilityId == accessibilityId && x.AppGroupId == groupId);
        if (accessibilityGroup == null) throw new Exception("Group Not found in Accessibility"); ;
        _dbContext.AppAccessibilityGroups.Remove(accessibilityGroup);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility AddGroupToAccessibility(string accessibilityKey, int groupId)
    {
        var accessibility = _dbContext.AppAccessibilities.FirstOrDefault(a => a.ModuleKey == accessibilityKey);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        var group = _dbContext.AppGroups.Find(groupId);
        if (group == null) throw new Exception("Group Not found"); ;
        var accessibilityGroup = new AppAccessibilityGroup
        {
            AppAccessibilityId = accessibility.Id,
            AppGroupId = groupId
        };
        _dbContext.AppAccessibilityGroups.Add(accessibilityGroup);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility RemoveGroupFromAccessibility(string accessibilityKey, int groupId)
    {
        var accessibility = _dbContext.AppAccessibilities.FirstOrDefault(a => a.ModuleKey == accessibilityKey);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        var group = _dbContext.AppGroups.Find(groupId);
        if (group == null) throw new Exception("Group Not found"); ;
        var accessibilityGroup = _dbContext.AppAccessibilityGroups.FirstOrDefault(x => x.AppAccessibilityId == accessibility.Id && x.AppGroupId == groupId);
        if (accessibilityGroup == null) throw new Exception("Group Not found in Accessibility"); ;
        _dbContext.AppAccessibilityGroups.Remove(accessibilityGroup);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility AddGroupsToAccessibility(int accessibilityId, List<int> groupIds)
    {
        var accessibility = _dbContext.AppAccessibilities.Find(accessibilityId);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        var groups = _dbContext.AppGroups.Where(g => groupIds.Contains(g.Id)).ToList();
        if (groups.Count != groupIds.Count) throw new Exception("Group Not found"); ;
        var accessibilityGroups = groups.Select(g => new AppAccessibilityGroup
        {
            AppAccessibilityId = accessibilityId,
            AppGroupId = g.Id
        }).ToList();
        _dbContext.AppAccessibilityGroups.AddRange(accessibilityGroups);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public AppAccessibility RemoveGroupsFromAccessibility(int accessibilityId, List<int> groupIds)
    {
        var accessibility = _dbContext.AppAccessibilities.Find(accessibilityId);
        if (accessibility == null) throw new Exception("Accessibility Not found"); ;
        var groups = _dbContext.AppGroups.Where(g => groupIds.Contains(g.Id)).ToList();
        if (groups.Count != groupIds.Count) throw new Exception("Group Not found"); ;
        var accessibilityGroups = _dbContext.AppAccessibilityGroups.Where(x => x.AppAccessibilityId == accessibilityId && groupIds.Contains(x.AppGroupId)).ToList();
        //if (accessibilityGroups.Count != groupIds.Count) throw new Exception("Group Not found in Accessibility"); ;
        _dbContext.AppAccessibilityGroups.RemoveRange(accessibilityGroups);
        _dbContext.SaveChanges();
        return accessibility;
    }

    public IEnumerable<AppGroup> GetAccessibilityGroups(int accessibilityId)
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == accessibilityId)
            ?.AccessibilityGroups.Select(a => a.AppGroup);
    }

    public IEnumerable<AppGroup> GetAccessibilityGroups(string accessibilityKey)
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefault(x => x.ModuleKey == accessibilityKey)
            ?.AccessibilityGroups.Select(a => a.AppGroup);
    }

    public IEnumerable<AppUser> GetAccessibilityUsers(int accessibilityId)
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .ThenInclude(a => a.GroupUsers)
            .ThenInclude(a => a.User)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == accessibilityId)
            ?.AccessibilityGroups.SelectMany(a => a.AppGroup.GroupUsers)
            .Select(a => a.User);
    }

    public IEnumerable<AppUser> GetAccessibilityUsers(string accessibilityKey)
    {
        return _dbContext.AppAccessibilities
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .ThenInclude(a => a.GroupUsers)
            .ThenInclude(a => a.User)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefault(x => x.ModuleKey == accessibilityKey)
            ?.AccessibilityGroups.SelectMany(a => a.AppGroup.GroupUsers)
            .Select(a => a.User);
    }

    public IEnumerable<AccessibilityPermissions> GetAccessibilityPermissionsForUser(string username)
    {
        return _dbContext.AppAccessibilities.AsSplitQuery().AsNoTracking()
            .Include(a => a.AccessibilityGroups)
            .ThenInclude(a => a.AppGroup)
            .ThenInclude(a => a.GroupUsers)
            .ThenInclude(a => a.User)
            .Select(x => new AccessibilityPermissions(x, x.AccessibilityGroups
                .Select(a => a.AppGroup)
                .SelectMany(a => a.GroupUsers)
                .Any(a => a.User.UserName == username)))
            .AsEnumerable();
    }
    public IEnumerable<AccessibilityPermissions> GetAccessibilityPermissionsForUserByUserId(string userId)
    {
        return _dbContext.AppAccessibilities.AsSplitQuery().AsNoTracking()
            .Select(x => new AccessibilityPermissions(x, x.AccessibilityGroups
                .Select(a => a.AppGroup)
                .SelectMany(a => a.GroupUsers)
                .Any(a => a.User.Id == userId)))
            .AsEnumerable();
    }

    public bool HasUserPermissionForModule(string userId, string moduleKey)
    {
        var hasPermission = _dbContext.AppAccessibilities
            .AsNoTracking()
            .Any(x => x.ModuleKey.ToLower() == moduleKey.ToLower() &&
                      x.AccessibilityGroups.Any(group =>
                          group.AppGroup.GroupUsers.Any(user => user.UserId == userId)));

        return hasPermission;
    }
}

public record AccessibilityPermissions(AppAccessibility Accessibility, bool HasPermission);
