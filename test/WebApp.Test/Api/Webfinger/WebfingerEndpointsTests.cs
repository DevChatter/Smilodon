using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http.HttpResults;
using Smilodon.WebApp.Api.Webfinger;

namespace WebApp.Test.Api.Webfinger
{
    public class WebfingerEndpointsTests
    {
        [Theory, AutoData]
        public async Task GetAllAccountReturnsResource(string resourceName)
        {
            var result = await WebfingerEndpoints.GetAllAccount(resourceName);
            result.Should().BeOfType<Ok<string>>()
                .Which.Value.Should().Be(resourceName);
        }
    }
}
