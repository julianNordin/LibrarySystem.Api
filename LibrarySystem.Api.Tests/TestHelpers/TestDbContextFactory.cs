using LibrarySystem.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("LibrarySystemTestDb")
            .Options;

        return new AppDbContext(options);
    }
}
