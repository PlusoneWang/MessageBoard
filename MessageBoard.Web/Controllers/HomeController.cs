namespace MessageBoard.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public ActionResult GetMessageList(List<Guid> excludeMessages = null)
        {
            var getResult = this.messageService.GetMessageListVmList(excludeMessages ?? new List<Guid>(), 3);
            return this.Json(getResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 送出(儲存)訊息
        /// </summary>
        /// <param name="messageContext">訊息內容</param>
        /// <param name="images">附件圖片清單</param>
        /// <param name="parentMessageId">父階留言Id</param>
        /// <returns>儲存結果</returns>
        [HttpPost]
        public ActionResult SendMessage(string messageContext, HttpPostedFileBase[] images, Guid? parentMessageId = null)
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
                ParentMessageId = parentMessageId
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
                    if (saveResult.Data.ParentMessageId == null)
                    {
                        // not reply
                        this.hub.Clients.All.updateMessage(messageListVmResult.Data.MessageId, "newMessage", messageListVmResult.Data);
                    }
                    else
                    {
                        // is reply
                        this.hub.Clients.All.updateMessage(saveResult.Data.ParentMessageId, "newReply", messageListVmResult.Data);
                    }
                }
            }
            else
            {
                if (images != null)
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

        /// <summary>
        /// 刪除訊息
        /// </summary>
        /// <param name="messageId">訊息Id</param>
        /// <returns>刪除結果</returns>
        [HttpPost]
        public ActionResult DeleteMessage(Guid messageId)
        {
            var deleteResult = this.messageService.DeleteMessage(messageId, this.CurrentUser.Id, this.CurrentUser.Id == AppSettings.AdminId);
            if (deleteResult.Success)
            {
                this.hub.Clients.All.updateMessage(messageId, "delete");
            }

            return this.Json(deleteResult);
        }

        /// <summary>
        /// 更新訊息
        /// </summary>
        /// <param name="messageId">訊息Id</param>
        /// <param name="context">訊息內容</param>
        /// <param name="deleteImages">要刪除的圖片</param>
        /// <param name="images">附件圖片清單</param>
        /// <returns>更新結果</returns>
        [HttpPost]
        public ActionResult UpdateMessage(Guid messageId, string context, Guid[] deleteImages, HttpPostedFileBase[] images)
        {
            // 驗證
            if (context.Length > 300)
            {
                return this.Json(PoResult.Fail("訊息內容超出限制，最多為300字"));
            }


            // 驗證
            if (context.Length == 0 && images == null)
            {
                return this.Json(PoResult.Fail("沒有可以儲存的訊息內容"));
            }

            // 組裝model
            var messageUpdateModel = new MessageUpdateModel()
            {
                MessageId = messageId,
                Context = context,
                DeleteAttachmentIds = deleteImages?.ToList() ?? new List<Guid>()
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
                    messageUpdateModel.NewAttachments.Add(($"{fileResult.OriName}{fileResult.Extension}", fileResult.VirtualPath));
                }
            }

            var updateResult = this.messageService.UpdateMessage(this.CurrentUser.Id, messageUpdateModel);
            if (updateResult.Success)
            {
                var messageListVmResult = this.messageService.GetMessageListVm(updateResult.Data.Id);
                if (messageListVmResult.Success)
                {
                    this.hub.Clients.All.updateMessage(messageListVmResult.Data.MessageId, "updateMessage", messageListVmResult.Data);
                }
            }
            else
            {
                if (images != null)
                    foreach (var image in messageUpdateModel.NewAttachments)
                    {
                        var mapPath = this.Server.MapPath(image.Path);
                        if (System.IO.File.Exists(mapPath))
                        {
                            System.IO.File.Delete(mapPath);
                        }
                    }
            }

            return this.Json(new PoResult { Success = updateResult.Success, Message = updateResult.Message });
        }

        public ActionResult Lab()
        {
            return this.View();
        }
    }
}