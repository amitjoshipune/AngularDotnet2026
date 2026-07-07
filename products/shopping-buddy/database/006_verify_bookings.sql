/*
  Verify how Customers, Buddies, and Bookings connect.
  Run in SSMS on ShoppingBuddy database.
*/
USE ShoppingBuddy;
GO

PRINT '=== Users (login accounts) ===';
SELECT UserId, Email, Role, DisplayName FROM dbo.Users ORDER BY UserId;
GO

PRINT '=== Buddies (shopping companions — may link to a Buddy user) ===';
SELECT
    b.BuddyId,
    b.DisplayName AS BuddyName,
    b.UserId AS BuddyLoginUserId,
    u.Email AS BuddyLoginEmail
FROM dbo.Buddies b
LEFT JOIN dbo.Users u ON u.UserId = b.UserId
ORDER BY b.DisplayName;
GO

PRINT '=== Bookings (CustomerUserId = who booked; BuddyId = who they booked) ===';
SELECT
    bk.BookingId,
    bk.BookingDate,
    bk.TimeSlot,
    bk.Status,
    c.UserId AS CustomerUserId,
    c.Email AS CustomerEmail,
    buddy.DisplayName AS BuddyName,
    v.Name AS VenueName
FROM dbo.Bookings bk
INNER JOIN dbo.Users c ON c.UserId = bk.CustomerUserId
INNER JOIN dbo.Buddies buddy ON buddy.BuddyId = bk.BuddyId
INNER JOIN dbo.Venues v ON v.VenueId = bk.VenueId
ORDER BY bk.CreatedAt DESC;
GO

PRINT 'Demo: customer@demo.com is UserId 1 — should have 2 seed bookings (BMSB-100001, BMSB-100003).';
GO
