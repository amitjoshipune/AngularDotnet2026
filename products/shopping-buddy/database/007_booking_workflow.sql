/*
  BookMyShoppingBuddy — booking workflow + multi-role users (run on existing ShoppingBuddy DB)
  - UserRoles: Customer + Buddy can coexist (buddy can also book trips)
  - Booking status: PendingBuddy → Confirmed | RejectedByBuddy
  - No duplicate: same customer + venue + date + time slot (active bookings)
*/
USE ShoppingBuddy;
GO

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles
    (
        UserId   INT           NOT NULL,
        Role     NVARCHAR(20)  NOT NULL,
        CONSTRAINT PK_UserRoles PRIMARY KEY CLUSTERED (UserId, Role),
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId),
        CONSTRAINT CK_UserRoles_Role CHECK (Role IN (N'Customer', N'Buddy', N'Admin'))
    );
END
GO

INSERT INTO dbo.UserRoles (UserId, Role)
SELECT u.UserId, u.Role
FROM dbo.Users u
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur WHERE ur.UserId = u.UserId AND ur.Role = u.Role
);
GO

-- Buddies can also act as customers (book other buddies).
INSERT INTO dbo.UserRoles (UserId, Role)
SELECT u.UserId, N'Customer'
FROM dbo.Users u
WHERE u.Role = N'Buddy'
  AND NOT EXISTS (
      SELECT 1 FROM dbo.UserRoles ur WHERE ur.UserId = u.UserId AND ur.Role = N'Customer'
  );
GO

IF COL_LENGTH(N'dbo.Bookings', N'RejectionReasonCode') IS NULL
    ALTER TABLE dbo.Bookings ADD RejectionReasonCode NVARCHAR(50) NULL;
GO

IF COL_LENGTH(N'dbo.Bookings', N'RejectionReasonText') IS NULL
    ALTER TABLE dbo.Bookings ADD RejectionReasonText NVARCHAR(500) NULL;
GO

IF COL_LENGTH(N'dbo.Bookings', N'BuddyRespondedAt') IS NULL
    ALTER TABLE dbo.Bookings ADD BuddyRespondedAt DATETIME2(0) NULL;
GO

IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Bookings_Status')
    ALTER TABLE dbo.Bookings DROP CONSTRAINT CK_Bookings_Status;
GO

ALTER TABLE dbo.Bookings ADD CONSTRAINT CK_Bookings_Status
    CHECK (Status IN (N'PendingBuddy', N'Confirmed', N'RejectedByBuddy', N'Completed', N'Cancelled'));
GO

UPDATE dbo.Bookings SET Status = N'Confirmed' WHERE Status = N'Confirmed' OR Status NOT IN (
    N'PendingBuddy', N'Confirmed', N'RejectedByBuddy', N'Completed', N'Cancelled'
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Bookings_CustomerSlot_Active')
BEGIN
    CREATE UNIQUE INDEX UQ_Bookings_CustomerSlot_Active
        ON dbo.Bookings (CustomerUserId, VenueId, BookingDate, TimeSlot)
        WHERE Status NOT IN (N'Cancelled', N'RejectedByBuddy');
END
GO

PRINT '007 complete: UserRoles, PendingBuddy workflow, duplicate-booking index.';
GO
