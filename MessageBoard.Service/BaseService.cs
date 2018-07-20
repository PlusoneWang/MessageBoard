namespace MessageBoard.Service
{
    using System;

    using MessageBoard.Library.Models;

    /// <summary>
    /// 基底Service，定義了其他Service會用到的共用程式碼
    /// </summary>
    public class BaseService : IDisposable
    {
        /// <summary>
        /// MessageBoardEntities執行個體
        /// </summary>
        protected MessageBoardEntities Database { get; set; } = new MessageBoardEntities();

        public void Dispose()
        {
            this.Database.Dispose();
        }
    }
}
