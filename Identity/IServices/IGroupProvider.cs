using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AppIdentity.Domain;

namespace AppIdentity.IServices;

public interface IGroupProvider
{
    public IEnumerable<AppGroup> GetGroups(Func<AppGroup, bool>? predicate = null);
    public AppGroup GetGroup(int groupId);
    public IEnumerable<AppGroup> GetGroupsWithDetails();
    public IEnumerable<AppGroup> GetGroup(Func<AppGroup, bool>? predicate = null);
    public IEnumerable<AppUser> GetGroupUsers(int groupId);
    public IEnumerable<AppUser> GetGroupUsers(string groupName);
    
    public IEnumerable<AppUser> GetGroupsUsers(List<int> groupsIds);
    public bool GroupExist(string groupName);


    public AppGroup AddGroup(string groupName);
    public AppGroup UpdateGroupName(int groupId, string groupName);
    public AppGroup DeleteGroup(int groupId);
    public AppGroup ActivateGroup(int groupId, bool activate);
    
    public AppGroup AddUser(int groupId, string username);
    public AppGroup AddUsers(int groupId, List<string> usernames);
    
    public AppGroup DeleteUser(int groupId, string username);
    public AppGroup DeleteUsers(int groupId, List<string> usernames);
    public List<AppGroup> GetGroupsOfUser(string userId);
    bool IsUserInAdminGroup(string userId);
}