using Application.Interfaces;
using EntityFrameworkCoreMock;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Helpers
{
    public static class MockDependencyHelper
    {
        public static IConfiguration GetMockConfiguration()
        {
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("my-secret-key-that-will-be-used-for-unit-testing");
            mockConfiguration.Setup(c => c["Jwt:ExpirationInMinutes"]).Returns("2");
            mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("unit-tester");
            mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("unit-tester");
            mockConfiguration.Setup(c => c["RefTok:ExpirationInMinutes"]).Returns("5");
            mockConfiguration.Setup(c => c["Email:VerificationTokenExpirationInMinutes"]).Returns("5");

            return mockConfiguration.Object;
        }

        public static DbContextMock<ApplicationDbContext> GetMockDbContext()
        {
            DbContextOptions<ApplicationDbContext> dummyOptions = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

            DbContextMock<ApplicationDbContext> mockDbContext = new DbContextMock<ApplicationDbContext>(dummyOptions);

            return mockDbContext;
        }

        public static IEmailService GetMockEmailService()
        {
            Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(x => x.SendEmailVerificationLinkAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.FromResult(true));
            return mockEmailService.Object;
        }

        public static IHttpHelper GetMockHttpHelper()
        {
            Mock<IHttpHelper> mockHttpHelper = new Mock<IHttpHelper>();

            mockHttpHelper.Setup(x => x.GetHostPathPrefix()).Returns(string.Empty);

            mockHttpHelper.Setup(x => x.GetCurrentUserId()).Returns(Guid.Empty);

            return mockHttpHelper.Object;
        }
    }
}
