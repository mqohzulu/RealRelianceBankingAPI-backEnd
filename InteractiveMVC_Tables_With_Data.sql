
CREATE TABLE Person (
    PersonId UNIQUEIDENTIFIER PRIMARY KEY,
    IdNumber INT,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(15),
    Address NVARCHAR(255),
    DateOfBirth DATE,
    ActiveInd BIT NOT NULL,
);

CREATE TABLE Account (
    AccountId UNIQUEIDENTIFIER PRIMARY KEY,
    PersonId UNIQUEIDENTIFIER NOT NULL,
    AccountNumber NVARCHAR(20) NOT NULL UNIQUE,
	AccountType NVARCHAR(50),
    Balance DECIMAL(18, 2) NOT NULL,
    IsClosed BIT NOT NULL,
    ActiveInd BIT NOT NULL,
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId)
);

CREATE TABLE Transactions (
    TransactionId UNIQUEIDENTIFIER PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    TransactionType VARCHAR(10) NOT NULL CHECK (TransactionType IN ('Debit', 'Credit')),
    TransactionDate DATETIME NOT NULL,
    Description NVARCHAR(255),
    ActiveInd BIT NOT NULL,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

	CREATE TABLE Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
	password NVARCHAR(100) NOT NULL,
	Role NVARCHAR(50) NOT NULL,
    ActiveInd BIT NOT NULL
);

-- Insert test data into the Person table


INSERT INTO Person (PersonId, IdNumber, FirstName, LastName, Email, PhoneNumber, Address, DateOfBirth, ActiveInd)
VALUES
    ('C399843A-B0F9-4432-A70E-2BAECCE7619F', 123456, 'John', 'Doe', 'john.doe@example.com', '555-1234', '123 Elm Street', '1980-01-01', 1),
    ( '5E516D14-1732-40BB-AC52-4294E7E685A6', 234567, 'Jane', 'Smith', 'jane.smith@example.com', '555-5678', '456 Oak Avenue', '1990-02-02', 1),
    ('23C43FE1-0B33-4530-B045-47B4CFEF230D', 345678, 'Bob', 'Johnson', 'bob.johnson@example.com', '555-9876', '789 Pine Road', '1975-03-03', 0);


	-- Insert test data into the Account table
INSERT INTO Account (AccountId, PersonId, AccountNumber, AccountType, Balance, IsClosed, ActiveInd)
VALUES
    ('D0300D5A-52FE-4B02-8844-78E9CC69769A', 'C399843A-B0F9-4432-A70E-2BAECCE7619F', 'ACC1234567890', 'Checking', 1000.00, 0, 1),
    ('D9771B65-BCE2-47C7-B611-A0C30FEDCF68', '5E516D14-1732-40BB-AC52-4294E7E685A6', 'ACC2345678901', 'Savings', 2500.50, 0, 1),
    ('C11B7553-085C-4126-A128-F083E887A087', '23C43FE1-0B33-4530-B045-47B4CFEF230D', 'ACC3456789012', 'Checking', 500.75, 1, 0);

	-- Insert test data into the Transactions table
INSERT INTO Transactions (TransactionId, AccountId, Amount, TransactionType, TransactionDate, Description, ActiveInd)
VALUES
    (NEWID(), 'D0300D5A-52FE-4B02-8844-78E9CC69769A', 100.00, 'Credit', '2024-07-01', 'Initial deposit', 1),
    (NEWID(), 'D9771B65-BCE2-47C7-B611-A0C30FEDCF68', 50.75, 'Debit', '2024-07-02', 'Grocery shopping', 1),
    (NEWID(), 'C11B7553-085C-4126-A128-F083E887A087', 200.00, 'Credit', '2024-07-03', 'Salary', 1);

-- Insert test data into Users Table
	INSERT INTO Users (UserId, FirstName, LastName, Email, password,Role, ActiveInd)
VALUES
    (NEWID(), 'Bob', 'Builder', 'bob.builder@example.com', 'pass1','Admin', 1),
	('5E516D14-1732-40BB-AC52-4294E7E685A6', 'Jane', 'Smith', 'jane.smith@example.com', 'JaneS','Customer', 1);