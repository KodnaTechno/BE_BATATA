using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AppIdentity.Database;
using AppIdentity.Domain;
using AppIdentity.Exceptions;
using AppIdentity.IServices;
using AppIdentity.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AppIdentity.Services;

public class AppUserProvider : UserProvider
{
    private readonly AppIdentityDbContext _dbContext;
    private readonly SignInManager<AppUser> _signInManager;

    public AppUserProvider(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, AppIdentityDbContext dbContext, IGroupProvider groupProvider, SignInManager<AppUser> signInManager, ISettingsService settingsService)
        : base(userManager, httpContextAccessor, dbContext, groupProvider, settingsService)
    {
        this._dbContext = dbContext;
        _signInManager = signInManager;
    }

    public override async Task<AppResult<AppUser>> Update(UpdateUserReq updateUserRequest)
    {
        var userToUpdate = await UserManager.FindByIdAsync(updateUserRequest.Id);

        if (userToUpdate is null)
        {
            return new() { Succeeded = false, Message = "User not found" };
        }

        userToUpdate.Mobile = updateUserRequest.Mobile ?? userToUpdate.Mobile;
        userToUpdate.DisplayName = updateUserRequest.DisplayName ?? userToUpdate.DisplayName;
        userToUpdate.Email = updateUserRequest.Email ?? userToUpdate.Email;
        userToUpdate.Ext = updateUserRequest.Ext ?? userToUpdate.Ext;
        userToUpdate.Image = updateUserRequest.Image ?? userToUpdate.Image;
        
        var result = await UserManager.UpdateAsync(userToUpdate);
        if (result.Succeeded is false) return new() { Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null };
        return new AppResult<AppUser> { Succeeded = true, Message = "", Data = userToUpdate };
    }


    public override async Task<AppResult<AppUser>> Add(AddUserRes user, bool isExternal = false)
    {
        var appUser = new AppUser
        {
            UserName = user.Username,
            DisplayName = user.DisplayName,
            NormalizedUserName = user.Username.Normalize(),
            Email = user.Email,
            NormalizedEmail = user.Email.Normalize(),
            EmailConfirmed = true,
            AccessFailedCount = 0,
            Stars = user.Stars,
            Mobile = user.Mobile,
            Ext = user.Ext,
            IsExternal = isExternal,
            ExtraInfo = user.ExtraInfo ?? new List<KeyValuePair<string, string>>(),
            ProviderExtraInfo = new List<KeyValuePair<string, string>>(),
            CreatedAt = DateTime.UtcNow,
            Image = user.Image
        };
        
        var result = await UserManager.CreateAsync(appUser, user.Password);
        if (result.Succeeded is false) return new() {Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser> {Succeeded = true, Message = "", Data = appUser}; 
    }

    public override AuthenticateResult Authenticate(string username, string password)
    {
        var user = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (user is null) throw new UserNotFoundException(username);
        
        var result = _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true).GetAwaiter().GetResult();
        switch (result)
        {
            case var _ when result.Succeeded:
                var generatedToken = GenerateToken(username, password, false, user.IsExternal);
                return new AuthenticateResult
                {
                    AppUser = user,
                    Succeeded = true,
                    Token = generatedToken.token,
                    ExpiryDate = generatedToken.expireDate
                };

            case var _ when result.IsLockedOut:
                return new AuthenticateResult
                {
                    AppUser = user,
                    Succeeded = false,
                    Token = null,
                    ExpiryDate = default,
                    IsLocked = true
                };

            default:
                return new AuthenticateResult
                {
                    AppUser = user,
                    Succeeded = false,
                    Token = null,
                    ExpiryDate = default
                };
        }
    }

    public override UserImageDetails GetUserPhoto(string username)
    {
        var user = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (user is null) return null;// throw new UserNotFoundException(username);
        return new UserImageDetails
        {
            ImageReference = user.Image,
            IsStream = false
        };
    }

    public override Task<AppResult<UserImageDetails>> UpdateUserPhoto(string username, string ImageReference)
    {
        var user = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (user is null) throw new UserNotFoundException(username);

        var userDetails = _dbContext.Users.Find(user.Id);
        userDetails.Image = ImageReference;
        var result = new AppResult<UserImageDetails>
        {
            Data = new UserImageDetails { ImageReference = ImageReference },
            Succeeded = _dbContext.SaveChanges() > 0,
        };
        return Task.FromResult(result);
    }
    public override Task<AppResult<UserImageDetails>> RemoveUserPhoto(string username)
    {
        return UpdateUserPhoto(username, null);
    }
    public override void SyncProviderExtraInfo(List<string> providerExtraKey)
    {
        throw new NotValidOperationException("Standalone user provider does not support provider extra info");
    }

    public override AppResult<AppUser> UpdatePassword(string username, string oldPassword, string newPassword)
    {
        var appUser = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (appUser is null) return new AppResult<AppUser>(){Succeeded = false, Message = "User not found", Data = null};
        var result = UserManager.ChangePasswordAsync(appUser, oldPassword, newPassword).GetAwaiter().GetResult();
        if (result.Succeeded is false) return new AppResult<AppUser>(){Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser>(){Succeeded = true, Message = "", Data = appUser};
    }

    public override AppResult<AppUser> UpdatePassword(string username, string newPassword)
    {
        var appUser = UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (appUser is null) return new AppResult<AppUser>(){Succeeded = false, Message = "User not found", Data = null};
        var token = UserManager.GeneratePasswordResetTokenAsync(appUser).GetAwaiter().GetResult();
        var result = UserManager.ResetPasswordAsync(appUser, token, newPassword).GetAwaiter().GetResult();
        if (result.Succeeded is false) return new AppResult<AppUser>(){Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null};
        return new AppResult<AppUser>(){Succeeded = true, Message = "", Data = appUser};
    }

    public override Task<AppResult<AppUser>> Delete(string userName)
    {
        var appUser = UserManager.FindByNameAsync(userName).GetAwaiter().GetResult();
        if(appUser is null)
            Task.FromResult(new AppResult<AppUser>() { Succeeded = false, Message = "User not found!", Data = null });
        // delete user from groups
        var appUserGroup = _dbContext.AppGroupUsers.Where(g => g.UserId == appUser.Id);
        _dbContext.RemoveRange(appUserGroup);
        _dbContext.SaveChanges();
        var result =  UserManager.DeleteAsync(appUser).GetAwaiter().GetResult();
        if (result.Succeeded is false) 
            return Task.FromResult( new AppResult<AppUser>() { Succeeded = false, Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()), Data = null });
        return Task.FromResult(new AppResult<AppUser>() { Succeeded = true, Message = "", Data = appUser });
    }

   

    public override IEnumerable<AppUser> SearchADUsers(string user)
    {
        throw new NotImplementedException();
    }

    public override bool UserExistInAD(string user)
    {
        throw new NotImplementedException();
    }
}