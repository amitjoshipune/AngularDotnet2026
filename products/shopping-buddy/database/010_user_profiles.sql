/*
  BookMyShoppingBuddy — user profiles and addresses (PR pair 2)
  Safe to re-run on existing ShoppingBuddy databases.
*/
USE ShoppingBuddy;
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.UserAddresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserAddresses
    (
        AddressId   INT            IDENTITY(1, 1) NOT NULL,
        UserId      INT            NOT NULL,
        Label       NVARCHAR(50)   NOT NULL,
        Line1       NVARCHAR(200)  NOT NULL,
        Line2       NVARCHAR(200)  NULL,
        City        NVARCHAR(100)  NOT NULL,
        State       NVARCHAR(100)  NOT NULL,
        Pincode     NVARCHAR(10)   NOT NULL,
        IsDefault   BIT            NOT NULL CONSTRAINT DF_UserAddresses_IsDefault DEFAULT (0),
        CreatedAt   DATETIME2(0)   NOT NULL CONSTRAINT DF_UserAddresses_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UserAddresses PRIMARY KEY CLUSTERED (AddressId),
        CONSTRAINT FK_UserAddresses_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
    );

    CREATE NONCLUSTERED INDEX IX_UserAddresses_UserId ON dbo.UserAddresses (UserId);
END;
GO

IF OBJECT_ID(N'dbo.UserProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserProfiles
    (
        UserId                   INT            NOT NULL,
        Phone                    NVARCHAR(20)   NULL,
        DateOfBirth              DATE           NULL,
        Gender                   NVARCHAR(20)   NULL,
        EmergencyContactName     NVARCHAR(100)  NULL,
        EmergencyContactPhone    NVARCHAR(20)   NULL,
        Bio                      NVARCHAR(500)  NULL,
        BuddyApplicationStatus   NVARCHAR(20)   NOT NULL CONSTRAINT DF_UserProfiles_BuddyApplicationStatus DEFAULT (N'None'),
        BuddyApplicationNotes    NVARCHAR(500)  NULL,
        BuddyLocalityId          NVARCHAR(50)   NULL,
        UpdatedAt                DATETIME2(0)   NOT NULL CONSTRAINT DF_UserProfiles_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UserProfiles PRIMARY KEY CLUSTERED (UserId),
        CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId),
        CONSTRAINT CK_UserProfiles_BuddyApplicationStatus CHECK (
            BuddyApplicationStatus IN (N'None', N'Pending', N'Approved', N'Rejected')
        )
    );
END;
GO

PRINT '010_user_profiles complete: UserProfiles + UserAddresses.';
GO
