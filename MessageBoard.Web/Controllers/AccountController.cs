namespace MessageBoard.Web.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    using MessageBoard.Library.ViewModels.Users;
    using MessageBoard.Service;

    using Po.Helper;

    /// <summary>
    /// 帳戶控制相關Controller
    /// </summary>
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly UserService userService;

        public AccountController()
        {
            this.userService = new UserService();
        }

        /// <summary>
        /// 登入Get
        /// </summary>
        /// <param name="returnUrl">登入後前往的位址</param>
        /// <returns>登入頁面</returns>
        public ActionResult Login(string returnUrl = null)
        {
            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                if (returnUrl == null || !this.Url.IsLocalUrl(returnUrl))
                {
                    return this.RedirectToAction("Index", "Home");
                }

                return this.Redirect(returnUrl);
            }

            this.TempData["returnUrl"] = returnUrl;

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVm loginVm)
        {
            this.SignOutCurrentUser();
            if (this.ModelState.IsValid)
            {
                var verifyResult = this.userService.VerifyUser(loginVm.Email, loginVm.Password);
                if (verifyResult.Success)
                {
                    var user = this.userService.GetUserByEmail(loginVm.Email).Data;
                    IdentityTool.Authentication(
                        this.AuthenticationManager,
                        loginVm.RememberMe,
                        user.Id.ToString().ToUpper(),
                        user.Email,
                        user.UserName);
                    var returnUrl = this.TempData["returnUrl"]?.ToString();
                    if (returnUrl == null || !this.Url.IsLocalUrl(returnUrl))
                    {
                        return this.RedirectToAction("Index", "Home");
                    }

                    return this.Redirect(returnUrl);
                }

                this.TempData["alert"] = "電子郵件或密碼錯誤";
            }
            else
            {
                this.SetModelStateError();
            }

            return this.View(loginVm);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns>登入頁</returns>
        [Authorize]
        public ActionResult Logout()
        {
            this.SignOutCurrentUser();
            return this.RedirectToAction("Login");
        }

        /// <summary>
        /// 註冊頁面Get
        /// </summary>
        /// <returns>註冊頁面</returns>
        [HttpGet]
        public ActionResult Register()
        {
            return this.View();
        }

        /// <summary>
        /// 註冊Post
        /// </summary>
        /// <param name="userCreateVm">新建使用者ViewModel</param>
        /// <param name="imageFile">頭像圖片</param>
        /// <returns>註冊結果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserCreateVm userCreateVm, HttpPostedFileBase imageFile)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetModelStateError();
                return this.View(userCreateVm);
            }

            if (imageFile == null || imageFile.ContentLength == 0)
            {
                this.TempData["alert"] = "請選擇頭像";
                return this.View(userCreateVm);
            }

            if (imageFile.ContentLength > 1024 * 1024)
            {
                this.TempData["alert"] = "頭像大小超出限制，最大1MB";
                return this.View(userCreateVm);
            }

            try
            {
                using (new System.Drawing.Bitmap(imageFile.InputStream))
                {
                    // ignore
                }
            }
            catch (Exception)
            {
                this.TempData["alert"] = "選擇的檔案格式錯誤，請確定上傳的是圖片";
                return this.View(userCreateVm);
            }

            imageFile.InputStream.Position = 0;
            var binaryReader = new System.IO.BinaryReader(imageFile.InputStream);
            var readBytes = binaryReader.ReadBytes((int)imageFile.InputStream.Length);
            userCreateVm.ImageBase64 = Convert.ToBase64String(readBytes);

            var result = this.userService.CreateUser(userCreateVm);
            if (result.Success)
            {
                this.TempData["alert"] = "註冊成功，請登入以開始使用網站功能。";
                return this.RedirectToAction("Login");
            }

            this.TempData["alert"] = result.Message.ReplaceContent();

            return this.View(userCreateVm);
        }

        public string HeadPortrait()
        {
            return this.userService.GetUser(this.CurrentUser.Id).Data.ImageBase64;
        }
    }
}