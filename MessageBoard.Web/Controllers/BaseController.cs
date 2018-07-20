namespace MessageBoard.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    /// <summary>
    /// 基底控制器，定義了其他控制器會用到的共用程式碼
    /// </summary>
    [Authorize]
    public class BaseController : Controller
    {
        /// <summary>
        /// 當前的使用者
        /// </summary>
        public AppUser CurrentUser { get; set; }

        /// <summary>
        /// 目前Controller
        /// </summary>
        public string CurrentController { get; set; }

        /// <summary>
        /// 目前Action
        /// </summary>
        public string CurrentAction { get; set; }

        /// <summary>
        /// 驗證管理員
        /// </summary>
        protected IAuthenticationManager AuthenticationManager => this.HttpContext.GetOwinContext().Authentication;

        /// <summary>
        /// 初始化呼叫建構函式時可能無法使用的資料。
        /// </summary>
        /// <param name="requestContext">HTTP 內容和路由資料。</param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.CurrentUser = new AppUser(this.User);
            this.CurrentController = Convert.ToString(this.ControllerContext.RouteData.Values["controller"]);
            this.CurrentAction = Convert.ToString(this.ControllerContext.RouteData.Values["action"]);
        }

        /// <summary>
        /// 複寫: 在叫用動作方法之前呼叫
        /// </summary>
        /// <param name="filterContext">目前要求和動作的相關資訊</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // 用於設定TempData["alert"]要在前端使用的彈出視窗樣式，預設值:default。
            // 參考: ~/Shared/_TempDataAlert.cshtml
            this.ViewBag.AlertMode = AppSettings.AlertMode;

            // 取得當前的User
            this.ViewBag.CurrentUser = this.CurrentUser;
        }

        /// <summary>
        /// 將ModelState中的驗證失敗錯誤訊息加到TempData["alert"]裡
        /// </summary>
        protected void SetModelStateError()
        {
            var errors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            foreach (var error in errors)
            {
                this.TempData["alert"] += error.ErrorMessage + "\n";
            }
        }

        /// <summary>
        /// 登出當前的使用者
        /// </summary>
        protected void SignOutCurrentUser()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
        }
    }
}