namespace MessageBoard.Library.ViewModels.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 訊息列表ViewModel
    /// </summary>
    public class MessageListVm
    {
        /// <summary>
        /// 使用者Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 留言者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 訊息Id
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 附件圖片清單
        /// </summary>
        public List<Attahment> AttahmentList { get; set; } = new List<Attahment>();

        /// <summary>
        /// 留言時間
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 回覆訊息
        /// </summary>
        public List<MessageListVm> ReplyMessages { get; set; } = new List<MessageListVm>();
    }

    /// <summary>
    /// 訊息附件(圖片)
    /// </summary>
    public class Attahment
    {
        /// <summary>
        /// 訊息Id
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// 圖片的Base64字串
        /// </summary>
        public string ImageBase64 { get; set; }
    }
}
