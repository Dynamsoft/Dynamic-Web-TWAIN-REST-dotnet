using DynamicWebTWAIN.ServiceFinder;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;


namespace DynamicWebTWAIN.ServiceFinder.Tests
{

    public class DWTServiceFinderTests
    {
        [Fact]
        public async Task DiscoverServicesAsync_ShouldReturnServiceInfo_WhenServicesAreAvailable()
        {
            // Arrange
            var expectedServiceName = "ellie";
            var expectedServiceTag = "ellie-tag";

            // Act
            var services = await ServiceFinder.DiscoverServicesAsync(); // Fixed CS0176 by qualifying with the type name

            // Assert
            Assert.NotNull(services); // Updated to use Xunit's Assert.NotNull
            Assert.NotEmpty(services); // Updated to use Xunit's Assert.NotEmpty
            Assert.True(services.Length > 0); // Updated to use Xunit's Assert.Equal
            var service = services[0];
            Assert.Equal(expectedServiceName, service.Name); // Updated to use 'Name' instead of 'ServiceType'
            Assert.Equal(expectedServiceTag, service.Tags); // Updated to use 'Tags' instead of 'ServiceName'
            Assert.NotNull(service.Addresses); // Updated to use Xunit's Assert.NotNull
            Assert.NotEmpty(service.Addresses); // Updated to use Xunit's Assert.NotEmpty
            Assert.True(service.Addresses.Length > 0); // Updated to use Xunit's Assert.Equal

            foreach (var address in service.Addresses)
            {
                Assert.NotNull(address.Address); // Updated to use Xunit's Assert.NotNull
                Assert.NotEmpty(address.Address.ToString()); // Updated to use Xunit's Assert.NotEmpty
                var isHttps = address.IsHttps;
                var bStartsWithHttps = address.Address.ToString().StartsWith("https://");
                Assert.True(isHttps == bStartsWithHttps); // Updated to use Xunit's Assert.True
            }
        }
    }
}
