using AppIdentity.Domain;
using AppIdentity.IServices;
using AppIdentity.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AppIdentity.Exceptions;
using Microsoft.AspNetCore.Identity;
using AppCommon;

namespace AppIdentity.Services;

public abstract class UserProviderBase : IUserProvider
{
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly UserManager<AppUser> userManager;

    protected UserProviderBase(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
    {
        HttpContextAccessor = httpContextAccessor;
        this.userManager = userManager;
        
    }

    public abstract Task<AppResult<AppUser>> Add(AddUserRes user, bool isExternal = false);
    public abstract Task<AppResult<AppUser>> Update(UpdateUserReq updateUserRequest);
    public abstract Task<AppResult<AppUser>> Get(string userName);
    public abstract Task<AppResult<AppUser>> GetById(string userName);
    public abstract Task<AppResult<List<AppUser>>> GetByIds(List<string> userIds);

    public abstract Task<AppResult<AppUser>> GetByEmail(string email);

    public abstract IEnumerable<AppUser> Users();
    public abstract Task<List<AppUser>> UsersAsync();
    public abstract IEnumerable<AppUser> Users(List<string> usernames);
    public abstract IEnumerable<AppUser> Search(string query, int? groupID = null);
    public abstract AuthenticateResult Authenticate(string username, string password);
    public abstract UserImageDetails GetUserPhoto(string username);
    public abstract Task<AppResult<UserImageDetails>> UpdateUserPhoto(string username, string imageReference);
    public abstract Task<AppResult<UserImageDetails>> RemoveUserPhoto(string username);
    public abstract Task<AppResult<AppUser>> UpdateDisplayName(string username, string displayName);
    public abstract Task<AppResult<AppUser>> UpdateExtraInfo(string username, List<KeyValuePair<string, string>> extraInfo);
    public abstract void SyncProviderExtraInfo(List<string> providerExtraKey);
    public abstract Task<AppResult<AppUser>> UpdateStars(string username, int stars);
    public abstract Task<AppResult<AppUser>> UpdateMobile(string username, string mobile, string ext);

    public abstract AppResult<AppUser> UpdatePassword(string username, string oldPassword, string newPassword);
    public abstract AppResult<AppUser> UpdatePassword(string username, string newPassword);
    public abstract AppResult<AppUser> AddClaims(string username, List<AddClaimRes> claims);
    public abstract AppResult<AppUser> AddClaim(string username, AddClaimRes claim);
    public abstract List<AppUserClaim> AddClaims(List<AppUserClaim> claims);
    public abstract AppResult<AppUser> RemoveClaims(List<int> claimIds);
    public abstract AppResult<AppUser> RemoveClaims(IEnumerable<AppUserClaim> claims);

    public abstract IEnumerable<AppUserClaim> GetClaims(Func<AppUserClaim, bool>? predicate = null);
    public abstract AppUser CurrentUser { get; }
    public abstract Task<AppResult<AppUser>> Delete(string userName);

    public string GetImpersonateUser()
    {
        return "";
    }

    public bool IsImpersonateSession()
    {
        return false;
    }

    public Dictionary<string, string> GenerateImpersonateUserClaims(string impersonateAccount)
    {
        return new Dictionary<string, string>();
    }
    
    public virtual bool VerifyToken(string token)
    {
        if (TokenRevocationList.IsTokenRevoked(token)) return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfigration.JWTKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal != null;
        }
        catch (Exception)
        {
            return false;
        }

    }
    
    private string GetUserIdentifierFromJwtToken(string token)
    {
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        var claim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nickname");
        
        if(claim == null) throw new Exception("User identifier not found in token, or the user mapping in configuration not set correctly");

        return claim.Value;
    }
    
    public void Logout(string token)
    {
        TokenRevocationList.RevokeToken(token);
    }

    protected (string token, DateTime expireDate) GenerateToken(string username, string password,
        bool withPasswordClaim, bool isExternal = false)
    {
        // validate username and password
        var handler = new JwtSecurityTokenHandler();
        var expireDate = DateTime.Now.AddDays(1);

        var claims = new List<Claim>
        {
            new("UserName",username),
            new("ExpireDate", expireDate.ToString()),
            new("IsExternal", isExternal.ToString()),
            new(ClaimTypes.Locality, Thread.CurrentThread.CurrentCulture.Name)
        };
        if (withPasswordClaim) claims.Add(new Claim("Password", Encrypt(password)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
            Expires = expireDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(ConfigurationManager.JwtKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var secureTokenInformation = handler.CreateToken(tokenDescriptor);
        return (handler.WriteToken(secureTokenInformation), expireDate);
    }

    protected string Encrypt(string clearText)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        var encryptText = string.Empty;
        using Aes encryptor = Aes.Create();
        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ConfigurationManager.AesKey,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using MemoryStream ms = new MemoryStream();
        using CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(clearBytes, 0, clearBytes.Length);
        cs.Close();
        encryptText = Convert.ToBase64String(ms.ToArray());
        return encryptText;
    }

    protected string Decrypt(string cipherText)
    {
        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using Aes encryptor = Aes.Create();
            var clearText = string.Empty;
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ConfigurationManager.AesKey,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();
            clearText = Encoding.Unicode.GetString(ms.ToArray());

            return clearText;
        }
        catch (Exception)
        {
            return "";
        }
    }

    public abstract IEnumerable<AppUser> SearchADUsers(string user);
    public abstract bool UserExistInAD(string user);

    public abstract AppResult<AppUser> RemoveClaimsByRefIds(List<string> refIds);
   

    public bool IsAuthenticated => HttpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}