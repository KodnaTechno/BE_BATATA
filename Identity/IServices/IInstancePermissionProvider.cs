using AppIdentity.Domain;

namespace AppIdentity.IServices;

public interface IInstancePermissionProvider
{
    public IEnumerable<AppGroup> GetInstancePermissionGroups(int instanceId);
    
    public IEnumerable<AppUser> GetInstancePermissionUsers(int instanceId);

    public InstanceGroupPermission AddGroupToInstancePermission(int instanceId, int groupId);
    
    public InstanceGroupPermission RemoveGroupFromInstancePermission(int instanceId, int groupId);
    
    public bool GroupExistInInstancePermissions(int instanceId, int groupId);

    public IEnumerable<InstanceGroupPermission> GetInstancesByGroupId(int groupId);
    public IEnumerable<InstanceGroupPermission> GetInstancesForCurrentUser();

}