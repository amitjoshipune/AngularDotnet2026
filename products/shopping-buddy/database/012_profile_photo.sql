/*
  BookMyShoppingBuddy — optional profile photo URL on UserProfiles
*/
USE ShoppingBuddy;
GO

IF COL_LENGTH(N'dbo.UserProfiles', N'ProfilePhotoUrl') IS NULL
BEGIN
    ALTER TABLE dbo.UserProfiles ADD ProfilePhotoUrl NVARCHAR(500) NULL;
END;
GO

PRINT '012_profile_photo complete.';
GO
