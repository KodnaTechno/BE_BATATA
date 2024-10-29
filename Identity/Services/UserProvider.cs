using System.IdentityModel.Tokens.Jwt;
using AppIdentity.Database;
using AppIdentity.Domain;
using AppIdentity.IServices;
using AppIdentity.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity.Services;

public abstract class UserProvider : UserProviderBase
{
    protected readonly UserManager<AppUser> UserManager;
    protected readonly AppIdentityDbContext DbContext;
    private readonly IGroupProvider _groupProvider;
    private readonly ISettingsService _settingsService;


    protected UserProvider(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, AppIdentityDbContext dbContext, IGroupProvider groupProvider) : base(httpContextAccessor, userManager)
    {
        UserManager = userManager;
        DbContext = dbContext;
        _groupProvider = groupProvider;
    }

    public abstract override Task<AppResult<AppUser>> Add(AddUserRes user, bool isExternal = false);
    public abstract override Task<AppResult<AppUser>> Update(UpdateUserReq updateUserRequest);
    public override async Task<AppResult<AppUser>> GetById(string userId)
    {
        var user = await UserManager.FindByIdAsync(userId);
        if (user is null)
            return new()
            {
                Succeeded = false,
                Message = "User not found",
                Data = null
            };

        return new()
        {
            Succeeded = true,
            Data = user
        }; ;
    }

    public override async Task<AppResult<List<AppUser>>> GetByIds(List<string> userIds)
    {
        var users = await UserManager.Users.Where(u => userIds.Contains(u.Id))
            .ToListAsync(); 
        if (users.Count == 0)
            return new()
            {
                Succeeded = false,
                Message = "Users not found",
                Data = null
            };

        return new()
        {
            Succeeded = true,
            Data = users
        }; ;
    }



    public override async Task<AppResult<AppUser>> Get(string username)
    {
        var user = await UserManager.FindByNameAsync(username);
        if (user is null)
            return new()
            {
                Succeeded = false,
                Message = "User not found",
                Data = null
            };
        
        return new()
        {
            Succeeded = true,
            Data = user
        };;
    }

    public override async Task<AppResult<AppUser>> GetByEmail(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user is null)
            return new()
            {
                Succeeded = false,
                Message = "User not found",
                Data = null
            };
        
        return new()
        {
            Succeeded = true,
            Data = user
        };;
    }

    public override IEnumerable<AppUser> Users()
    {
        return UserManager.Users.ToList();
    }

    public override Task<List<AppUser>> UsersAsync()
    {
        return UserManager.Users.ToListAsync();
    }


    public override IEnumerable<AppUser> Users(List<string> usernames)
    {
        return UserManager.Users.Where(u => usernames.Contains(u.UserName)).ToList();
    }

    public override IEnumerable<AppUser> Search(string query,  int? groupID = null)
    {
        query = query?.ToLower();
        return groupID.HasValue
            ?
            _groupProvider.GetGroupUsers(groupID.Value).Where(u => u.UserName.ToLower().Contains(query) ||
                                             u.DisplayName.ToLower().Contains(query) ||
                                             u.Email.ToLower().Contains(query)).ToList()

            : UserManager.Users.Where(u => u.UserName.ToLower().Contains(query) || 
                                             u.DisplayName.ToLower().Contains(query) ||
                                             u.Email.ToLower().Contains(query)).ToList();
    }

    public override async Task<AppResult<AppUser>> UpdateDisplayName(string username, string displayName)
    {
        var userResult = await Get(username);
        if(userResult.Succeeded is false) return userResult;
        var user = userResult.Data;
        user.DisplayName = displayName;
        var result = await UserManager.UpdateAsync(user);
        if (result.Succeeded is false) return new() {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser> {Succeeded = false, Message = "", Data = user};
    }
    
    public override async Task<AppResult<AppUser>> UpdateStars(string username, int stars)
    {
        var userResult = await Get(username);
        if(userResult.Succeeded is false) return userResult;
        var user = userResult.Data;
        user.Stars = stars;
        var result = await UserManager.UpdateAsync(user);
        if (result.Succeeded is false) return new() {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser> {Succeeded = false, Message = "", Data = user};
    }

    public override async Task<AppResult<AppUser>> UpdateMobile(string username, string mobile, string ext)
    {
        var userResult = await Get(username);
        if(userResult.Succeeded is false) return userResult;
        var user = userResult.Data;
        user.Mobile = mobile;
        user.Ext = ext;
        var result = await UserManager.UpdateAsync(user);
        if (result.Succeeded is false) return new() {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser> {Succeeded = false, Message = "", Data = user}; 
    }

    public override async Task<AppResult<AppUser>> UpdateExtraInfo(string username, List<KeyValuePair<string, string>> extraInfo)
    {
        var userResult = await Get(username);
        if(userResult.Succeeded is false) return userResult;
        var user = userResult.Data;
        user.ExtraInfo = extraInfo;
        var result = await UserManager.UpdateAsync(user);
        if (result.Succeeded is false) return new() {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser> {Succeeded = false, Message = "", Data = user}; 
    }

    public abstract override AuthenticateResult Authenticate(string username, string password);

    public abstract override UserImageDetails GetUserPhoto(string username);
    public abstract override Task<AppResult<UserImageDetails>> UpdateUserPhoto(string username, string ImageReference);
    public abstract override Task<AppResult<UserImageDetails>> RemoveUserPhoto(string username);

    public abstract override void SyncProviderExtraInfo(List<string> providerExtraKey);
    
    public override AppResult<AppUser> AddClaims(string username, List<AddClaimRes> claims)
    {
        var user = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if(user == null) return new AppResult<AppUser> {Succeeded = false, Message = "User not found", Data = null};
        var appUserClaims = claims.Select(c => new AppUserClaim
        {
            UserId = user.Id,
            Username = username,
            ClaimType = c.ClaimType,
            ClaimValue = c.ClaimValue,
            ClaimValueRef = c.ClaimValueRef
        });
        DbContext.AppUserClaims.AddRange(appUserClaims);
        DbContext.SaveChanges();
        return new AppResult<AppUser> {Succeeded = true, Message = "", Data = user};
    }

    public override List<AppUserClaim> AddClaims(List<AppUserClaim> claims)
    {
        DbContext.ChangeTracker.Clear();
        DbContext.AppUserClaims.AddRange(claims);
        DbContext.SaveChanges();
        return claims;  
    }

    public override AppResult<AppUser> AddClaim(string username, AddClaimRes claim)
    {
        var user = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if(user == null) return new AppResult<AppUser> {Succeeded = false, Message = "User not found", Data = null};
        var appUserClaim =new AppUserClaim
        {
            UserId = user.Id,
            Username = username,
            ClaimType = claim.ClaimType,
            ClaimValue = claim.ClaimValue,
            ClaimValueRef = claim.ClaimValueRef
        };
        DbContext.AppUserClaims.Add(appUserClaim);
        DbContext.SaveChanges();
        return new AppResult<AppUser> {Succeeded = true, Message = "", Data = user};
    }


    public override AppResult<AppUser> RemoveClaims(IEnumerable<AppUserClaim> claims)
    {
        DbContext.AppUserClaims.RemoveRange(claims);
        DbContext.SaveChanges();
        return new AppResult<AppUser> { Succeeded = true, Message = "", Data = null };
    }


    public override AppResult<AppUser> RemoveClaims(List<int> claimIds)
    {
        DbContext.ChangeTracker.Clear();
        foreach (var claim in claimIds.Chunk(2000))
        { 
           var claims = DbContext.AppUserClaims.Where(c => claim.Contains(c.Id));
            DbContext.AppUserClaims.RemoveRange(claims);
            DbContext.SaveChanges();
        }
        return new AppResult<AppUser> {Succeeded = true, Message = "", Data = null};
    }
    public override AppResult<AppUser> RemoveClaimsByRefIds(List<string> claimIds)
    {
        DbContext.AppUserClaims.RemoveRange(DbContext.AppUserClaims.Where(c=> claimIds.Contains(c.ClaimValueRef)));
        DbContext.SaveChanges();
      
        return new AppResult<AppUser> {Succeeded = true, Message = "", Data = null};
    }

    public override IEnumerable<AppUserClaim> GetClaims(Func<AppUserClaim, bool>? predicate = null)
    {
        if (predicate is null) return new List<AppUserClaim>();
        
        return DbContext.AppUserClaims.AsNoTracking().Where(predicate);
    }


    //public override AppUser CurrentUser => UserManager.GetUserAsync(HttpContextAccessor?.HttpContext?.User).GetAwaiter().GetResult();
    // public override AppUser CurrentUser =>
    //     HttpContextAccessor?.HttpContext == null
    //         ? null
    //         : (HttpContextAccessor?.HttpContext?.User.FindFirst("UserName")?.Value != null
    //             ? UserManager.FindByNameAsync(HttpContextAccessor?.HttpContext?.User.FindFirst("UserName")?.Value)
    //                 ?.Result
    //             : null);

    // public override AppUser CurrentUser
    // {
    //     get
    //     {
    //         var userName = HttpContextAccessor?.HttpContext?.User?.FindFirst(_settingsService.UsernameMapping())?.Value;
    //
    //         if (userName == null)
    //         {
    //             return null;
    //         }
    //
    //         return UserManager.FindByNameAsync(userName)?.GetAwaiter().GetResult();
    //     }
    // }
    
    public override AppUser CurrentUser
    {
        
        get
        {
            var mappingUsername = _settingsService.IsSSOEnabled() ? _settingsService.UsernameMapping() : "UserName";

            // Try to extract username from the identity token first
            var userName = HttpContextAccessor?.HttpContext?.User?.FindFirst(mappingUsername)?.Value;

            if (userName != null)
            {
                return UserManager.FindByNameAsync(userName)?.GetAwaiter().GetResult();
            }

            // If username is not found, try to extract from the access token
            var accessToken = HttpContextAccessor?.HttpContext?.Request?.Headers["Access-Token"].FirstOrDefault()?.Replace("Bearer ", "");

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(accessToken))
                {
                    var jwtToken = handler.ReadJwtToken(accessToken);
                    userName = jwtToken.Claims.FirstOrDefault(c => c.Type == mappingUsername)?.Value;
                }
            }
            else
            {
                var cookieToken = HttpContextAccessor?.HttpContext?.Request.Cookies["X-Access-Token"];
                if (!string.IsNullOrWhiteSpace(cookieToken)) {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(cookieToken))
                    {
                        var jwtToken = handler.ReadJwtToken(cookieToken);
                        userName = jwtToken.Claims.FirstOrDefault(c => c.Type == mappingUsername)?.Value;
                    }
                }
            }

            if (userName == null)
            {
                return null;
            }

            return UserManager.FindByNameAsync(userName)?.GetAwaiter().GetResult();
        }
    }

    public abstract override Task<AppResult<AppUser>> Delete(string userName);
}