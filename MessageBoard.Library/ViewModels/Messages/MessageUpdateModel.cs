namespace MessageBoard.Library.ViewModels.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 訊息更新Model
    /// </summary>
    public class MessageUpdateModel
    {
        /// <summary>
        /// 訊息Id
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 要刪除的附件Id
        /// </summary>
        public List<Guid> DeleteAttachmentIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 新附件
        /// </summary>
        public List<(string Name, string Path)> NewAttachments { get; set; } = new List<(string Name, string Path)>();
    }
}
