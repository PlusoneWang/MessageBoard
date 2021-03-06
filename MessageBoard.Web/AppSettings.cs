﻿namespace MessageBoard.Web
{
    using System;
    using System.Configuration;
    using System.Web;


    /// <summary>
    /// 用於取得AppSettings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Alert樣式設定
        /// </summary>
        private static string alertMode;

        /// <summary>
        /// 管理員Id
        /// </summary>
        private static Guid? adminId;

        /// <summary>
        /// Alert樣式設定
        /// </summary>
        public static string AlertMode => alertMode ?? (alertMode = ConfigurationManager.AppSettings["AlertMode"]);

        /// <summary>
        /// 管理員Id
        /// </summary>
        public static Guid AdminId => (Guid)(adminId ?? (adminId = new Guid(ConfigurationManager.AppSettings["AdminId"])));

        /// <summary>
        /// Server根目錄
        /// </summary>
        public static string ServerRoot { get; private set; }

        /// <summary>
        /// 將AppSetting中的設定值進行初始化
        /// </summary>
        public static void RegisterSettings()
        {
            ServerRoot = HttpContext.Current.Server.MapPath("~");
            var alertModeInit = AlertMode;
            var adminIdInit = AdminId;
        }
    }
}