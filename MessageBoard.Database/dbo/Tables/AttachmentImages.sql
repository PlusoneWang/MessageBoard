CREATE TABLE [dbo].[AttachmentImages] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_AttachmentImages_Id] DEFAULT (newsequentialid()) NOT NULL,
    [MessageId]   UNIQUEIDENTIFIER NOT NULL,
    [ImageBase64] VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_AttachmentImages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AttachmentImages_Message] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[Messages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息附件圖片資料表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AttachmentImages';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'PK_GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AttachmentImages', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'留言Id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AttachmentImages', @level2type = N'COLUMN', @level2name = N'MessageId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'圖片', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AttachmentImages', @level2type = N'COLUMN', @level2name = N'ImageBase64';

