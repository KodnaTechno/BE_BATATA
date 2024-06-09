using System.Linq.Expressions;
using AppIdentity.Domain;
using AppIdentity.Resources;

namespace AppIdentity.IServices;

//TODO: Implement Return User Image Profile


public interface IUserProvider
{
    public Task<AppResult<AppUser>> Add(AddUserRes user, bool isExternal = false);
    public Task<AppResult<AppUser>> Update(UpdateUserReq userToUpdate);
    public Task<AppResult<AppUser>> Get(string userName);

    public Task<AppResult<AppUser>> GetById(string userId);
    public Task<AppResult<List<AppUser>>> GetByIds(List<string> userIds);

    public Task<AppResult<AppUser>> GetByEmail(string email);
    public IEnumerable<AppUser> Users();
    public Task<List<AppUser>> UsersAsync();
    public IEnumerable<AppUser> Users(List<string> usernames);
    public IEnumerable<AppUser> Search(string query, int? groupID = null );
    public AuthenticateResult Authenticate(string username, string password);   
    public UserImageDetails GetUserPhoto(string username);
    public Task<AppResult<UserImageDetails>> UpdateUserPhoto(string username, string ImageReference);
    public Task<AppResult<UserImageDetails>> RemoveUserPhoto(string username);
    public Task<AppResult<AppUser>> UpdateDisplayName(string username, string displayName);
    public Task<AppResult<AppUser>> UpdateExtraInfo(string username, List<KeyValuePair<string, string>> extraInfo);
    public void SyncProviderExtraInfo(List<string> providerExtraKey);
    public Task<AppResult<AppUser>> UpdateStars(string username, int stars);
    public Task<AppResult<AppUser>> UpdateMobile(string username, string mobile, string ext);
    public bool VerifyToken(string token);
    public void Logout(string token);
    public AppResult<AppUser> UpdatePassword(string username, string oldPassword, string newPassword);
    public AppResult<AppUser> UpdatePassword(string username, string newPassword);
    public AppResult<AppUser> AddClaims(string username, List<AddClaimRes> claims);
    public List<AppUserClaim> AddClaims(List<AppUserClaim> claims);
    public AppResult<AppUser> AddClaim(string username, AddClaimRes claim);
    public AppResult<AppUser> RemoveClaims(List<int> claimIds);
    public AppResult<AppUser> RemoveClaims(IEnumerable<AppUserClaim> claims);
    public AppResult<AppUser> RemoveClaimsByRefIds(List<string> refIds);
    public IEnumerable<AppUserClaim> GetClaims(Func<AppUserClaim, bool>? predicate = null);
    public IEnumerable<AppUser> SearchADUsers(string user);

    public bool UserExistInAD(string user);

    public AppUser CurrentUser { get; }
    public bool IsAuthenticated { get; }
    public Task<AppResult<AppUser>> Delete(string userName);

    public string GetImpersonateUser();
    public bool IsImpersonateSession();
    public Dictionary<string, string> GenerateImpersonateUserClaims(string impersonateAccount);
}