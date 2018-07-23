namespace MessageBoard.Web
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    /// <summary>
    /// 自訂宣告結構
    /// </summary>
    public struct ClaimValueStruct
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Field;

        /// <summary>
        /// 欄位值
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// IdentityTool
    /// </summary>
    public class IdentityTool
    {
        /// <summary>
        /// Identity驗證
        /// </summary>
        /// <param name="authenticationManager">The authentication manager.</param>
        /// <param name="rememberMe">if set to <c>true</c> [remember me].</param>
        /// <param name="id">The identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="name">The user name.</param>
        /// <param name="additionalClaim">The permissions.</param>
        public static void Authentication(IAuthenticationManager authenticationManager, bool rememberMe, string id, string email, string name, params ClaimValueStruct[] additionalClaim)
        {
            var identity = new ClaimsIdentity(
                new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, id),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Name, name),
                    },
                DefaultAuthenticationTypes.ApplicationCookie);

            if (additionalClaim != null && additionalClaim.Length > 0)
            {
                foreach (var permission in additionalClaim)
                {
                    identity.AddClaim(new Claim(permission.Field, permission.Value));
                }
            }

            if (rememberMe)
            {
                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14) }, identity);
                return;
            }

            authenticationManager.SignIn(identity);
        }
    }

    /// <summary>
    /// App使用者
    /// </summary>
    public class AppUser : ClaimsPrincipal
    {
        public AppUser(IPrincipal claimsPrincipal) : base(claimsPrincipal)
        {
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id => new Guid(this.FindFirst(ClaimTypes.NameIdentifier).Value);

        /// <summary>
        /// Email
        /// </summary>
        public string Email => this.FindFirst(ClaimTypes.Email).Value;

        /// <summary>
        /// Name
        /// </summary>
        public string Name => this.FindFirst(ClaimTypes.Name).Value;
    }
}