using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Database
{
    public sealed class TestDatabaseFixture : IAsyncLifetime
    {
        private const string DatabaseName = "RealRelianceManagementDB_Test";
        private static readonly string MasterConnectionString =
            "Server=(localdb)\\AccountManagerDB;Database=master;Integrated Security=true;TrustServerCertificate=True;";

        public string ConnectionString { get; } =
            $"Server=(localdb)\\AccountManagerDB;Database={DatabaseName};Integrated Security=true;TrustServerCertificate=True;";

        public async Task InitializeAsync()
        {
            await EnsureDatabaseAsync();
            await EnsureSchemaAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task ResetAsync()
        {
            const string sql = @"
DELETE FROM Users;
DELETE FROM Transactions;
DELETE FROM Account;
DELETE FROM Person;";

            await ExecuteAsync(sql);
        }

        public async Task<Guid> InsertPersonAsync(
            int idNumber,
            string firstName,
            string lastName,
            string email,
            bool activeInd = true,
            string? phoneNumber = null,
            DateTime? dateOfBirth = null)
        {
            var personId = Guid.NewGuid();
            const string sql = @"
INSERT INTO Person (PersonId, IdNumber, FirstName, LastName, Email, ActiveInd, PhoneNumber, DateOfBirth)
VALUES (@PersonId, @IdNumber, @FirstName, @LastName, @Email, @ActiveInd, @PhoneNumber, @DateOfBirth);";

            await ExecuteAsync(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@PersonId", personId);
                cmd.Parameters.AddWithValue("@IdNumber", idNumber);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@ActiveInd", activeInd);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)phoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", (object?)dateOfBirth ?? DBNull.Value);
            });

            return personId;
        }

        public async Task<Guid> InsertAccountAsync(
            Guid personId,
            string accountNumber,
            string accountType,
            decimal balance,
            bool isClosed = false,
            bool activeInd = true)
        {
            var accountId = Guid.NewGuid();
            const string sql = @"
INSERT INTO Account (AccountId, PersonId, AccountNumber, AccountType, Balance, IsClosed, ActiveInd)
VALUES (@AccountId, @PersonId, @AccountNumber, @AccountType, @Balance, @IsClosed, @ActiveInd);";

            await ExecuteAsync(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@PersonId", personId);
                cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                cmd.Parameters.AddWithValue("@AccountType", accountType);
                cmd.Parameters.AddWithValue("@Balance", balance);
                cmd.Parameters.AddWithValue("@IsClosed", isClosed);
                cmd.Parameters.AddWithValue("@ActiveInd", activeInd);
            });

            return accountId;
        }

        public async Task<Guid> InsertTransactionAsync(
            Guid accountId,
            decimal amount,
            string transactionType,
            DateTime transactionDate,
            string description,
            bool activeInd = true)
        {
            var transactionId = Guid.NewGuid();
            const string sql = @"
INSERT INTO Transactions (TransactionId, AccountId, Amount, TransactionType, TransactionDate, CaptureDate, Description, ActiveInd)
VALUES (@TransactionId, @AccountId, @Amount, @TransactionType, @TransactionDate, @CaptureDate, @Description, @ActiveInd);";

            await ExecuteAsync(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                cmd.Parameters.AddWithValue("@CaptureDate", transactionDate);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@ActiveInd", activeInd);
            });

            return transactionId;
        }

        public async Task<Guid> InsertUserAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            string role,
            string? refreshToken = null,
            DateTime? refreshTokenExpires = null,
            bool activeInd = true)
        {
            var userId = Guid.NewGuid();
            const string sql = @"
INSERT INTO Users (UserId, FirstName, LastName, Email, Password, Role, RefreshToken, RefreshTokenExpires, ActiveInd)
VALUES (@UserId, @FirstName, @LastName, @Email, @Password, @Role, @RefreshToken, @RefreshTokenExpires, @ActiveInd);";

            await ExecuteAsync(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Role", role);
                cmd.Parameters.AddWithValue("@RefreshToken", (object?)refreshToken ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RefreshTokenExpires", (object?)refreshTokenExpires ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActiveInd", activeInd);
            });

            return userId;
        }

        private static async Task EnsureDatabaseAsync()
        {
            using var connection = new SqlConnection(MasterConnectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"IF DB_ID('{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";
            await command.ExecuteNonQueryAsync();
        }

        private async Task EnsureSchemaAsync()
        {
            const string sql = @"
IF OBJECT_ID('dbo.Person', 'U') IS NULL
BEGIN
    CREATE TABLE Person (
        PersonId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        IdNumber INT NOT NULL,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(15) NULL,
        Address NVARCHAR(255) NULL,
        DateOfBirth DATE NULL,
        ActiveInd BIT NOT NULL DEFAULT 1
    );
END;

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        UserId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(100) NOT NULL,
        Role NVARCHAR(50) NOT NULL,
        RefreshToken NVARCHAR(512) NULL,
        RefreshTokenExpires DATETIME2 NULL,
        ActiveInd BIT NOT NULL DEFAULT 1
    );
END;

IF OBJECT_ID('dbo.Account', 'U') IS NULL
BEGIN
    CREATE TABLE Account (
        AccountId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        PersonId UNIQUEIDENTIFIER NOT NULL,
        AccountNumber NVARCHAR(20) NOT NULL UNIQUE,
        AccountType NVARCHAR(50) NULL,
        Balance DECIMAL(18, 2) NOT NULL DEFAULT 0,
        IsClosed BIT NOT NULL DEFAULT 0,
        ActiveInd BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Account_Person FOREIGN KEY (PersonId) REFERENCES Person(PersonId) ON DELETE CASCADE
    );
END;

IF OBJECT_ID('dbo.Transactions', 'U') IS NULL
BEGIN
    CREATE TABLE Transactions (
        TransactionId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        AccountId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        TransactionType VARCHAR(10) NOT NULL,
        TransactionDate DATETIME NOT NULL,
        CaptureDate DATETIME NULL,
        Description NVARCHAR(255) NULL,
        ActiveInd BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Transactions_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId) ON DELETE CASCADE
    );
END;";

            await ExecuteAsync(sql);
        }

        private async Task ExecuteAsync(string sql, Action<SqlCommand>? parameterize = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            parameterize?.Invoke(command);
            await command.ExecuteNonQueryAsync();
        }
    }
}
