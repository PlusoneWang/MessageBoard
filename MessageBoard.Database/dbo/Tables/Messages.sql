CREATE TABLE [dbo].[Messages] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_Messages_Id] DEFAULT (newsequentialid()) NOT NULL,
    [UserId]          UNIQUEIDENTIFIER NOT NULL,
    [Context]         NVARCHAR (300)   NULL,
    [CreateTime]      DATETIME         CONSTRAINT [DF_Messages_CreateTime] DEFAULT (getdate()) NOT NULL,
    [ParentMessageId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Messages_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_SubMessages_Message] FOREIGN KEY ([ParentMessageId]) REFERENCES [dbo].[Messages] ([Id])
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息資料表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Messages';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'PK_GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Messages', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'留言者Id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Messages', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Messages', @level2type = N'COLUMN', @level2name = N'Context';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'留言時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Messages', @level2type = N'COLUMN', @level2name = N'CreateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'父階留言', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Messages', @level2type = N'COLUMN', @level2name = N'ParentMessageId';

