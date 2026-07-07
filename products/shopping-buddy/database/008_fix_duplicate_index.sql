/*
  Fix duplicate-booking index if 007/008 failed.
  Filtered indexes require QUOTED_IDENTIFIER ON (sqlcmd: use -I or run this script as-is).
  Safe to re-run.
*/
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

USE ShoppingBuddy;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Bookings_CustomerSlot_Active')
BEGIN
    DROP INDEX UQ_Bookings_CustomerSlot_Active ON dbo.Bookings;
END
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Bookings_CustomerSlot_Active
    ON dbo.Bookings (CustomerUserId, VenueId, BookingDate, TimeSlot)
    WHERE Status IN (N'PendingBuddy', N'Confirmed', N'Completed');
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Bookings_CustomerSlot_Active')
    PRINT '008 complete: duplicate-booking index created.';
ELSE
    PRINT '008 FAILED: index UQ_Bookings_CustomerSlot_Active was not created.';
GO
