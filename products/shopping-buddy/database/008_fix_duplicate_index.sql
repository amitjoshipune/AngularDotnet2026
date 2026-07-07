/*
  Fix duplicate-booking index if 007 failed with "Incorrect syntax near 'NOT'".
  Safe to re-run.
*/
USE ShoppingBuddy;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Bookings_CustomerSlot_Active')
BEGIN
    DROP INDEX UQ_Bookings_CustomerSlot_Active ON dbo.Bookings;
END
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Bookings_CustomerSlot_Active
    ON dbo.Bookings (CustomerUserId, VenueId, BookingDate, TimeSlot)
    WHERE Status IN (N'PendingBuddy', N'Confirmed', N'Completed');
GO

PRINT '008 complete: duplicate-booking index created.';
GO
