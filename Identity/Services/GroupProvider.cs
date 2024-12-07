using AppIdentity.Database;
using AppIdentity.Domain;
using AppIdentity.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity.Services;

public class GroupProvider : IGroupProvider
{
    private readonly AppIdentityDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    public GroupProvider(AppIdentityDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }
    
    public IEnumerable<AppGroup> GetGroups(Func<AppGroup, bool>? predicate = null)
    {
        var groups = _dbContext.AppGroups.AsQueryable();
        
        if (predicate is not null)
        {
            return groups.Where(predicate).ToList();
        }

        return groups.ToList();
    }
    
    public IEnumerable<AppGroup> GetGroupsWithDetails()
    {
        var groups = _dbContext.AppGroups.AsSplitQuery().AsNoTracking()
            .Include(x => x.GroupUsers)
            .ThenInclude(x => x.User)
            .AsQueryable();

        return groups.ToList();
    }


    public AppGroup GetGroup(int groupId)
    {
        return _dbContext.AppGroups.FirstOrDefault(g => g.Id == groupId);
    }
    public bool GroupExist(string groupName)
    {
        return _dbContext.AppGroups.FirstOrDefault(g => g.Name == groupName) !=null;
    }
    public IEnumerable<AppGroup> GetGroup(Func<AppGroup, bool>? predicate = null)
    {
        return _dbContext.AppGroups.Where(predicate);
    }

    public IEnumerable<AppUser> GetGroupUsers(string groupName)
    {
        return _dbContext.AppGroupUsers
            .Include(gu => gu.User)
            .Include(gu => gu.Group)
            .Where(g => g.Group.Name == groupName)
            .Select(g => g.User);
    }
    public bool IsUserInAdminGroup(Guid userId)
    {
        var adminGroupId = _dbContext.AppGroups.FirstOrDefault().Id;
        return _dbContext.AppGroupUsers
            .Any(g => g.UserId == userId && g.GroupId == adminGroupId);
    }
    public IEnumerable<AppUser> GetGroupUsers(int groupId)
    {
        return _dbContext.AppGroupUsers
            .Include(gu => gu.User)
            .Where(g => g.GroupId == groupId)
            .Select(g => g.User);
    }

    public IEnumerable<AppUser> GetGroupsUsers(List<int> groupsIds)
    {
        return _dbContext.AppGroupUsers
            .Include(gu => gu.User)
            .Where(g => groupsIds.Contains(g.GroupId))
            .Select(g => g.User);
    }

    public AppGroup AddGroup(string groupName)
    {
        var AppGroup = new AppGroup { Name = groupName };
        _dbContext.AppGroups.Add(AppGroup);
        _dbContext.SaveChanges();
        return AppGroup;
    }

    public List<AppGroup> GetGroupsOfUser(Guid userId)
    {
        return _dbContext.AppGroupUsers
            .Include(gu => gu.Group)
            .Where(g => g.UserId == userId)
            .Select(g => g.Group)
            .ToList();
    }

    public AppGroup UpdateGroupName(int groupId, string groupName)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if(appGroup == null) throw new Exception("Group not found");
        appGroup.Name = groupName;
        _dbContext.SaveChanges();
        return appGroup;
    }

    public AppGroup DeleteGroup(int groupId)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if(appGroup == null) throw new Exception("Group not found");
        // Delete group users
        _dbContext.AppGroupUsers.RemoveRange(_dbContext.AppGroupUsers.Where(g => g.GroupId == groupId));
        _dbContext.AppGroups.Remove(appGroup);
        _dbContext.SaveChanges();
        return appGroup;
    }

    public AppGroup ActivateGroup(int groupId, bool activate)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if(appGroup == null) throw new Exception("Group not found");
        appGroup.IsActivated = activate;
        _dbContext.SaveChanges();
        return appGroup;
    }

    public AppGroup AddUser(int groupId, string username)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if (appGroup == null) throw new Exception("Group not found");
        var appUser = _userManager.FindByNameAsync(username).Result;
        if(appUser == null) throw new Exception("User not found");
        _dbContext.AppGroupUsers.Add(new AppGroupUser
        {
            UserId = appUser.Id,
            GroupId = groupId,
        });
        _dbContext.SaveChanges();
        return appGroup;
    }

    public AppGroup AddUsers(int groupId, List<string> usernames)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if(appGroup == null) throw new Exception("Group not found");
        var appUsers = _userManager.Users.Where(u => usernames.Contains(u.UserName)).ToList();
        if(appUsers.Count != usernames.Count) throw new Exception("User not found");
        var groupUsers = appUsers.Select(x => new AppGroupUser
        {
            UserId = x.Id,
            GroupId = groupId,
        });
        _dbContext.AppGroupUsers.AddRange(groupUsers);
        _dbContext.SaveChanges();
        return appGroup;
    }

    public AppGroup DeleteUser(int groupId, string username)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if(appGroup == null) throw new Exception("Group not found");
        var appUser = _userManager.FindByNameAsync(username).Result;
        if(appUser == null) throw new Exception("User not found");
        var groupUser = _dbContext.AppGroupUsers.FirstOrDefault(x => x.UserId == appUser.Id && x.GroupId == groupId);
        if(groupUser == null) throw new Exception("User not found in group");
        _dbContext.AppGroupUsers.Remove(groupUser);
        _dbContext.SaveChanges();
        return appGroup;
    }

    public AppGroup DeleteUsers(int groupId, List<string> usernames)
    {
        var appGroup = _dbContext.AppGroups.Find(groupId);
        if(appGroup == null) throw new Exception("Group not found");
        var appUsers = _userManager.Users.Where(u => usernames.Contains(u.UserName)).ToList();
        if(appUsers.Count != usernames.Count) throw new Exception("User not found");
        var groupUsers = _dbContext.AppGroupUsers.Where(x => appUsers.Select(u => u.Id).Contains(x.UserId) && x.GroupId == groupId);
        _dbContext.AppGroupUsers.RemoveRange(groupUsers);
        _dbContext.SaveChanges();
        return appGroup;
    }
}