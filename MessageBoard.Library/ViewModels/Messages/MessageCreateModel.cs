namespace MessageBoard.Library.ViewModels.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 新增訊息Model
    /// </summary>
    public class MessageCreateModel
    {
        /// <summary>
        /// 新增訊息的使用者Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 附件圖片的Base64字串及原始檔名
        /// </summary>
        public List<(string Name, string Path)> Images { get; set; } = new List<(string Name, string Path)>();

        /// <summary>
        /// 父階留言Id
        /// </summary>
        public Guid? ParentMessageId { get; set; }
    }
}
