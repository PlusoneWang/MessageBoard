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
        /// <param name="additionalClaim">The permissions.</param>
        public static void Authentication(IAuthenticationManager authenticationManager, bool rememberMe, string id, params ClaimValueStruct[] additionalClaim)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id) }, DefaultAuthenticationTypes.ApplicationCookie);

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
        public string Id => this.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}