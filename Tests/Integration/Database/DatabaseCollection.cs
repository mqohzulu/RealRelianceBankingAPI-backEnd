using Xunit;

namespace Tests.Integration.Database
{
    [CollectionDefinition("Database collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<TestDatabaseFixture>
    {
    }
}
