-- پاک کردن کامل هر چه در دیتابیس است
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;
-- جدول Students
-- نکته: دابل‌کوت‌ها عمداً هستن تا حروف بزرگ/کوچک حفظ بشن
-- و با خروجی PostgresCompiler کتابخانه SqlKata یکسان بشن
CREATE TABLE "Students" (
    "StudentNumber" INT PRIMARY KEY,
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "Grade" INT NOT NULL,
    "IsMale" BOOLEAN NOT NULL
);
-- داده‌های نمونه (کاملاً یکسان با SQL Server)
INSERT INTO "Students" (
        "StudentNumber",
        "FirstName",
        "LastName",
        "Grade",
        "IsMale"
    )
VALUES (101, 'Ali', 'Rezaei', 18, TRUE),
    (102, 'Sara', 'Ahmadi', 17, FALSE),
    (103, 'Reza', 'Karimi', 19, TRUE),
    (104, 'Maryam', 'Mousavi', 18, FALSE),
    (105, 'Hossein', 'Taheri', 17, TRUE);