﻿namespace MessageBoard.Library.ViewModels.Messages
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
        /// 頭像路徑
        /// </summary>
        public string HeadPortraitPath { get; set; }

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
        public List<Attachment> AttachmentList { get; set; } = new List<Attachment>();

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
    public class Attachment
    {
        /// <summary>
        /// 訊息Id
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 原始檔名
        /// </summary>
        public string Name { get; set; }
    }
}
