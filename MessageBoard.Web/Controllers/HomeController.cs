namespace MessageBoard.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    using Ci.Upload.Extensions;

    using MessageBoard.Library.ViewModels.Messages;
    using MessageBoard.Service;
    using MessageBoard.Web.Hubs;

    using Microsoft.AspNet.SignalR;

    using Po.Result;

    public class HomeController : BaseController
    {
        private readonly MessageService messageService;

        private readonly IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();

        public HomeController()
        {
            this.messageService = new MessageService();
        }

        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// 取得訊息列表
        /// </summary>
        /// <param name="excludeMessages">要排除的訊息的Id清單</param>
        /// <returns>執行結果及訊息列表</returns>
        public ActionResult GetMessageList(List<string> excludeMessages = null)
        {
            var guids = excludeMessages?.Select(o => new Guid(o)).ToList();
            var getResult = this.messageService.GetMessageListVmList(guids ?? new List<Guid>(), 3);
            return this.Json(getResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 送出(儲存)訊息
        /// </summary>
        /// <param name="messageContext">訊息內容</param>
        /// <param name="images">附件圖片清單</param>
        /// <returns>儲存結果</returns>
        [HttpPost]
        public ActionResult SendMessage(string messageContext, HttpPostedFileBase[] images)
        {
            // 驗證
            if (messageContext.Length > 300)
            {
                return this.Json(PoResult.Fail("訊息內容超出限制，最多為300字"));
            }

            // 驗證
            if (messageContext.Length == 0 && images == null)
            {
                return this.Json(PoResult.Fail("沒有可以儲存的訊息內容"));
            }

            // 組裝model
            var messageCreateModel = new MessageCreateModel
            {
                UserId = this.CurrentUser.Id,
                Context = messageContext,
                ParentMessageId = null
            };

            if (images != null)
            {
                foreach (var image in images)
                {
                    if (image == null || image.ContentLength == 0)
                    {
                        continue;
                    }

                    if (image.ContentLength > 1024 * 1024)
                    {
                        return this.Json(PoResult.Fail($"檔案「{image.FileName}」大小超出限制，最大1MB"));
                    }

                    try
                    {
                        using (new System.Drawing.Bitmap(image.InputStream))
                        {
                            // ignore
                        }
                    }
                    catch (Exception)
                    {
                        return this.Json(PoResult.Fail($"檔案「{image.FileName}」格式錯誤，請確定上傳的是圖片"));
                    }

                    image.InputStream.Position = 0;
                }

                foreach (var image in images)
                {
                    var fileResult = image.SaveAsLocal("AttachmentImages");
                    messageCreateModel.Images.Add(($"{fileResult.OriName}{fileResult.Extension}", fileResult.VirtualPath));
                }
            }

            var saveResult = this.messageService.SaveMessage(messageCreateModel);
            if (saveResult.Success)
            {
                var messageListVmResult = this.messageService.GetMessageListVm(saveResult.Data.Id);
                if (messageListVmResult.Success)
                {
                    this.hub.Clients.All.updateMessage(messageListVmResult.Data.MessageId, "newMessage", messageListVmResult.Data);
                }
            }
            else
            {
                foreach (var image in messageCreateModel.Images)
                {
                    var mapPath = this.Server.MapPath(image.Path);
                    if (System.IO.File.Exists(mapPath))
                    {
                        System.IO.File.Delete(mapPath);
                    }
                }
            }

            return this.Json(new PoResult { Success = saveResult.Success, Message = saveResult.Message });
        }
    }
}