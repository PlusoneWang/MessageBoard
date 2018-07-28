namespace MessageBoard.Service
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Web;

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
        public PoResult<List<MessageListVm>> GetMessageListVmList(List<Guid> excludeMessages, int count)
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
                        HeadPortraitPath = message.User.HeadPortraitPath,
                        MessageId = message.Id,
                        Context = message.Context,
                        Time = $"{message.CreateTime: yyyy-MM-dd HH:mm:ss}",
                        AttachmentList =
                                                    message.AttachmentImages.Select(
                                                        o => new Attachment
                                                        {
                                                            ImageId = o.Id,
                                                            Name = o.FileName,
                                                            ImagePath = o.Path,
                                                        }).ToList(),
                        ReplyMessages =
                            this.Database.Messages
                                .Where(o => o.ParentMessageId != null && o.ParentMessageId.Value == message.Id)
                                .OrderBy(o => o.CreateTime)
                                .AsEnumerable()
                                .Select(o => new MessageListVm
                                {
                                    UserId = o.UserId,
                                    UserName = o.User.UserName,
                                    HeadPortraitPath = o.User.HeadPortraitPath,
                                    MessageId = o.Id,
                                    Context = o.Context,
                                    Time = $"{o.CreateTime: yyyy-MM-dd HH:mm:ss}",
                                    AttachmentList =
                                        o.AttachmentImages.Select(
                                            c => new Attachment
                                            {
                                                ImageId = c.Id,
                                                Name = c.FileName,
                                                ImagePath = c.Path
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
        /// 使用訊息Id，取得MessageListVm
        /// </summary>
        /// <param name="messageId">訊息Id</param>
        /// <returns>取得結果</returns>
        public PoResult<MessageListVm> GetMessageListVm(Guid messageId)
        {
            try
            {
                var message = this.Database.Messages.Include(o => o.User).FirstOrDefault(o => o.Id == messageId);
                if (message == null)
                {
                    return PoResult<MessageListVm>.DbNotFound();
                }

                var messageListVm = new MessageListVm
                {
                    UserId = message.UserId,
                    UserName = message.User.UserName,
                    HeadPortraitPath = message.User.HeadPortraitPath,
                    MessageId = message.Id,
                    Context = message.Context,
                    Time = $"{message.CreateTime: yyyy-MM-dd HH:mm:ss}",
                    AttachmentList =
                                                    message.AttachmentImages.Select(
                                                        o => new Attachment
                                                        {
                                                            ImageId = o.Id,
                                                            Name = o.FileName,
                                                            ImagePath = o.Path
                                                        }).ToList(),
                    ReplyMessages =
                            this.Database.Messages
                                .Where(o => o.ParentMessageId != null && o.ParentMessageId.Value == message.Id)
                                .OrderByDescending(o => o.CreateTime)
                                .AsEnumerable()
                                .Select(o => new MessageListVm
                                {
                                    UserId = o.UserId,
                                    UserName = o.User.UserName,

                                    MessageId = o.Id,
                                    Context = o.Context,
                                    Time = $"{o.CreateTime: yyyy-MM-dd HH:mm:ss}",
                                    AttachmentList =
                                        o.AttachmentImages.Select(
                                            c => new Attachment
                                            {
                                                ImageId = c.Id,
                                                Name = c.FileName,
                                                ImagePath = c.Path
                                            }).ToList(),
                                })
                                .ToList()
                };

                return PoResult<MessageListVm>.PoSuccess(messageListVm);
            }
            catch (Exception e)
            {
                return PoResult<MessageListVm>.Exception(e);
            }
        }

        /// <summary>
        /// 使用MessageCreate物件，新增訊息，並回傳建立的訊息
        /// </summary>
        /// <param name="newMessage">Message物件</param>
        /// <returns>新增結果</returns>
        public PoResult<Message> SaveMessage(MessageCreateModel newMessage)
        {
            try
            {
                if (newMessage.ParentMessageId != null && this.Database.Messages.All(o => o.Id != newMessage.ParentMessageId.Value))
                {
                    return PoResult<Message>.Fail("找不到訊息的回覆目標");
                }

                var message = new Message
                {
                    Id = Ci.Sequential.Guid.Create(),
                    UserId = newMessage.UserId,
                    Context = newMessage.Context,
                    CreateTime = DateTime.Now,
                    ParentMessageId = newMessage.ParentMessageId,
                    AttachmentImages = new List<AttachmentImage>()
                };

                foreach (var image in newMessage.Images)
                {
                    message.AttachmentImages.Add(new AttachmentImage
                    {
                        Id = Ci.Sequential.Guid.Create(),
                        MessageId = message.Id,
                        Path = image.Path,
                        FileName = image.Name
                    });
                }

                this.Database.Messages.Add(message);
                this.Database.SaveChanges();
                return PoResult<Message>.PoSuccess(message);
            }
            catch (Exception e)
            {
                return PoResult<Message>.Exception(e);
            }
        }

        /// <summary>
        /// 使用訊息Id、使用者Id、是否為管理員，刪除訊息
        /// </summary>
        /// <param name="messageId">訊息Id</param>
        /// <param name="userId">使用者Id</param>
        /// <param name="isAdmin">是否為管理員</param>
        /// <returns>刪除結果</returns>
        public PoResult DeleteMessage(Guid messageId, Guid userId, bool isAdmin)
        {
            try
            {
                var message = this.Database.Messages.FirstOrDefault(o => o.Id == messageId && (isAdmin || o.UserId == userId));
                if (message == null)
                    return PoResult.DbNotFound();

                var imagePaths = new List<string>();
                imagePaths.AddRange(message.AttachmentImages.Select(o => o.Path));
                imagePaths.AddRange(message.Messages1.SelectMany(o => o.AttachmentImages.Select(c => c.Path)));

                this.Database.Messages.RemoveRange(message.Messages1);
                this.Database.Messages.Remove(message);

                this.Database.SaveChanges();
                foreach (var imagePath in imagePaths)
                {
                    var mapPath = HttpContext.Current.Server.MapPath(imagePath);
                    if (mapPath != null && File.Exists(mapPath))
                        File.Delete(mapPath);
                }

                return PoResult.PoSuccess();
            }
            catch (Exception e)
            {
                return PoResult.Exception(e);
            }
        }

        /// <summary>
        /// 使用使用者Id、訊息更新物件，更新訊息
        /// </summary>
        /// <param name="userId">使用者Id</param>
        /// <param name="updateMessage">訊息更新物件</param>
        /// <returns>更新結果</returns>
        public PoResult<Message> UpdateMessage(Guid userId, MessageUpdateModel updateMessage)
        {
            try
            {
                var message = this.Database.Messages.FirstOrDefault(o => o.UserId == userId && o.Id == updateMessage.MessageId);
                if (message == null)
                    return PoResult<Message>.DbNotFound();
                message.Context = updateMessage.Context;

                // delete attachment
                var toDeleteAttachmentImages = message.AttachmentImages.Where(o => updateMessage.DeleteAttachmentIds.Contains(o.Id));
                var toDeletePaths = toDeleteAttachmentImages.Select(o => o.Path).AsEnumerable().ToList();
                this.Database.AttachmentImages.RemoveRange(toDeleteAttachmentImages);
                foreach (var newAttachment in updateMessage.NewAttachments)
                {
                    this.Database.AttachmentImages.Add(
                        new AttachmentImage
                        {
                            Id = Ci.Sequential.Guid.Create(),
                            MessageId = message.Id,
                            FileName = newAttachment.Name,
                            Path = newAttachment.Path
                        });
                }

                this.Database.SaveChanges();

                foreach (var deletePath in toDeletePaths)
                {
                    var mapPath = HttpContext.Current.Server.MapPath(deletePath);
                    if (mapPath != null && File.Exists(mapPath))
                        File.Delete(mapPath);
                }

                return PoResult<Message>.PoSuccess(message);
            }
            catch (Exception e)
            {
                return PoResult<Message>.Exception(e);
            }
        }
    }
}
