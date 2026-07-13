/*
  BookMyShoppingBuddy — identity documents + booking cancel support (PR pair 3)
  Safe to re-run on existing ShoppingBuddy databases.
*/
USE ShoppingBuddy;
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.UserDocuments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserDocuments
    (
        DocumentId     INT            IDENTITY(1, 1) NOT NULL,
        UserId         INT            NOT NULL,
        DocumentType   NVARCHAR(30)   NOT NULL,
        FileName       NVARCHAR(256)  NOT NULL,
        StoragePath    NVARCHAR(500)  NOT NULL,
        Status         NVARCHAR(20)   NOT NULL CONSTRAINT DF_UserDocuments_Status DEFAULT (N'Pending'),
        UploadedAt     DATETIME2(0)   NOT NULL CONSTRAINT DF_UserDocuments_UploadedAt DEFAULT (SYSUTCDATETIME()),
        VerifiedAt     DATETIME2(0)   NULL,
        RejectionReason NVARCHAR(500) NULL,
        CONSTRAINT PK_UserDocuments PRIMARY KEY CLUSTERED (DocumentId),
        CONSTRAINT FK_UserDocuments_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId),
        CONSTRAINT CK_UserDocuments_DocumentType CHECK (
            DocumentType IN (N'Aadhaar', N'AddressProof', N'PAN')
        ),
        CONSTRAINT CK_UserDocuments_Status CHECK (
            Status IN (N'Pending', N'Verified', N'Rejected')
        )
    );

    CREATE NONCLUSTERED INDEX IX_UserDocuments_UserId_Type
        ON dbo.UserDocuments (UserId, DocumentType, UploadedAt DESC);
END;
GO

IF COL_LENGTH(N'dbo.Bookings', N'CancelledAt') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings ADD CancelledAt DATETIME2(0) NULL;
END;
GO

-- Demo buddies: pre-verified ID docs so confirm flow works out of the box
IF NOT EXISTS (SELECT 1 FROM dbo.UserDocuments WHERE UserId = 3 AND DocumentType = N'Aadhaar')
BEGIN
    INSERT INTO dbo.UserDocuments (UserId, DocumentType, FileName, StoragePath, Status, VerifiedAt)
    VALUES
        (3, N'Aadhaar', N'meera-aadhaar.pdf', N'seed/meera-aadhaar.pdf', N'Verified', SYSUTCDATETIME()),
        (3, N'AddressProof', N'meera-address.pdf', N'seed/meera-address.pdf', N'Verified', SYSUTCDATETIME()),
        (4, N'Aadhaar', N'anjali-aadhaar.pdf', N'seed/anjali-aadhaar.pdf', N'Verified', SYSUTCDATETIME()),
        (4, N'AddressProof', N'anjali-address.pdf', N'seed/anjali-address.pdf', N'Verified', SYSUTCDATETIME());
END;
GO

PRINT '011_identity_and_cancel complete: UserDocuments + CancelledAt.';
GO
