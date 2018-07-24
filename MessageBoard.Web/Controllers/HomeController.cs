namespace MessageBoard.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    using MessageBoard.Library.ViewModels.Messages;
    using MessageBoard.Service;

    using Po.Result;

    public class HomeController : BaseController
    {
        private readonly MessageService messageService;

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
            var getResult = this.messageService.GetMessageListVm(excludeMessages ?? new List<Guid>(), 3);
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
            if (messageContext.Length > 300)
                return this.Json(PoResult.Fail("訊息內容超出限制，最多為300字"));

            var messageCreateModel = new MessageCreateModel
            {
                UserId = this.CurrentUser.Id,
                Context = messageContext,
                ParentMessageId = null
            };

            if (images != null)
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
                    var binaryReader = new System.IO.BinaryReader(image.InputStream);
                    var readBytes = binaryReader.ReadBytes((int)image.InputStream.Length);
                    messageCreateModel.ImageBase64List.Add(Convert.ToBase64String(readBytes));
                }

            var saveResult = this.messageService.SaveMessage(messageCreateModel);

            // TODO update all client via signalr
            return this.Json(saveResult);
        }
    }
}