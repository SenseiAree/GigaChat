USE master

IF EXISTS(SELECT [name] FROM sys.databases WHERE name = N'GigaChatDB' OR '[' + name + ']' = N'GigaChatDB')
    DROP DATABASE GigaChatDB
GO

CREATE DATABASE GigaChatDB
GO

USE GigaChatDB
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'Role' OR '[' + name + ']' = N'Role')
    DROP TABLE [dbo].[Role]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'[User]' OR '[' + name + ']' = N'[User]')
    DROP TABLE [dbo].[User]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'UserProfile' OR '[' + name + ']' = N'UserProfile')
    DROP TABLE [dbo].[UserProfile]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'FriendList' OR '[' + name + ']' = N'FriendList')
    DROP TABLE [dbo].[FriendList]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'Chat' OR '[' + name + ']' = N'Chat')
    DROP TABLE [dbo].[Chat]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'Message' OR '[' + name + ']' = N'Message')
    DROP TABLE [dbo].[Message]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'StarredMessage' OR '[' + name + ']' = N'StarredMessage')
    DROP TABLE [dbo].[StarredMessage]
GO

IF EXISTS(SELECT [name] FROM sys.tables WHERE name = N'Feedback' OR '[' + name + ']' = N'Feedback')
    DROP TABLE [dbo].[Feedback]
GO


CREATE TABLE [Role](
    RoleId INT IDENTITY(1,1),
    RoleName VARCHAR(10) NOT NULL,

    CONSTRAINT pk_RoleId PRIMARY KEY(RoleId),
    CONSTRAINT uq_RoleName UNIQUE(RoleName)
)
GO

CREATE TABLE [dbo].[User](
    UserId INT IDENTITY(1000,1),
    EmailId VARCHAR(40) NOT NULL,
    [Password] VARCHAR(20) NOT NULL,
    RoleId INT NOT NULL,
    DisplayName VARCHAR(50) NOT NULL,
    DateOfBirth DATE NOT NULL,
    RegistrationTime DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT pk_UserId_User PRIMARY KEY(UserId),
    CONSTRAINT uq_EmailId UNIQUE(EmailId),    
    CONSTRAINT chk_Password CHECK(LEN([Password]) BETWEEN 8 AND 16),
    CONSTRAINT fk_RoleId FOREIGN KEY(RoleId) REFERENCES [Role](RoleId),
    CONSTRAINT chk_DateOfBirth CHECK(DateOfBirth < GETDATE())
)
GO

CREATE TABLE UserProfile(
    UserId INT CONSTRAINT pk_UserId_UserProfile PRIMARY KEY,
    Avatar VARCHAR(255) NOT NULL DEFAULT 'Avatar1.jpg',
    IsActive VARCHAR(7) NOT NULL DEFAULT 'Offline',
    Theme VARCHAR(5) NOT NULL DEFAULT 'Light',
    AvailabilityStatus VARCHAR(30) DEFAULT 'Available' NOT NULL,

    CONSTRAINT fk_UserId_UserProfile FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT chk_AvailabilityStatus CHECK (AvailabilityStatus IN ('Available','Busy','Be right back','Do not disturb','Appear Offline')),
    CONSTRAINT chk_IsActive CHECK (IsActive IN ('Online','Offline')),
    CONSTRAINT chk_Theme CHECK (Theme IN ('Light','Dark'))
)
GO

CREATE TABLE FriendList(
    UserId INT,
    FriendId INT,
    IsBlocked BIT DEFAULT 0 NOT NULL,

    CONSTRAINT pk_FriendList PRIMARY KEY (UserId, FriendId),
    CONSTRAINT fk_UserId_FriendList FOREIGN KEY(UserId) REFERENCES [User](UserId),
    CONSTRAINT fk_FriendId FOREIGN KEY(FriendId) REFERENCES [User](UserId)
)
GO

CREATE TABLE Chat(
    ChatId INT CONSTRAINT pk_ChatId PRIMARY KEY IDENTITY(10,1),
    InitiatorId INT NOT NULL,
    RecipientId INT NOT NULL,

    CONSTRAINT fk_InitiatorId FOREIGN KEY (InitiatorId) REFERENCES [User](UserId),
    CONSTRAINT fk_RecipientId FOREIGN KEY (RecipientId) REFERENCES [User](UserId),
    CONSTRAINT uq_Chat UNIQUE (InitiatorId, RecipientId),
    CONSTRAINT chk_Chat CHECK (InitiatorId <> RecipientId)
)
GO

CREATE TABLE [Message](
    MessageId INT IDENTITY(5000,1),
    ChatId INT NOT NULL,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content NVARCHAR(200) NOT NULL,
    SentTime DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead VARCHAR(10) NOT NULL DEFAULT 'Sent',

    CONSTRAINT pk_MessageId PRIMARY KEY(MessageId),
    CONSTRAINT chk_IsRead CHECK (IsRead IN ('Sent','Delivered')),
    CONSTRAINT fk_ChatId FOREIGN KEY (ChatId) REFERENCES Chat(ChatId),
    CONSTRAINT fk_SenderId FOREIGN KEY (SenderId) REFERENCES [User](UserId),
    CONSTRAINT fk_ReceiverId FOREIGN KEY (ReceiverId) REFERENCES [User](UserId)
)
GO

CREATE TABLE StarredMessage (
    StarredMessageId INT IDENTITY(1,1),
    MessageId INT NOT NULL,
    UserId INT NOT NULL,

    CONSTRAINT pk_StarredMessageId PRIMARY KEY(StarredMessageId),
    CONSTRAINT fk_MessageId FOREIGN KEY (MessageId) REFERENCES [Message](MessageId),
    CONSTRAINT fk_UserId_Message FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

CREATE TABLE Feedback(
    FeedbackId INT IDENTITY(2000,1),
    UserId INT NOT NULL,
    UserFeedback VARCHAR(255) NOT NULL,
    Rating INT NOT NULL,
    PostedAt DATETIME NOT NULL DEFAULT GETDATE(),
    AdminReply VARCHAR(255),
    AdminReplyTime DATETIME,

    CONSTRAINT pk_Feedback PRIMARY KEY(FeedbackId),
    CONSTRAINT fk_UserId_Feedback FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT chk_Rating CHECK (Rating BETWEEN 1 AND 5)
)
GO

CREATE FUNCTION ufn_CalculateAge
(
    @DateOfBirth DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @CurrentDate DATE = GETDATE();
    DECLARE @Age INT;

    SET @Age = DATEDIFF(YEAR, @DateOfBirth, @CurrentDate);

    IF (MONTH(@DateOfBirth) > MONTH(@CurrentDate)) OR
        (MONTH(@DateOfBirth) = MONTH(@CurrentDate) AND DAY(@DateOfBirth) > DAY(@CurrentDate))
    BEGIN
        SET @Age = @Age - 1;
    END;

    RETURN @Age;
END

GO

CREATE PROCEDURE usp_RegisterUser 
    @EmailId VARCHAR(40),
    @Password VARCHAR(20),
    @DisplayName VARCHAR(50),
    @DateOfBirth DATE,
    @UserId INT OUT
    
AS  
BEGIN  
    DECLARE @RoleId INT;

    BEGIN TRY

        SELECT @RoleId = RoleId FROM [Role] WHERE RoleName = 'User';

        IF EXISTS (SELECT EmailId FROM [User] WHERE EmailId = @EmailId)  
            RETURN -1        
 
        IF ( LEN(@EmailId) < 4 OR LEN(@EmailId) > 40 OR (@EmailId IS NULL))  
            RETURN -2 

        --IF ( LEN(@Password) < 8 OR LEN(@Password)>20 OR (@Password IS NULL))  
        --    RETURN -3 

        IF (NOT(LEN(@Password) BETWEEN 8 AND 16) OR (@Password IS NULL))  
            RETURN -3  
            
        IF ( LEN(@DisplayName) < 2 OR LEN(@DisplayName)>50 OR (@DisplayName IS NULL))  
            RETURN -4  
  
        IF ( [dbo].ufn_CalculateAge(@DateOfBirth) < 13 OR @DateOfBirth IS NULL)   
            RETURN -5
    
        
        BEGIN
            INSERT INTO [User] (EmailId, [Password], RoleId, DisplayName, DateOfBirth)
            VALUES (@EmailId, @Password, @RoleId, @DisplayName, @DateOfBirth);
            SELECT @UserId = UserId FROM [User] WHERE EmailId =@EmailId
			INSERT INTO [UserProfile](UserId, Avatar) VALUES (@UserId,'Anonymous/anonymous.png')
            
            RETURN 1  
        END
     END TRY
    BEGIN CATCH
        RETURN -99
    END CATCH
END
GO

CREATE FUNCTION [dbo].ufn_GetActiveUserCount()
RETURNS INT
AS
BEGIN
    DECLARE @ActiveUserCount INT;
    SELECT @ActiveUserCount = COUNT(*)
    FROM dbo.[User] AS u
    JOIN UserProfile AS up ON u.UserId = up.UserId
    WHERE up.IsActive = 'Online' AND u.RoleId = 1;
    RETURN @ActiveUserCount;
END
GO

-- Insert Roles
INSERT INTO [Role] (RoleName)
VALUES ('User'), ('Admin');

--Insert Admin
INSERT INTO [User] (EmailId, [Password], RoleId, DisplayName, DateOfBirth)
VALUES ('admin@example.com', 'adminpassword', 2,'Admin','2023-01-01');
INSERT INTO [UserProfile] (UserId, Avatar, Theme) VALUES (1000,'Animals/cat.png', 'Dark');

-- Insert Users
INSERT INTO [User] (EmailId, [Password], RoleId, DisplayName, DateOfBirth)
VALUES ('john.doe@example.com', 'password123', 1, 'John Doe', '1990-01-01'),
	   ('jane.doe@example.com', 'password456', 1, 'Jane Doe', '1995-01-01'),
	   ('adam.smith@example.com', 'password789', 1, 'Adam Smith', '1999-01-01'),
	   ('jimmy.neutraon@example.com', 'password123', 1, 'Jimmy Neutron', '2005-01-01'),
	   ('cathie.neutraon@example.com', 'password123', 1, 'Cathie Neutron', '2005-01-01'),
	   ('max.neutraon@example.com', 'password123', 1, 'Max Neutron', '2005-01-01');
GO


-- Insert Sample Values in User Profile
INSERT INTO [UserProfile] (UserId, Avatar) 
VALUES  (1001, 'Men/man (3).png'),
        (1002, 'Men/man (2).png'),
        (1003, 'Animals/dog.png'),
        (1004, 'Animals/dog.png'),
        (1005, 'Women/woman (2).png'),
        (1006, 'Women/woman (2).png');
GO

-- Insert Sample Values in FriendList
INSERT INTO FriendList (UserId, FriendId) VALUES (1001, 1002);
INSERT INTO FriendList (UserId, FriendId) VALUES (1002, 1001);
INSERT INTO FriendList (UserId, FriendId) VALUES (1001, 1003);
GO

-- Insert Sample Values in Chat
INSERT INTO Chat (InitiatorId, RecipientId) VALUES (1001, 1002);
INSERT INTO Chat (InitiatorId, RecipientId) VALUES (1001, 1003);
GO

-- Insert Sample Values in Message
INSERT INTO [Message] (ChatId, SenderId, ReceiverId, Content, SentTime, IsRead) VALUES (10, 1001, 1002, 'Hello', '2023-07-01 10:00:00', 'Sent');
INSERT INTO [Message] (ChatId, SenderId, ReceiverId, Content, SentTime, IsRead) VALUES (10, 1002, 1001, 'Hi', '2023-07-02 10:01:00', 'Delivered');
INSERT INTO [Message] (ChatId, SenderId, ReceiverId, Content, SentTime, IsRead) VALUES (10, 1001, 1002, 'How are you?', '2023-07-03 10:02:00', 'Sent');
INSERT INTO [Message] (ChatId, SenderId, ReceiverId, Content, SentTime, IsRead) VALUES (10, 1002, 1001, 'I am fine.', '2023-07-04 05:03:00', 'Delivered');
GO

-- Insert Sample Values in StarredMessage
INSERT INTO StarredMessage (MessageId, UserId) VALUES (5000, 1001);
INSERT INTO StarredMessage (MessageId, UserId) VALUES (5001, 1001);
GO

-- Insert Sample Values in Feedback
INSERT INTO Feedback (UserId, UserFeedback, Rating, PostedAt, AdminReply, AdminReplyTime) VALUES (1001, 'I really like this application', 5, '2023-07-01 10:00:00', 'We are glad to hear that.', '2023-07-02 10:00:00');
INSERT INTO Feedback (UserId, UserFeedback, Rating, PostedAt, AdminReply, AdminReplyTime) VALUES (1002, 'This is a fun place to spend time', 4, '2023-07-01 10:00:00', 'Hope you are having fun.', '2023-07-02 10:00:00');
INSERT INTO Feedback (UserId, UserFeedback, Rating, PostedAt, AdminReply, AdminReplyTime) VALUES (1003, 'I cannot wait to get more updates', 3, '2023-07-01 10:00:00', 'Fingers Crossed.', '2023-07-02 10:00:00');
INSERT INTO Feedback (UserId, UserFeedback, Rating, PostedAt, AdminReply, AdminReplyTime) VALUES (1006, 'Lets chat together', 4, '2023-07-01 10:00:00', NULL, NULL);


GO


--Show All Tables in different select clauses
SELECT * FROM [Role];
SELECT * FROM [User];
SELECT * FROM UserProfile;
SELECT * FROM FriendList;
SELECT * FROM Chat;
SELECT * FROM [Message];
SELECT * FROM StarredMessage;
SELECT * FROM Feedback;
GO

SELECT dbo.ufn_GetActiveUserCount()

-- Scaffold-DbContext -Connection "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GigaChatDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir "Models" -Force