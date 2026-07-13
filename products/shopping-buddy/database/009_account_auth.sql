/*
  BookMyShoppingBuddy — account auth: email verification + OTP challenges
  Run after 001–004 on existing databases, or included in run-all.bat for fresh installs.
*/
USE ShoppingBuddy;
GO

SET NOCOUNT ON;

-- Extend Users for email verification (idempotent for re-runs)
IF COL_LENGTH(N'dbo.Users', N'EmailVerified') IS NULL
BEGIN
    ALTER TABLE dbo.Users
    ADD EmailVerified BIT NOT NULL
        CONSTRAINT DF_Users_EmailVerified DEFAULT (0);
END;
GO

IF COL_LENGTH(N'dbo.Users', N'EmailVerifiedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users
    ADD EmailVerifiedAt DATETIME2(0) NULL;
END;
GO

-- BCrypt hashes need a little more room than plain-text demo passwords
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users')
      AND name = N'Password'
      AND max_length < 512
)
BEGIN
    ALTER TABLE dbo.Users ALTER COLUMN Password NVARCHAR(256) NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.OtpChallenges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OtpChallenges
    (
        OtpChallengeId INT            IDENTITY(1, 1) NOT NULL,
        Email          NVARCHAR(256)  NOT NULL,
        Code           NVARCHAR(10)   NOT NULL,
        Purpose        NVARCHAR(30)   NOT NULL,
        ExpiresAt      DATETIME2(0)   NOT NULL,
        UsedAt         DATETIME2(0)   NULL,
        CreatedAt      DATETIME2(0)   NOT NULL CONSTRAINT DF_OtpChallenges_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_OtpChallenges PRIMARY KEY CLUSTERED (OtpChallengeId),
        CONSTRAINT CK_OtpChallenges_Purpose CHECK (
            Purpose IN (N'EmailVerification', N'PasswordReset', N'ForgotLoginId')
        )
    );

    CREATE NONCLUSTERED INDEX IX_OtpChallenges_Email_Purpose_CreatedAt
        ON dbo.OtpChallenges (Email, Purpose, CreatedAt DESC);
END;
GO

-- Demo / seed users stay verified so existing logins keep working
UPDATE dbo.Users
SET EmailVerified = 1,
    EmailVerifiedAt = COALESCE(EmailVerifiedAt, SYSUTCDATETIME())
WHERE EmailVerified = 0
  AND Email IN (
      N'customer@demo.com',
      N'senior@demo.com',
      N'meera@demo.com',
      N'anjali@demo.com',
      N'admin@example.com'
  );
GO

PRINT '009_account_auth complete: EmailVerified columns + OtpChallenges table.';
GO
