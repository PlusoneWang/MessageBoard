namespace MessageBoard.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MessageBoard.Library.Models;
    using MessageBoard.Library.ViewModels.Messages;

    using Po.Result;

    /// <summary>
    /// Message相關邏輯
    /// </summary>
    public class MessageService : BaseService
    {
        /// <summary>
        /// 設定需要排除的訊息、要取得的訊息總數，取得以留言時間降序排序的留言ViewModel陣列
        /// </summary>
        /// <param name="excludeMessages">要排除的訊息的Id清單</param>
        /// <param name="count">要取得的訊息數量</param>
        /// <returns>取得結果</returns>
        public PoResult<List<MessageListVm>> GetMessageListVm(List<Guid> excludeMessages, int count)
        {
            if (count < 0) count = 0;
            try
            {
                var messageListVms = new List<MessageListVm>();
                var messages = this.Database.Messages.Where(o => excludeMessages.All(c => o.Id != c));
                messages = messages.Where(o => o.ParentMessageId == null).OrderByDescending(o => o.CreateTime).Take(count);
                foreach (var message in messages)
                {
                    var messageListVm = new MessageListVm
                    {
                        UserId = message.UserId,
                        UserName = message.User.UserName,
                        MessageId = message.Id,
                        Context = message.Context,
                        Time = message.CreateTime,
                        AttahmentList =
                                                    message.AttachmentImages.Select(
                                                        o => new Attahment
                                                        {
                                                            ImageId = o.Id,
                                                            ImageBase64 = o.ImageBase64
                                                        }).ToList(),
                        ReplyMessages =
                            this.Database.Messages
                                .Where(o => o.ParentMessageId != null && o.ParentMessageId.Value == message.Id)
                                .OrderByDescending(o => o.CreateTime)
                                .Select(o => new MessageListVm
                                {
                                    UserId = o.UserId,
                                    UserName = o.User.UserName,
                                    MessageId = o.Id,
                                    Context = o.Context,
                                    Time = o.CreateTime,
                                    AttahmentList =
                                        o.AttachmentImages.Select(
                                            c => new Attahment
                                            {
                                                ImageId = c.Id,
                                                ImageBase64 = c.ImageBase64
                                            }).ToList(),
                                })
                                .ToList()
                    };

                    messageListVms.Add(messageListVm);
                }

                return PoResult<List<MessageListVm>>.PoSuccess(messageListVms);
            }
            catch (Exception e)
            {
                return PoResult<List<MessageListVm>>.Exception(e);
            }
        }

        /// <summary>
        /// 使用Message物件，新增訊息
        /// </summary>
        /// <param name="newMessage">Message物件</param>
        /// <returns>新增結果</returns>
        public PoResult SaveMessage(MessageCreateModel newMessage)
        {
            try
            {
                if (newMessage.ParentMessageId != null && this.Database.Messages.All(o => o.Id != newMessage.ParentMessageId.Value))
                {
                    return PoResult.Fail("找不到訊息的回覆目標");
                }

                var message = new Message
                {
                    Id = Ci.Sequential.Guid.Create(),
                    UserId = newMessage.UserId,
                    Context = newMessage.Context,
                    CreateTime = DateTime.Now,
                    ParentMessageId = newMessage.ParentMessageId
                };

                message.AttachmentImages = new List<AttachmentImage>();
                foreach (var imageBase64 in newMessage.ImageBase64List)
                {
                    message.AttachmentImages.Add(new AttachmentImage
                    {
                        Id = Ci.Sequential.Guid.Create(),
                        MessageId = message.Id,
                        ImageBase64 = imageBase64
                    });
                }
                this.Database.Messages.Add(message);
                this.Database.SaveChanges();
                return PoResult.PoSuccess();
            }
            catch (Exception e)
            {
                return PoResult.Exception(e);
            }
        }
    }
}
