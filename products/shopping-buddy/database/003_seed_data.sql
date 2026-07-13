/*
  BookMyShoppingBuddy — demo seed data (passwords are plain text for local MVP only)
*/
USE ShoppingBuddy;
GO

SET NOCOUNT ON;

DELETE FROM dbo.Bookings;
DELETE FROM dbo.Buddies;
DELETE FROM dbo.Venues;
DELETE FROM dbo.Localities;
DELETE FROM dbo.Users;
GO

SET IDENTITY_INSERT dbo.Users ON;

INSERT INTO dbo.Users (UserId, Email, Password, DisplayName, Role, EmailVerified, EmailVerifiedAt)
VALUES
    (1, N'customer@demo.com', N'Customer@123', N'Sunita Patil', N'Customer', 1, SYSUTCDATETIME()),
    (2, N'senior@demo.com', N'Senior@123', N'Kamla Deshmukh', N'Customer', 1, SYSUTCDATETIME()),
    (3, N'meera@demo.com', N'Buddy@123', N'Meera K.', N'Buddy', 1, SYSUTCDATETIME()),
    (4, N'anjali@demo.com', N'Buddy@123', N'Anjali S.', N'Buddy', 1, SYSUTCDATETIME()),
    (5, N'admin@example.com', N'Admin@123', N'Admin User', N'Admin', 1, SYSUTCDATETIME());

SET IDENTITY_INSERT dbo.Users OFF;

INSERT INTO dbo.Localities (LocalityId, Name, Zone)
VALUES
    (N'wakad', N'Wakad', N'PCMC'),
    (N'hinjawadi', N'Hinjawadi', N'PCMC'),
    (N'aundh', N'Aundh', N'Pune City'),
    (N'baner', N'Baner', N'Pune City'),
    (N'kothrud', N'Kothrud', N'Pune City'),
    (N'hadapsar', N'Hadapsar', N'Pune Metro'),
    (N'vimannagar', N'Viman Nagar', N'Pune Metro'),
    (N'chinchwad', N'Chinchwad', N'PCMC'),
    (N'pimpri', N'Pimpri', N'PCMC'),
    (N'kharadi', N'Kharadi', N'Pune Metro');

INSERT INTO dbo.Venues (VenueId, Name, LocalityId, VenueType)
VALUES
    (N'phoenix-wakad', N'Phoenix Marketcity (Wakad)', N'wakad', N'Mall'),
    (N'elpro-chinchwad', N'Elpro City Square (Chinchwad)', N'chinchwad', N'Mall'),
    (N'seasons-aundh', N'Seasons Mall (Aundh)', N'aundh', N'Mall'),
    (N'amanora-hadapsar', N'Amanora Mall (Hadapsar)', N'hadapsar', N'Mall'),
    (N'koregaon-park', N'Koregaon Park Plaza', N'vimannagar', N'Mall'),
    (N'dmart-baner', N'DMart (Baner)', N'baner', N'Supermarket'),
    (N'laxmi-road', N'Laxmi Road Market', N'kothrud', N'Market'),
    (N'kharadi-retail', N'Kharadi High Street', N'kharadi', N'Market');

INSERT INTO dbo.Buddies
(
    BuddyId, UserId, DisplayName, Age, LocalityId, Languages, VerificationLevel,
    Rating, CompletedTrips, Bio, ActivityTypes, AvailableToday, PreferredVenueIds, Gender, AvatarUrl
)
VALUES
(
    N'buddy-1', 3, N'Meera K.', 58, N'wakad', N'Marathi,Hindi,English', N'PoliceVerified',
    4.9, 127,
    N'Retired bank officer. Comfortable with mall shopping, billing queues, and heavy bags.',
    N'Shopping,MallVisit,GroceryRun', 1, N'phoenix-wakad,dmart-baner', N'Female',
    N'https://ui-avatars.com/api/?name=Meera+K&background=2E7D6F&color=fff&size=128'
),
(
    N'buddy-2', 4, N'Anjali S.', 42, N'aundh', N'Marathi,English', N'AadhaarVerified',
    4.8, 89,
    N'Homemaker and part-time tutor. Patient with seniors and first-time app users.',
    N'Shopping,MarketVisit', 1, N'seasons-aundh,laxmi-road', N'Female',
    N'https://ui-avatars.com/api/?name=Anjali+S&background=2E7D6F&color=fff&size=128'
),
(
    N'buddy-3', NULL, N'Priya D.', 35, N'hadapsar', N'Hindi,Marathi,English', N'AadhaarVerified',
    4.7, 64,
    N'IT professional on weekends. Helps with electronics, fashion, and grocery runs.',
    N'MallVisit,GroceryRun', 0, N'amanora-hadapsar', N'Female',
    N'https://ui-avatars.com/api/?name=Priya+D&background=2E7D6F&color=fff&size=128'
),
(
    N'buddy-4', NULL, N'Sunita R.', 61, N'chinchwad', N'Marathi,Hindi', N'PoliceVerified',
    5.0, 203,
    N'Former nurse. Calm companion for market visits and long shopping days.',
    N'MarketVisit,Shopping', 1, N'elpro-chinchwad,laxmi-road', N'Female',
    N'https://ui-avatars.com/api/?name=Sunita+R&background=2E7D6F&color=fff&size=128'
),
(
    N'buddy-5', NULL, N'Kavita M.', 48, N'kharadi', N'English,Hindi,Marathi', N'AadhaarVerified',
    4.6, 41,
    N'Single professional. Weekday evening slots for quick grocery and pharmacy runs.',
    N'GroceryRun,Shopping', 1, N'kharadi-retail', N'Female',
    N'https://ui-avatars.com/api/?name=Kavita+M&background=2E7D6F&color=fff&size=128'
);

DECLARE @today DATE = CAST(GETDATE() AS DATE);

INSERT INTO dbo.Bookings
(
    BookingId, CustomerUserId, BuddyId, VenueId, ActivityType, BookingDate,
    TimeSlot, SpecialNotes, SafetyPin, ShareLiveLocation, Status
)
VALUES
(
    N'BMSB-100001', 1, N'buddy-1', N'phoenix-wakad', N'Shopping', DATEADD(DAY, 2, @today),
    N'10:00 AM – 12:00 PM', N'Need help with saree shopping.', N'4821', 1, N'Confirmed'
),
(
    N'BMSB-100002', 2, N'buddy-2', N'seasons-aundh', N'MallVisit', DATEADD(DAY, 5, @today),
    N'3:00 PM – 5:00 PM', NULL, N'7390', 1, N'Confirmed'
),
(
    N'BMSB-100003', 1, N'buddy-3', N'amanora-hadapsar', N'GroceryRun', DATEADD(DAY, -1, @today),
    N'5:00 PM – 7:00 PM', N'Weekly grocery run.', N'1156', 0, N'Completed'
);

PRINT 'Seed complete: 5 users, 10 localities, 8 venues, 5 buddies, 3 bookings.';
GO
