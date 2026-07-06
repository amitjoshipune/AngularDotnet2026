/*
  BookMyShoppingBuddy — tables (no EF Core; script-first for SQL Server)
*/
USE ShoppingBuddy;
GO

IF OBJECT_ID(N'dbo.Bookings', N'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID(N'dbo.Buddies', N'U') IS NOT NULL DROP TABLE dbo.Buddies;
IF OBJECT_ID(N'dbo.Venues', N'U') IS NOT NULL DROP TABLE dbo.Venues;
IF OBJECT_ID(N'dbo.Localities', N'U') IS NOT NULL DROP TABLE dbo.Localities;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users
(
    UserId       INT            IDENTITY(1, 1) NOT NULL,
    Email        NVARCHAR(256)  NOT NULL,
    Password     NVARCHAR(128)  NOT NULL,
    DisplayName  NVARCHAR(100)  NOT NULL,
    Role         NVARCHAR(20)   NOT NULL,
    IsActive     BIT            NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt    DATETIME2(0)   NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Role CHECK (Role IN (N'Customer', N'Buddy', N'Admin'))
);

CREATE TABLE dbo.Localities
(
    LocalityId   NVARCHAR(50)   NOT NULL,
    Name         NVARCHAR(100)  NOT NULL,
    Zone         NVARCHAR(50)   NOT NULL,
    CONSTRAINT PK_Localities PRIMARY KEY CLUSTERED (LocalityId)
);

CREATE TABLE dbo.Venues
(
    VenueId      NVARCHAR(50)   NOT NULL,
    Name         NVARCHAR(200)  NOT NULL,
    LocalityId   NVARCHAR(50)   NOT NULL,
    VenueType    NVARCHAR(50)   NOT NULL,
    CONSTRAINT PK_Venues PRIMARY KEY CLUSTERED (VenueId),
    CONSTRAINT FK_Venues_Localities FOREIGN KEY (LocalityId) REFERENCES dbo.Localities (LocalityId)
);

CREATE TABLE dbo.Buddies
(
    BuddyId             NVARCHAR(50)   NOT NULL,
    UserId              INT            NULL,
    DisplayName         NVARCHAR(100)  NOT NULL,
    Age                 INT            NOT NULL,
    LocalityId          NVARCHAR(50)   NOT NULL,
    Languages           NVARCHAR(200)  NOT NULL,
    VerificationLevel   NVARCHAR(30)   NOT NULL,
    Rating              DECIMAL(3, 1)  NOT NULL,
    CompletedTrips      INT            NOT NULL,
    Bio                 NVARCHAR(MAX)  NOT NULL,
    ActivityTypes       NVARCHAR(200)  NOT NULL,
    AvailableToday      BIT            NOT NULL CONSTRAINT DF_Buddies_AvailableToday DEFAULT (1),
    PreferredVenueIds   NVARCHAR(500)  NOT NULL,
    Gender              NVARCHAR(20)   NOT NULL CONSTRAINT DF_Buddies_Gender DEFAULT (N'Female'),
    AvatarUrl           NVARCHAR(500)  NULL,
    CONSTRAINT PK_Buddies PRIMARY KEY CLUSTERED (BuddyId),
    CONSTRAINT FK_Buddies_Localities FOREIGN KEY (LocalityId) REFERENCES dbo.Localities (LocalityId),
    CONSTRAINT FK_Buddies_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
);

CREATE TABLE dbo.Bookings
(
    BookingId           NVARCHAR(20)   NOT NULL,
    CustomerUserId      INT            NOT NULL,
    BuddyId             NVARCHAR(50)   NOT NULL,
    VenueId             NVARCHAR(50)   NOT NULL,
    ActivityType        NVARCHAR(50)   NOT NULL,
    BookingDate         DATE           NOT NULL,
    TimeSlot            NVARCHAR(50)   NOT NULL,
    SpecialNotes        NVARCHAR(500)  NULL,
    SafetyPin           CHAR(4)        NOT NULL,
    ShareLiveLocation   BIT            NOT NULL CONSTRAINT DF_Bookings_ShareLiveLocation DEFAULT (0),
    Status              NVARCHAR(20)   NOT NULL CONSTRAINT DF_Bookings_Status DEFAULT (N'Confirmed'),
    CreatedAt           DATETIME2(0)   NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Bookings PRIMARY KEY CLUSTERED (BookingId),
    CONSTRAINT FK_Bookings_Customer FOREIGN KEY (CustomerUserId) REFERENCES dbo.Users (UserId),
    CONSTRAINT FK_Bookings_Buddy FOREIGN KEY (BuddyId) REFERENCES dbo.Buddies (BuddyId),
    CONSTRAINT FK_Bookings_Venue FOREIGN KEY (VenueId) REFERENCES dbo.Venues (VenueId),
    CONSTRAINT CK_Bookings_Status CHECK (Status IN (N'Confirmed', N'Completed', N'Cancelled'))
);

CREATE INDEX IX_Buddies_LocalityId ON dbo.Buddies (LocalityId);
CREATE INDEX IX_Bookings_CustomerUserId ON dbo.Bookings (CustomerUserId);
CREATE INDEX IX_Bookings_BuddyId ON dbo.Bookings (BuddyId);
GO
