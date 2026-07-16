USE ShoppingBuddy;
-- Who can log into the app?
SELECT UserId, Email, Role, DisplayName , * FROM dbo.Users;
-- Buddies from your DB (not mock)
SELECT BuddyId, DisplayName, Rating, LocalityId FROM dbo.Buddies;
-- All bookings
SELECT BookingId, BookingDate, TimeSlot, Status FROM dbo.Bookings;