CREATE TABLE [dbo].[Users] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Users_Id] DEFAULT (newsequentialid()) NOT NULL,
    [Email]       VARCHAR (200)    NOT NULL,
    [UserName]    NVARCHAR (50)    NOT NULL,
    [Password]    CHAR (44)        NOT NULL,
    [ImageBase64] NVARCHAR (MAX)   NOT NULL,
    [CreateTime]  DATETIME         CONSTRAINT [DF_Users_CreateTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者資料表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'PK_GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電子郵件', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users', @level2type = N'COLUMN', @level2name = N'Email';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users', @level2type = N'COLUMN', @level2name = N'UserName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users', @level2type = N'COLUMN', @level2name = N'Password';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'頭像Base64', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users', @level2type = N'COLUMN', @level2name = N'ImageBase64';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Users', @level2type = N'COLUMN', @level2name = N'CreateTime';

