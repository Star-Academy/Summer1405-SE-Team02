USE master;
GO IF DB_ID('mohaymen') IS NOT NULL BEGIN ALTER DATABASE mohaymen
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE mohaymen;
END
GO CREATE DATABASE mohaymen;
GO USE mohaymen;
GO CREATE TABLE dbo.Students (
        StudentNumber INT PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Grade INT NOT NULL,
        IsMale BIT NOT NULL
    );
GO
INSERT INTO dbo.Students (
        StudentNumber,
        FirstName,
        LastName,
        Grade,
        IsMale
    )
VALUES (101, N'Ali', N'Rezaei', 18, 1),
    (102, N'Sara', N'Ahmadi', 17, 0),
    (103, N'Reza', N'Karimi', 19, 1),
    (104, N'Maryam', N'Mousavi', 18, 0),
    (105, N'Hossein', N'Taheri', 17, 1);
GO