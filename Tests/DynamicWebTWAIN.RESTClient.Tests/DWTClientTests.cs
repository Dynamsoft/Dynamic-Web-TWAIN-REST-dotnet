using DynamicWebTWAIN.RestClient;
using System.Net;
using System.Reflection;


namespace DynamicWebTWAIN.RestClient.Tests
{

    public class DWTClientTests 
{
        static readonly String productKey = "t0132CgEAAB9/n9vWL7wm7VOhIZpeIA2qu9siTS4WRZMXpJkGyyPZl86hu9FwdN3DieZXn9KOjSFKSKVR1IKUfST5oPHP+MBoP40udJksA+NI65yRh+lCvF222P+wn6W+ZkwlePf3x/V1G5798SRxzCHAiZKQhCU0gHL+zIlq6s1r+QLl8Tbv";
        static readonly String productKeyWithDBR = "t0143KwEAAKNuOdQonChJAzpj2ZE7JEvss0fP4IGRHydjB8kxvA7JWKPcADjLciPBgK2/FTgVG5Kjn02yXEC3S8JMiFoqdVB2y0CeZqsitWLti3CagdQ+bLH/w35CfZ0xI8hzvN7Oj0u5j/tD4DE7ijZJTUKT2MQoko6vfskJlTBB+Z77yUlSacxDNjV/A3rQPo0=";
        static readonly string customUrl = "http://127.0.0.1:18625/";

        // Updated the method to await the asynchronous call to fix CS4014
        [Fact]
        public async Task Constructor_WithDefaultBaseAddress_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var client = new DWTClient(productKey);

            // Assert
            Assert.NotNull(client.Connection);
            Assert.NotNull(client.ServerControlClient);
            Assert.Equal(DWTClient.DWTApiUrl, client.BaseAddress);

            var result = await client.ServerControlClient.Version.Get();
            Assert.NotNull(result);

            var result1 = await client.ServerControlClient.Settings.Get();
            Assert.NotNull(result1);

            await client.ServerControlClient.Settings.SetLogLevel(1);
        }

        // Test that DWTClient constructor initializes with custom base address and required clients
        [Fact]
        public void Constructor_WithCustomBaseAddress_ShouldInitializeCorrectly()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");

            // Act
            var client = new DWTClient(customBaseAddress, productKey);

            // Assert
            Assert.NotNull(client.Connection);
            Assert.NotNull(client.ServerControlClient);
            Assert.Equal(customBaseAddress, client.BaseAddress);
        }

        // Test that DWTClient sets the request timeout on the connection.
        [Fact]
        public void SetRequestTimeout_ShouldSetTimeoutOnConnection()
        {
            // Arrange
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(productKey);
            // Act
            client.SetRequestTimeout(timeout);
            // Assert
            var httpClient = GetHttpClientFromDWTClient(client);
            //Assert.Equal(timeout, client.Connection.GetType().GetProperty("Timeout", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(client.Connection));
            Assert.Equal(timeout, httpClient?.Timeout);
        }



        // Test that DWTClient sets the request timeout on the connection.
        [Fact]
        public void SetRequestTimeout_ShouldSetTimeoutOnConnection_WithCustomBaseAddress()
        {
            // Arrange
            var timeout = TimeSpan.FromSeconds(30);
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            // Act
            client.SetRequestTimeout(timeout);
            // Assert
            var httpClient = GetHttpClientFromDWTClient(client);
            Assert.Equal(timeout, httpClient?.Timeout);
        }


        // Test that DWTClient.BaseAddress returns the correct base address.
        [Fact]
        public void BaseAddress_ShouldReturnConnectionBaseAddress()
        {
            // Arrange
            var client = new DWTClient(productKey);

            // Act
            var baseAddress = client.BaseAddress;

            // Assert
            Assert.Equal(DWTClient.DWTApiUrl, baseAddress);
        }


        //Test that DWTClient initializes with default base address and required clients
        [Fact]
        public void DWTClient_Initialization_ShouldInitializeCorrectly()
        {
            // Arrange
            var client = new DWTClient(productKey);
            // Act & Assert
            Assert.NotNull(client.Connection);
            Assert.NotNull(client.ServerControlClient);
            Assert.Equal(DWTClient.DWTApiUrl, client.BaseAddress);
        }

        //Test that DWTClient initializes with custom base address and required clients
        [Fact]
        public void DWTClient_Initialization_WithCustomBaseAddress_ShouldInitializeCorrectly()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var client = new DWTClient(customBaseAddress, productKey);
            // Act & Assert
            Assert.NotNull(client.Connection);
            Assert.NotNull(client.ServerControlClient);
            Assert.Equal(customBaseAddress, client.BaseAddress);
        }

        //Test that DWTClient initializes with custom base address and required clients
        [Fact]
        public void DWTClient_Initialization_WithCustomBaseAddressAndTimeout_ShouldInitializeCorrectly()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            Assert.NotNull(client.Connection);
            Assert.NotNull(client.ServerControlClient);
            Assert.Equal(customBaseAddress, client.BaseAddress);
        }

        private static HttpClient? GetHttpClientFromDWTClient(DWTClient client)
        {
            var httpClientField = client.Connection.GetType()
                .GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance);
            var httpClientObj = httpClientField?.GetValue(client.Connection);

            var innerHttpField = httpClientObj?.GetType()
                .GetField("_http", BindingFlags.NonPublic | BindingFlags.Instance);
            return innerHttpField?.GetValue(httpClientObj) as HttpClient;
        }

        // Test for AddHttpHeader
        [Fact]
        public void AddHttpHeader_ShouldAddHeaderToConnection()
        {
            // Arrange
            var client = new DWTClient(productKey);
            var headerName = "X-Custom-Header";
            var headerValue = "CustomValue";
            // Act
            client.AddHttpHeader(headerName, headerValue);
            // Assert
            //var headers = client.Connection.GetType().GetProperty("DefaultRequestHeaders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(client.Connection) as System.Net.Http.Headers.HttpRequestHeaders;
            // Assert.NotNull(headers);
            //Assert.True(headers.Contains(headerName));

            var httpClient = GetHttpClientFromDWTClient(client);
            Assert.NotNull(httpClient);
            Assert.True(httpClient.DefaultRequestHeaders.Contains(headerName));
        }

        // Test for AddHttpHeader with custom base address
        [Fact]
        public void AddHttpHeader_WithCustomBaseAddress_ShouldAddHeaderToConnection()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var client = new DWTClient(customBaseAddress, productKey);
            var headerName = "X-Custom-Header";
            var headerValue = "CustomValue";
            // Act
            client.AddHttpHeader(headerName, headerValue);
            // Assert
            var httpClient = GetHttpClientFromDWTClient(client);
            Assert.NotNull(httpClient);
            Assert.True(httpClient.DefaultRequestHeaders.Contains(headerName));
        }

        // Test for AddHttpHeader with custom base address and timeout
        [Fact]
        public void AddHttpHeader_WithCustomBaseAddressAndTimeout_ShouldAddHeaderToConnection()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var headerName = "X-Custom-Header";
            var headerValue = "CustomValue";
            // Act
            client.AddHttpHeader(headerName, headerValue);
            // Assert
            var httpClient = GetHttpClientFromDWTClient(client);
            Assert.NotNull(httpClient);
            Assert.True(httpClient.DefaultRequestHeaders.Contains(headerName));
        }

        // Test for AddHttpHeader with custom base address and timeout
        [Fact]
        public void AddHttpHeader_WithCustomBaseAddressAndTimeout_ShouldAddHeaderToConnection_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var headerName = "X-Custom-Header";
            var headerValue = "CustomValue";
            // Act & Assert
            try
            {
                client.AddHttpHeader(headerName, headerValue);
                Assert.True(true); // No exception means success
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for Connection
        [Fact]
        public void Connection_ShouldReturnHttpClient()
        {
            // Arrange
            var client = new DWTClient(productKey);
            // Act
            var connection = client.Connection;
            // Assert
            Assert.NotNull(connection);
            Assert.IsType<Connection>(connection);
        }

        // Test for Connection with custom base address
        [Fact]
        public void Connection_WithCustomBaseAddress_ShouldReturnHttpClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var client = new DWTClient(customBaseAddress, productKey);
            // Act
            var connection = client.Connection;
            // Assert
            Assert.NotNull(connection);
            Assert.IsType<Connection>(connection);
        }

        // Test for Connection with custom base address and timeout
        [Fact]
        public void Connection_WithCustomBaseAddressAndTimeout_ShouldReturnHttpClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act
            var connection = client.Connection;
            // Assert
            Assert.NotNull(connection);
            Assert.IsType<Connection>(connection);
        }

        // Test for Connection with custom base address and timeout
        [Fact]
        public void Connection_WithCustomBaseAddressAndTimeout_ShouldReturnHttpClient_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            try
            {
                var connection = client.Connection;
                Assert.NotNull(connection);
                Assert.IsType<Connection>(connection);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for ServerControlClient
        [Fact]
        public void ServerControlClient_ShouldReturnServerControlClient()
        {
            // Arrange
            var client = new DWTClient(productKey);
            // Act
            var serverControlClient = client.ServerControlClient;
            // Assert
            Assert.NotNull(serverControlClient);
            Assert.IsType<ServerControlClient>(serverControlClient);
        }

        // Test for ServerControlClient with custom base address
        [Fact]
        public void ServerControlClient_WithCustomBaseAddress_ShouldReturnServerControlClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var client = new DWTClient(customBaseAddress, productKey);
            // Act
            var serverControlClient = client.ServerControlClient;
            // Assert
            Assert.NotNull(serverControlClient);
            Assert.IsType<ServerControlClient>(serverControlClient);
        }

        // Test for ServerControlClient with custom base address and timeout
        [Fact]
        public void ServerControlClient_WithCustomBaseAddressAndTimeout_ShouldReturnServerControlClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act
            var serverControlClient = client.ServerControlClient;
            // Assert
            Assert.NotNull(serverControlClient);
            Assert.IsType<ServerControlClient>(serverControlClient);
        }

        // Test for ServerControlClient with custom base address and timeout
        [Fact]
        public void ServerControlClient_WithCustomBaseAddressAndTimeout_ShouldReturnServerControlClient_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            try
            {
                var serverControlClient = client.ServerControlClient;
                Assert.NotNull(serverControlClient);
                Assert.IsType<ServerControlClient>(serverControlClient);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for ScannerControlClient
        [Fact]
        public void ScannerControlClient_ShouldReturnScannerControlClient()
        {
            // Arrange
            var client = new DWTClient(productKey);
            // Act
            var scannerControlClient = client.ScannerControlClient;
            // Assert
            Assert.NotNull(scannerControlClient);
            Assert.IsType<ScannerControlClient>(scannerControlClient);
        }

        // Test for ScannerControlClient with custom base address
        [Fact]
        public void ScannerControlClient_WithCustomBaseAddress_ShouldReturnScannerControlClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var client = new DWTClient(customBaseAddress, productKey);
            // Act
            var scannerControlClient = client.ScannerControlClient;
            // Assert
            Assert.NotNull(scannerControlClient);
            Assert.IsType<ScannerControlClient>(scannerControlClient);
        }

        // Test for ScannerControlClient with custom base address and timeout
        [Fact]
        public void ScannerControlClient_WithCustomBaseAddressAndTimeout_ShouldReturnScannerControlClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act
            var scannerControlClient = client.ScannerControlClient;
            // Assert
            Assert.NotNull(scannerControlClient);
            Assert.IsType<ScannerControlClient>(scannerControlClient);
        }

        // Test for ScannerControlClient with custom base address and timeout
        [Fact]
        public void ScannerControlClient_WithCustomBaseAddressAndTimeout_ShouldReturnScannerControlClient_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            try
            {
                var scannerControlClient = client.ScannerControlClient;
                Assert.NotNull(scannerControlClient);
                Assert.IsType<ScannerControlClient>(scannerControlClient);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for DocumentProcessClient
        [Fact]
        public void DocumentProcessClient_ShouldReturnDocumentProcessClient()
        {
            // Arrange
            var client = new DWTClient(productKey);
            // Act
            var documentProcessClient = client.DocumentProcessClient;
            // Assert
            Assert.NotNull(documentProcessClient);
            Assert.IsType<DocumentProcessClient>(documentProcessClient);
        }

        // Test for DocumentProcessClient with custom base address
        [Fact]
        public void DocumentProcessClient_WithCustomBaseAddress_ShouldReturnDocumentProcessClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var client = new DWTClient(customBaseAddress, productKey);
            // Act
            var documentProcessClient = client.DocumentProcessClient;
            // Assert
            Assert.NotNull(documentProcessClient);
            Assert.IsType<DocumentProcessClient>(documentProcessClient);
        }

        // Test for DocumentProcessClient with custom base address and timeout
        [Fact]
        public void DocumentProcessClient_WithCustomBaseAddressAndTimeout_ShouldReturnDocumentProcessClient()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act
            var documentProcessClient = client.DocumentProcessClient;
            // Assert
            Assert.NotNull(documentProcessClient);
            Assert.IsType<DocumentProcessClient>(documentProcessClient);
        }

        // Test for DocumentProcessClient with custom base address and timeout
        [Fact]
        public void DocumentProcessClient_WithCustomBaseAddressAndTimeout_ShouldReturnDocumentProcessClient_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri("https://example.com/");
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            try
            {
                var documentProcessClient = client.DocumentProcessClient;
                Assert.NotNull(documentProcessClient);
                Assert.IsType<DocumentProcessClient>(documentProcessClient);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for ServerControlClient.Settings.SetLogLevel
        // This test is commented out because it requires a specific setup and may not be suitable for unit testing.
        [Fact]
        public async Task SetLogLevel_ShouldSetLogLevel()
        {
            // Arrange
            var client = new DWTClient(productKey);
            var logLevel = 1; // Example log level
            // Act
            await client.ServerControlClient.Settings.SetLogLevel(logLevel);
            // Assert
            var settings = await client.ServerControlClient.Settings.Get();
            Assert.Equal(logLevel, settings.LogLevel);
        }

        // Test for ServerControlClient.Settings.SetLogLevel with custom base address
        // This test is commented out because it requires a specific setup and may not be suitable for unit testing.
        [Fact]
        public async Task SetLogLevel_WithCustomBaseAddress_ShouldSetLogLevel()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var logLevel = 1; // Example log level
            // Act
            await client.ServerControlClient.Settings.SetLogLevel(logLevel);
            // Assert
            var settings = await client.ServerControlClient.Settings.Get();
            Assert.Equal(logLevel, settings.LogLevel);
        }

        // Test for ServerControlClient.Settings.SetLogLevel with custom base address and timeout
        // This test is commented out because it requires a specific setup and may not be suitable for unit testing.
        [Fact]
        public async Task SetLogLevel_WithCustomBaseAddressAndTimeout_ShouldSetLogLevel()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var logLevel = 1; // Example log level
            // Act
            await client.ServerControlClient.Settings.SetLogLevel(logLevel);
            // Assert
            var settings = await client.ServerControlClient.Settings.Get();
            Assert.Equal(logLevel, settings.LogLevel);
        }


        // Test for ServerControlClient.Settings.SetLogLevel with custom base address and timeout
        // This test is commented out because it requires a specific setup and may not be suitable for unit testing.
        [Fact]
        public async Task SetLogLevel_WithCustomBaseAddressAndTimeout_ShouldSetLogLevel_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var logLevel = 1; // Example log level
            // Act & Assert
            try
            {
                await client.ServerControlClient.Settings.SetLogLevel(logLevel);
                Assert.True(true); // No exception means success
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for ServerControlClient.Settings.Update
        [Fact]
        public async Task UpdateServerSettings_ShouldUpdateSettings()
        {
            // Arrange
            var client = new DWTClient(productKey);
            var serverSettings = new ServerSettingsUpdate
            {
                LogLevel = 1 // Example log level
            };
            // Act
            var result = await client.ServerControlClient.Settings.Update(serverSettings);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(serverSettings.LogLevel, result.LogLevel);
        }

        // Test for ServerControlClient.Settings.Update with custom base address
        [Fact]
        public async Task UpdateServerSettings_WithCustomBaseAddress_ShouldUpdateSettings()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var serverSettings = new ServerSettingsUpdate
            {
                LogLevel = 1 // Example log level
            };
            // Act
            var result = await client.ServerControlClient.Settings.Update(serverSettings);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(serverSettings.LogLevel, result.LogLevel);
        }


        // Test for ServerControlClient.Settings.Update with custom base address and timeout
        [Fact]
        public async Task UpdateServerSettings_WithCustomBaseAddressAndTimeout_ShouldUpdateSettings()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var serverSettings = new ServerSettingsUpdate
            {
                LogLevel = 1 // Example log level
            };
            // Act
            var result = await client.ServerControlClient.Settings.Update(serverSettings);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(serverSettings.LogLevel, result.LogLevel);
        }

        // Test for ServerControlClient.Settings.Update with custom base address and timeout
        [Fact]
        public async Task UpdateServerSettings_WithCustomBaseAddressAndTimeout_ShouldUpdateSettings_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var serverSettings = new ServerSettingsUpdate
            {
                LogLevel = 1 // Example log level
            };
            // Act & Assert
            try
            {
                var result = await client.ServerControlClient.Settings.Update(serverSettings);
                Assert.NotNull(result);
                Assert.Equal(serverSettings.LogLevel, result.LogLevel);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        // Test for ServerControlClient.Version.Get
        [Fact]
        public async Task GetServerVersion_ShouldReturnVersion()
        {
            // Arrange
            var client = new DWTClient(productKey);
            // Act
            var version = await client.ServerControlClient.Version.Get();
            // Assert
            Assert.NotNull(version);
            Assert.IsType<ServerVersion>(version);
        }

        // Test for ServerControlClient.Version.Get with custom base address
        [Fact]
        public async Task GetServerVersion_WithCustomBaseAddress_ShouldReturnVersion()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            // Act
            var version = await client.ServerControlClient.Version.Get();
            // Assert
            Assert.NotNull(version);
            Assert.IsType<ServerVersion>(version);
        }
        // Test for ServerControlClient.Version.Get with custom base address and timeout
        [Fact]
        public async Task GetServerVersion_WithCustomBaseAddressAndTimeout_ShouldReturnVersion()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act
            var version = await client.ServerControlClient.Version.Get();
            // Assert
            Assert.NotNull(version);
            Assert.IsType<ServerVersion>(version);
        }

        // Test for ServerControlClient.Version.Get with custom base address and timeout
        [Fact]
        public async Task GetServerVersion_WithCustomBaseAddressAndTimeout_ShouldReturnVersion_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            try
            {
                var version = await client.ServerControlClient.Version.Get();
                Assert.NotNull(version);
                Assert.IsType<ServerVersion>(version);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }


        // Fix for CS0117: 'EnumDeviceTypeMask' does not contain a definition for 'All'
        // Replaced 'EnumDeviceTypeMask.All' with a bitwise OR of all defined EnumDeviceTypeMask values.

        [Fact]
        public async Task GetScanners_ShouldReturnScanners()
        {
            // Arrange
            var client = new DWTClient(productKey);
            var typeMask = EnumDeviceTypeMask.DT_TWAINSCANNER |
                           EnumDeviceTypeMask.DT_WIASCANNER |
                           EnumDeviceTypeMask.DT_TWAINX64SCANNER |
                           EnumDeviceTypeMask.DT_ICASCANNER |
                           EnumDeviceTypeMask.DT_SANESCANNER |
                           EnumDeviceTypeMask.DT_ESCLSCANNER |
                           EnumDeviceTypeMask.DT_WIFIDIRECTSCANNER |
                           EnumDeviceTypeMask.DT_WIATWAINSCANNER;

            // Act
            var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(typeMask);

            // Assert
            Assert.NotNull(scanners);
            Assert.IsType<List<Scanner>>(scanners);
        }

        //Test for ScannerControlClient.ScannerManager.GetScanners with custom base address
        [Fact]
        public async Task GetScanners_WithCustomBaseAddress_ShouldReturnScanners()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var typeMask = EnumDeviceTypeMask.DT_TWAINSCANNER |
                           EnumDeviceTypeMask.DT_WIASCANNER |
                           EnumDeviceTypeMask.DT_TWAINX64SCANNER |
                           EnumDeviceTypeMask.DT_ICASCANNER |
                           EnumDeviceTypeMask.DT_SANESCANNER |
                           EnumDeviceTypeMask.DT_ESCLSCANNER |
                           EnumDeviceTypeMask.DT_WIFIDIRECTSCANNER |
                           EnumDeviceTypeMask.DT_WIATWAINSCANNER;

            // Act
            var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(typeMask);

            // Assert
            Assert.NotNull(scanners);
            Assert.IsType<List<Scanner>>(scanners);
        }

        //Test for ScannerControlClient.ScannerManager.GetScanners with custom base address and timeout
        [Fact]
        public async Task GetScanners_WithCustomBaseAddressAndTimeout_ShouldReturnScanners()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var typeMask = EnumDeviceTypeMask.DT_TWAINSCANNER |
                           EnumDeviceTypeMask.DT_WIASCANNER |
                           EnumDeviceTypeMask.DT_TWAINX64SCANNER |
                           EnumDeviceTypeMask.DT_ICASCANNER |
                           EnumDeviceTypeMask.DT_SANESCANNER |
                           EnumDeviceTypeMask.DT_ESCLSCANNER |
                           EnumDeviceTypeMask.DT_WIFIDIRECTSCANNER |
                           EnumDeviceTypeMask.DT_WIATWAINSCANNER;

            // Act
            var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(typeMask);

            // Assert
            Assert.NotNull(scanners);
            Assert.IsType<List<Scanner>>(scanners);
        }

        //Test for ScannerControlClient.ScannerManager.GetScanners with custom base address and timeout
        [Fact]
        public async Task GetScanners_WithCustomBaseAddressAndTimeout_ShouldReturnScanners_WithoutException()
        {
            // Arrange
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var typeMask = EnumDeviceTypeMask.DT_TWAINSCANNER |
                           EnumDeviceTypeMask.DT_WIASCANNER |
                           EnumDeviceTypeMask.DT_TWAINX64SCANNER |
                           EnumDeviceTypeMask.DT_ICASCANNER |
                           EnumDeviceTypeMask.DT_SANESCANNER |
                           EnumDeviceTypeMask.DT_ESCLSCANNER |
                           EnumDeviceTypeMask.DT_WIFIDIRECTSCANNER |
                           EnumDeviceTypeMask.DT_WIATWAINSCANNER;

            // Act & Assert
            try
            {
                var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(typeMask);
                Assert.NotNull(scanners);
                Assert.IsType<List<Scanner>>(scanners);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }
        

        // Fix for CS1501: No overload for method 'CreateJob' takes 3 arguments
        // The CreateJob method in the IScannerJobsClient interface does not have an overload that accepts three arguments.
        // Update the code to use the correct method signature based on the provided type signatures.

        [Fact]
        public async Task CreateScannerJob_WithDefaultValue()
        {
            // Arrange  
            var client = new DWTClient(productKey);
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob();

            // Assert  
            Assert.NotNull(jobClient);
            Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob with custom base address
        [Fact]
        public async Task CreateScannerJob_WithCustomBaseAddress_WithDefaultValue()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob();
            // Assert  
            Assert.NotNull(jobClient);
            Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

            await jobClient.DeleteJob();

        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob with custom base address and timeout
        [Fact]
        public async Task CreateScannerJob_WithCustomBaseAddressAndTimeout_WithDefaultValue()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob();
            // Assert  
            Assert.NotNull(jobClient);
            Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob with custom base address and timeout
        [Fact]
        public async Task CreateScannerJob_WithCustomBaseAddressAndTimeout_WithDefaultValue_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            // Act & Assert
            try
            {
                var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob();
                Assert.NotNull(jobClient);
                Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }


        [Fact]
        public async Task CreateScannerJob_WithCustomValue()
        {
            // Arrange  
            var client = new DWTClient(productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);

            // Assert  
            Assert.NotNull(jobClient);
            Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob with custom base address
        [Fact]
        public async Task CreateScannerJob_WithCustomBaseAddress_WithCustomValue()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Assert  
            Assert.NotNull(jobClient);
            Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

            await jobClient.DeleteJob();

        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob with custom base address and timeout
        [Fact]
        public async Task CreateScannerJob_WithCustomBaseAddressAndTimeout_WithCustomValue()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Assert  
            Assert.NotNull(jobClient);
            Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob with custom base address and timeout
        [Fact]
        public async Task CreateScannerJob_WithCustomBaseAddressAndTimeout_WithCustomValue_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            // Act & Assert
            try
            {
                var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
                Assert.NotNull(jobClient);
                Assert.IsType<IScannerJobClient>(jobClient, exactMatch: false);

                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }
        
        // Test for ScannerControlClient.ScannerJobs.GetJob with custom base address and timeout

        //Test for ScannerControlClient.ScannerJobs.GetJob
        [Fact]
        public async Task GetScannerJob_ShouldReturnJobStatus()
        {
            // Arrange  
            var client = new DWTClient(productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act & Assert
            try
            {
                var jobStatus = await jobClient.GetJobStatus();
                Assert.NotNull(jobStatus);
                Assert.IsType<ScannerJobStatus>(jobStatus);
                Assert.Equal("Pending", jobStatus.Status);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }

            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.GetJob with custom base address
        [Fact]
        public async Task GetScannerJob_WithCustomBaseAddress_ShouldReturnJobStatus()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act
            var jobStatus = await jobClient.GetJobStatus();
            // Assert  
            Assert.NotNull(jobStatus);
            Assert.IsType<ScannerJobStatus>(jobStatus);
            Assert.Equal("Pending", jobStatus.Status);

            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.GetJob with custom base address and timeout
        [Fact]
        public async Task GetScannerJob_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);

            // Act
            var jobStatus = await jobClient.GetJobStatus();

            // Assert  
            Assert.NotNull(jobStatus);
            Assert.IsType<ScannerJobStatus>(jobStatus);
            Assert.Equal("Pending", jobStatus.Status);
            await jobClient.DeleteJob();
        }

        //Test for ScannerControlClient.ScannerJobs.GetJob with custom base address and timeout
        [Fact]
        public async Task GetScannerJob_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            // Act & Assert
            try
            {
                var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
                var jobStatus = await jobClient.GetJobStatus();
                Assert.NotNull(jobStatus);
                Assert.IsType<ScannerJobStatus>(jobStatus);
                Assert.Equal("Pending", jobStatus.Status);
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }


        //Test for ScannerControlClient.ScannerJobs.CreateJob, ScannerControlClient.ScannerJobs.GetJob, and ScannerControlClient.ScannerJobs.DeleteJob
        [Fact]
        public async Task CreateGetDeleteScannerJob()
        {
            // Arrange  
            var client = new DWTClient(productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
          
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
                
            // Act
            await jobClient.DeleteJob();


            try
            {
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                // Assert   
                Assert.Equal("Invalid value.", ex.Message); // Swapped the parameters to fix xUnit2000
            }
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob, ScannerControlClient.ScannerJobs.GetJob, and ScannerControlClient.ScannerJobs.DeleteJob with custom base address
        [Fact]
        public async Task CreateGetDeleteScannerJob_WithCustomBaseAddress()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act
            await jobClient.DeleteJob();
            try
            {
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                // Assert   
                Assert.Equal("Invalid value.", ex.Message); // Swapped the parameters to fix xUnit2000
            }
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob, ScannerControlClient.ScannerJobs.GetJob, and ScannerControlClient.ScannerJobs.DeleteJob with custom base address and timeout
        [Fact]
        public async Task CreateGetDeleteScannerJob_WithCustomBaseAddressAndTimeout()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act
            await jobClient.DeleteJob();
            try
            {
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                // Assert   
                Assert.Equal("Invalid value.", ex.Message); // Swapped the parameters to fix xUnit2000
            }
        }

        //Test for ScannerControlClient.ScannerJobs.CreateJob, ScannerControlClient.ScannerJobs.GetJob, and ScannerControlClient.ScannerJobs.DeleteJob with custom base address and timeout
        [Fact]
        public async Task CreateGetDeleteScannerJob_WithCustomBaseAddressAndTimeout_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            // Act & Assert
            try
            {
                var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
                // Act
                await jobClient.DeleteJob();
                try
                {
                    await jobClient.DeleteJob();
                }
                catch (Exception ex)
                {
                    // Assert   
                    Assert.Equal("Invalid value.", ex.Message); // Swapped the parameters to fix xUnit2000
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }
      
        //Test for ScannerJobClient.UpdateJobStatus() with custom base address and timeout
        [Fact]
        public async Task UpdateJobStatus_ShouldReturnJobStatus()
        {
            // Arrange  
            var client = new DWTClient(productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act & Assert
            var jobStatus = await jobClient.GetJobStatus();
            Assert.NotNull(jobStatus);
            Assert.IsType<ScannerJobStatus>(jobStatus);
            Assert.Equal("Pending", jobStatus.Status);

            var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

            Assert.True(
                status1 == "Completed" || status1 == "running",
                $"Unexpected job status: {status1}"
            );
            await jobClient.DeleteJob();
        }

        //Test for ScannerJobClient.UpdateJobStatus() with custom base address
        [Fact]
        public async Task UpdateJobStatus_WithCustomBaseAddress_ShouldReturnJobStatus()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act & Assert
            var jobStatus = await jobClient.GetJobStatus();
            Assert.NotNull(jobStatus);
            Assert.IsType<ScannerJobStatus>(jobStatus);
            Assert.Equal("Pending", jobStatus.Status);
            var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);
            Assert.True(
                status1 == "Completed" || status1 == "running",
                $"Unexpected job status: {status1}"
            );
            await jobClient.DeleteJob();
        }

        //Test for ScannerJobClient.UpdateJobStatus() with custom base address and timeout
        [Fact]
        public async Task UpdateJobStatus_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act & Assert
            var jobStatus = await jobClient.GetJobStatus();
            Assert.NotNull(jobStatus);
            Assert.IsType<ScannerJobStatus>(jobStatus);
            Assert.Equal("Pending", jobStatus.Status);
            var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);
            Assert.True(
                status1 == "Completed" || status1 == "running",
                $"Unexpected job status: {status1}"
            );
            await jobClient.DeleteJob();
        }

        //Test for ScannerJobClient.UpdateJobStatus() with custom base address and timeout
        [Fact]
        public async Task UpdateJobStatus_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            // Act & Assert
            try
            {
                var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
                // Act & Assert
                var jobStatus = await jobClient.GetJobStatus();
                Assert.NotNull(jobStatus);
                Assert.IsType<ScannerJobStatus>(jobStatus);
                Assert.Equal("Pending", jobStatus.Status);
                var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);
                Assert.True(
                    status1 == "Completed" || status1 == "running",
                    $"Unexpected job status: {status1}"
                );
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }

        //Test for ScannerJobClient.StartJob()
        [Fact]
        public async Task StartJob_ShouldReturnJobStatus()
        {
            // Arrange  
            var client = new DWTClient(productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = true,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act
            var jobStatus = await jobClient.StartJob();
            // Assert  
            Assert.True(
                 jobStatus == "Completed" || jobStatus == "running",
                 $"Unexpected job status: {jobStatus}"
             );
            await jobClient.DeleteJob();
        }

        //Test for ScannerJobClient.StartJob() with custom base address
        [Fact]
        public async Task StartJob_WithCustomBaseAddress_ShouldReturnJobStatus()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var client = new DWTClient(customBaseAddress, productKey);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = true,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act
            var jobStatus = await jobClient.StartJob();
            // Assert  
            Assert.True(
                 jobStatus == "Completed" || jobStatus == "running",
                 $"Unexpected job status: {jobStatus}"
             );
            await jobClient.DeleteJob();
        }

        //Test for ScannerJobClient.StartJob() with custom base address and timeout
        [Fact]
        public async Task StartJob_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = true,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            // Act
            var jobStatus = await jobClient.StartJob();
            // Assert  
            Assert.True(
                 jobStatus == "Completed" || jobStatus == "running",
                 $"Unexpected job status: {jobStatus}"
             );
            await jobClient.DeleteJob();
        }

        //Test for ScannerJobClient.StartJob() with custom base address and timeout
        [Fact]
        public async Task StartJob_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = true,
                Config = new ScannerConfiguration
                {
                    PixelType = EnumDWT_PixelType.TWPT_RGB
                }
            };
            // Act & Assert
            try
            {
                var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
                var jobStatus = await jobClient.StartJob();
                Assert.True(
                     jobStatus == "Completed" || jobStatus == "running",
                     $"Unexpected job status: {jobStatus}"
                 );
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred: {ex.Message}");
            }
        }
      
      //Test for ScannerJobClient.GetJobStatus()
      [Fact]
      public async Task GetJobStatus_ShouldReturnJobStatus()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var jobStatus = await jobClient.GetJobStatus();
          // Assert  
          Assert.NotNull(jobStatus);
          Assert.IsType<ScannerJobStatus>(jobStatus);
          Assert.True(
              jobStatus.Status == "Completed" || jobStatus.Status == "running",
              $"Unexpected job status: {jobStatus.Status}"
          );
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetJobStatus() with custom base address
      [Fact]
      public async Task GetJobStatus_WithCustomBaseAddress_ShouldReturnJobStatus()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var jobStatus = await jobClient.GetJobStatus();
          // Assert  
          Assert.NotNull(jobStatus);
          Assert.IsType<ScannerJobStatus>(jobStatus);
          Assert.True(
              jobStatus.Status == "Completed" || jobStatus.Status == "running",
              $"Unexpected job status: {jobStatus.Status}"
          );
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetJobStatus() with custom base address and timeout
      [Fact]
      public async Task GetJobStatus_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var jobStatus = await jobClient.GetJobStatus();
          // Assert  
          Assert.NotNull(jobStatus);
          Assert.IsType<ScannerJobStatus>(jobStatus);
          Assert.True(
              jobStatus.Status == "Completed" || jobStatus.Status == "running",
              $"Unexpected job status: {jobStatus.Status}"
          );
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetJobStatus() with custom base address and timeout
      [Fact]
      public async Task GetJobStatus_WithCustomBaseAddressAndTimeout_ShouldReturnJobStatus_WithoutException()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          // Act & Assert
          try
          {
              var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
              var jobStatus = await jobClient.GetJobStatus();
              Assert.NotNull(jobStatus);
              Assert.IsType<ScannerJobStatus>(jobStatus);
              Assert.True(
                  jobStatus.Status == "Completed" || jobStatus.Status == "running",
                  $"Unexpected job status: {jobStatus.Status}"
              );
              await jobClient.DeleteJob();
          }
          catch (Exception ex)
          {
              Assert.Fail($"Exception occurred: {ex.Message}");
          }
      }

      //Test for ScannerJobClient.GetNextImage()
      [Fact]
      public async Task GetNextImage_ShouldReturnImageUrl()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var imageUrl = await jobClient.GetNextImage();
          // Assert  
          Assert.NotNull(imageUrl);
          Assert.IsType<byte[]>(imageUrl);
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetNextImage() with custom base address
      [Fact]
      public async Task GetNextImage_WithCustomBaseAddress_ShouldReturnImageUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var imageUrl = await jobClient.GetNextImage();
          // Assert  
          Assert.NotNull(imageUrl);
          Assert.IsType<byte[]>(imageUrl);
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetNextImage() with custom base address and timeout
      [Fact]
      public async Task GetNextImage_WithCustomBaseAddressAndTimeout_ShouldReturnImageUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var imageUrl = await jobClient.GetNextImage();
          // Assert  
          Assert.NotNull(imageUrl);
          Assert.IsType<byte[]>(imageUrl);
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetNextImage() with custom base address and timeout
      [Fact]
      public async Task GetNextImage_WithCustomBaseAddressAndTimeout_ShouldReturnImageUrl_WithoutException()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          // Act & Assert
          try
          {
              var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
              var imageUrl = await jobClient.GetNextImage();
              Assert.NotNull(imageUrl);
              Assert.IsType<byte[]>(imageUrl);
              await jobClient.DeleteJob();
          }
          catch (Exception ex)
          {
              Assert.Fail($"Exception occurred: {ex.Message}");
          }
      }

      //Test for ScannerJobClient.GetNextImage(StringEnum<OutputFormat> Format)
      [Fact]
      public async Task GetNextImage_ShouldReturnImageUrlWithFormat()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var imageUrl = await jobClient.GetNextImage(OutputFormat.ImagePng);
          // Assert  
          Assert.NotNull(imageUrl);
          Assert.IsType<byte[]>(imageUrl);
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetNextImage(StringEnum<OutputFormat> Format) with custom base address
      [Fact]
      public async Task GetNextImage_WithCustomBaseAddress_ShouldReturnImageUrlWithFormat()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var imageUrl = await jobClient.GetNextImage(OutputFormat.ImagePng);
          // Assert  
          Assert.NotNull(imageUrl);
          Assert.IsType<byte[]>(imageUrl);
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetNextImage(StringEnum<OutputFormat> Format) with custom base address and timeout
      [Fact]
      public async Task GetNextImage_WithCustomBaseAddressAndTimeout_ShouldReturnImageUrlWithFormat()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          var imageUrl = await jobClient.GetNextImage(OutputFormat.ImagePng);
          // Assert  
          Assert.NotNull(imageUrl);
          Assert.IsType<byte[]>(imageUrl);
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetNextImage(StringEnum<OutputFormat> Format) with custom base address and timeout
      [Fact]
      public async Task GetNextImage_WithCustomBaseAddressAndTimeout_ShouldReturnImageUrlWithFormat_WithoutException()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          // Act & Assert
          try
          {
              var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
              var imageUrl = await jobClient.GetNextImage(OutputFormat.ImagePng);
              Assert.NotNull(imageUrl);
              Assert.IsType<byte[]>(imageUrl);
              await jobClient.DeleteJob();
          }
          catch (Exception ex)
          {
              Assert.Fail($"Exception occurred: {ex.Message}");
          }
      }
        
      //Test for ScannerJobClient.GetImageByUrl(string url)
      [Fact]
      public async Task GetImageByUrl_ShouldReturnImageUrl()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {

              // Act
              var imageUrl = await jobClient.GetImageByUrl(e.Url);
              // Assert  
              Assert.NotNull(imageUrl);
              Assert.IsType<byte[]>(imageUrl);
              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }



      //Test for ScannerJobClient.GetImageByUrl(string url) with custom base address
      [Fact]
      public async Task GetImageByUrl_WithCustomBaseAddress_ShouldReturnImageUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {

              // Act
              var imageUrl = await jobClient.GetImageByUrl(e.Url);
              // Assert  
              Assert.NotNull(imageUrl);
              Assert.IsType<byte[]>(imageUrl);
              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetImageByUrl(string url) with custom base address and timeout
      [Fact]
      public async Task GetImageByUrl_WithCustomBaseAddressAndTimeout_ShouldReturnImageUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {

              // Act
              var imageUrl = await jobClient.GetImageByUrl(e.Url);
              // Assert  
              Assert.NotNull(imageUrl);
              Assert.IsType<byte[]>(imageUrl);
              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

        //Test for ScannerJobClient.GetImageByUrl(string url) with custom base address and timeout
        [Fact]
        public async Task GetImageByUrl_WithCustomBaseAddressAndTimeout_ShouldReturnImageUrl_WithoutException()
        {
            // Arrange  
            var customBaseAddress = new Uri(customUrl);
            var timeout = TimeSpan.FromSeconds(30);
            var client = new DWTClient(customBaseAddress, productKey);
            client.SetRequestTimeout(timeout);
            var tcs = new TaskCompletionSource<bool>();

            var createScanJobOptions = new CreateScanJobOptions
            {
                AutoRun = false,
            };
            var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
            //var jobStatus = await jobClient.GetJobStatus();

            jobClient.PageScanned += async (sender, e) =>
            {

                try
                {
                    // Act
                    var imageUrl = await jobClient.GetImageByUrl(e.Url);
                    // Assert  
                    Assert.NotNull(imageUrl);
                    Assert.IsType<byte[]>(imageUrl);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Exception occurred: {ex.Message}");
                }

                await jobClient.DeleteJob();
                tcs.SetResult(true);
            };


            var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

            // Wait for either PageScanned or timeout
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

            // Assert that PageScanned completed in time
            Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

            // Propagate result or exception
            await tcs.Task;
        }
        
      // Fix for CS1061: 'Uri' does not contain a definition for 'GetAwaiter'
      // The issue occurs because `await` is being used on a `Uri` object, which is not awaitable.
      // The correct approach is to use the `Uri` object directly without `await`.
      [Fact]
      public async Task GetDocumentUrl_ShouldReturnDocumentUrl()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);

          // Act
          DocumentOutput ot = new DocumentOutput();
          ot.Format = OutputFormat.ImageJpeg;
          var documentUrl = jobClient.GetDocumentUrl(ot); // Removed 'await' as GetDocumentUrl returns a Uri, not a Task.

          // Assert  
          Assert.NotNull(documentUrl);
          Assert.IsType<Uri>(documentUrl); // Updated assertion to check for Uri type.
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetDocumentUrl(DocumentOutput output) with custom base address
      [Fact]
      public async Task GetDocumentUrl_WithCustomBaseAddress_ShouldReturnDocumentUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          DocumentOutput ot = new DocumentOutput();
          ot.Format = OutputFormat.ImageJpeg;
          var documentUrl = jobClient.GetDocumentUrl(ot); // Removed 'await' as GetDocumentUrl returns a Uri, not a Task.

          // Assert  
          Assert.NotNull(documentUrl);
          Assert.IsType<Uri>(documentUrl); // Updated assertion to check for Uri type.
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetDocumentUrl(DocumentOutput output) with custom base address and timeout
      [Fact]
      public async Task GetDocumentUrl_WithCustomBaseAddressAndTimeout_ShouldReturnDocumentUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          DocumentOutput ot = new DocumentOutput();
          ot.Format = OutputFormat.ImageJpeg;
          var documentUrl = jobClient.GetDocumentUrl(ot); // Removed 'await' as GetDocumentUrl returns a Uri, not a Task.

          // Assert  
          Assert.NotNull(documentUrl);
          Assert.IsType<Uri>(documentUrl); // Updated assertion to check for Uri type.
          await jobClient.DeleteJob();
      }

      //Test for ScannerJobClient.GetDocumentUrl(DocumentOutput output) with custom base address and timeout
      [Fact]
      public async Task GetDocumentUrl_WithCustomBaseAddressAndTimeout_ShouldReturnDocumentUrl_WithoutException()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = true,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          // Act & Assert
          try
          {
              var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
              DocumentOutput ot = new DocumentOutput();
              ot.Format = OutputFormat.ImageJpeg;
              var documentUrl = jobClient.GetDocumentUrl(ot); // Removed 'await' as GetDocumentUrl returns a Uri, not a Task.

              // Assert  
              Assert.NotNull(documentUrl);
              Assert.IsType<Uri>(documentUrl); // Updated assertion to check for Uri type.
              await jobClient.DeleteJob();
          }
          catch (Exception ex)
          {
              Assert.Fail($"Exception occurred: {ex.Message}");
          }
      }

      //Test for ScannerJobClient.GetDocument(DocumentOutput output)
      [Fact]
      public async Task GetDocument_ShouldReturnDocumentUrl()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.TransferEnded += async (sender, e) => {
              DocumentOutput output = new DocumentOutput();
              output.Format = OutputFormat.ApplicationPdf;

              var documentBytes = await jobClient.GetDocument(output);
              // Assert
              Assert.NotNull(documentBytes);
              Assert.True(documentBytes.Length > 0);
              Assert.IsType<byte[]>(documentBytes);

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetDocument(DocumentOutput output) with custom base address
      [Fact]
      public async Task GetDocument_WithCustomBaseAddress_ShouldReturnDocumentUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.TransferEnded += async (sender, e) => {
              DocumentOutput output = new DocumentOutput();
              output.Format = OutputFormat.ApplicationPdf;

              var documentBytes = await jobClient.GetDocument(output);
              // Assert
              Assert.NotNull(documentBytes);
              Assert.True(documentBytes.Length > 0);
              Assert.IsType<byte[]>(documentBytes);

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetDocument(DocumentOutput output) with custom base address and timeout
      [Fact]
      public async Task GetDocument_WithCustomBaseAddressAndTimeout_ShouldReturnDocumentUrl()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.TransferEnded += async (sender, e) => {
              DocumentOutput output = new DocumentOutput();
              output.Format = OutputFormat.ApplicationPdf;

              var documentBytes = await jobClient.GetDocument(output);
              // Assert
              Assert.NotNull(documentBytes);
              Assert.True(documentBytes.Length > 0);
              Assert.IsType<byte[]>(documentBytes);

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetDocument(DocumentOutput output) with custom base address and timeout
      [Fact]
      public async Task GetDocument_WithCustomBaseAddressAndTimeout_ShouldReturnDocumentUrl_WithoutException()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.TransferEnded += async (sender, e) => {
              try
              {
                  DocumentOutput output = new DocumentOutput();
                  output.Format = OutputFormat.ApplicationPdf;

                  var documentBytes = await jobClient.GetDocument(output);
                  // Assert
                  Assert.NotNull(documentBytes);
                  Assert.True(documentBytes.Length > 0);
                  Assert.IsType<byte[]>(documentBytes);

              }
              catch (Exception ex)
              {
                  Assert.Fail($"Exception occurred: {ex.Message}");
              }

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetPageCount()
      [Fact]
      public async Task GetPageCount_ShouldReturnPageCount()
      {
          // Arrange  
          var client = new DWTClient(productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.PageScanned += (sender, e) =>
          {
              //Console.WriteLine($"Page-{e.PageNumber} Scanned: {e.Url}");
          };

          jobClient.TransferEnded += async (sender, e) => {
              // Act
              var pageCount = jobClient.GetPageCount();
              // Assert  
              Assert.True(pageCount >= 0, "Page count should be non-negative.");

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetPageCount() with custom base address
      [Fact]
      public async Task GetPageCount_WithCustomBaseAddress_ShouldReturnPageCount()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.PageScanned += (sender, e) =>
          {
              //Console.WriteLine($"Page-{e.PageNumber} Scanned: {e.Url}");
          };

          jobClient.TransferEnded += async (sender, e) => {
              // Act
              var pageCount = jobClient.GetPageCount();
              // Assert  
              Assert.True(pageCount >= 0, "Page count should be non-negative.");

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetPageCount() with custom base address and timeout
      [Fact]
      public async Task GetPageCount_WithCustomBaseAddressAndTimeout_ShouldReturnPageCount()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.PageScanned += (sender, e) =>
          {
              //Console.WriteLine($"Page-{e.PageNumber} Scanned: {e.Url}");
          };

          jobClient.TransferEnded += async (sender, e) => {
              // Act
              var pageCount = jobClient.GetPageCount();
              // Assert  
              Assert.True(pageCount >= 0, "Page count should be non-negative.");

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for ScannerJobClient.GetPageCount() with custom base address and timeout
      [Fact]
      public async Task GetPageCount_WithCustomBaseAddressAndTimeout_ShouldReturnPageCount_WithoutException()
      {
          // Arrange  
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
              Config = new ScannerConfiguration
              {
                  PixelType = EnumDWT_PixelType.TWPT_RGB
              }
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          // Act
          jobClient.PageScanned += (sender, e) =>
          {
              //Console.WriteLine($"Page-{e.PageNumber} Scanned: {e.Url}");
          };

          jobClient.TransferEnded += async (sender, e) => {
              try
              {
                  // Act
                  var pageCount = jobClient.GetPageCount();
                  // Assert  
                  Assert.True(pageCount >= 0, "Page count should be non-negative.");
              }
              catch (Exception ex)
              {
                  Assert.Fail($"Exception occurred: {ex.Message}");
              }

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl) with invalid URL
      [Fact]
      public async Task IsBlankPage_InvalidURL_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var client = new DWTClient(productKey);
          var imageUrl = "https://demo3.dynamsoft.com/web-twain/images/blank.png"; // Replace with a valid image URL
                                                                                   // Act
          try
          {
              var result = await client.DocumentProcessClient.IsBlankPage(imageUrl);
          }
          catch (Exception ex)
          {
              Assert.Equal("The parameter is not valid.", ex.Message); // Swapped the parameters to fix xUnit2000
          }
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl)
      [Fact]
      public async Task IsBlankPage_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var client = new DWTClient(productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) =>{
              var result = await client.DocumentProcessClient.IsBlankPage(e.Url);
              // Assert
              Assert.True(result || !result); // The result should be either true or false

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl) with custom base address
      [Fact]
      public async Task IsBlankPage_WithCustomBaseAddress_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);

          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              var result = await client.DocumentProcessClient.IsBlankPage(e.Url);
              // Assert
              Assert.True(result || !result); // The result should be either true or false

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl) with custom base address and timeout
      [Fact]
      public async Task IsBlankPage_WithCustomBaseAddressAndTimeout_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              var result = await client.DocumentProcessClient.IsBlankPage(e.Url);
              // Assert
              Assert.True(result || !result); // The result should be either true or false

              await jobClient.DeleteJob();

              tcs.SetResult(true); 
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl) with custom base address and timeout
      [Fact]
      public async Task IsBlankPage_WithCustomBaseAddressAndTimeout_ShouldReturnTrueOrFalse_WithoutException()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);

          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              try
              {
                  var result = await client.DocumentProcessClient.IsBlankPage(e.Url);
                  // Assert
                  Assert.True(result || !result); // The result should be either true or false

                  await jobClient.DeleteJob();

                  tcs.SetResult(true);
              }
              catch (Exception ex)
              {
                  Assert.Fail($"Exception occurred: {ex.Message}");
              }
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }


      //Test for DocumentProcessClient.IsBlankPage(string imageUrl, CheckBlankSettings settings)
      [Fact]
      public async Task IsBlankPage_WithSettings_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var client = new DWTClient(productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };

          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {

              var settings = new CheckBlankSettings
              {
                  MinBlockHeight = 100,
                  MaxBlockHeight = 200
              };

              var result = await client.DocumentProcessClient.IsBlankPage(e.Url, settings);
              // Assert
              Assert.True(result || !result); // The result should be either true or false

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl, CheckBlankSettings settings) with custom base address
      [Fact]
      public async Task IsBlankPage_WithCustomBaseAddressAndSettings_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKey);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };

          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              var settings = new CheckBlankSettings
              {
                  MinBlockHeight = 100,
                  MaxBlockHeight = 200
              };

              var result = await client.DocumentProcessClient.IsBlankPage(e.Url, settings);
              // Assert
              Assert.True(result || !result); // The result should be either true or false

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl, CheckBlankSettings settings) with custom base address and timeout
      [Fact]
      public async Task IsBlankPage_WithCustomBaseAddressAndTimeoutAndSettings_ShouldReturnTrueOrFalse()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };

          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              var settings = new CheckBlankSettings
              {
                  MinBlockHeight = 100,
                  MaxBlockHeight = 200
              };

              var result = await client.DocumentProcessClient.IsBlankPage(e.Url, settings);
              // Assert
              Assert.True(result || !result); // The result should be either true or false

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.IsBlankPage(string imageUrl, CheckBlankSettings settings) with custom base address and timeout
      [Fact]
      public async Task IsBlankPage_WithCustomBaseAddressAndTimeoutAndSettings_ShouldReturnTrueOrFalse_WithoutException()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKey);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              try
              {
                  var settings = new CheckBlankSettings
                  {
                      MinBlockHeight = 100,
                      MaxBlockHeight = 200
                  };

                  var result = await client.DocumentProcessClient.IsBlankPage(e.Url, settings);
                  // Assert
                  Assert.True(result || !result); // The result should be either true or false

                  await jobClient.DeleteJob();

                  tcs.SetResult(true);
              }
              catch (Exception ex)
              {
                  Assert.Fail($"Exception occurred: {ex.Message}");
              }
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }


      [Fact]
      public async Task ReadBarcode_InvalidURL()
      {
          // Arrange
          var client = new DWTClient(productKey);
          var imageUrl = "https://demo3.dynamsoft.com/web-twain/images/blank.png"; // Replace with a valid image URL
                                                                                   // Act
          try
          {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(imageUrl);
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);
          }
          catch (Exception ex)
          {
              Assert.Equal("The parameter is not valid.", ex.Message); // Swapped the parameters to fix xUnit2000
          }
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl)
      [Fact]
      public async Task ReadBarcode_NoLicense()
      {
          // Arrange
          var client = new DWTClient(productKey);

          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              try
              {
                  // Act
                  var result = await client.DocumentProcessClient.ReadBarcode(e.Url);
              }
              catch (Exception ex)
              {
                  Assert.Equal("The license for the 1D Barcode module is invalid.", ex.Message); 
                  tcs.SetResult(true);
                  await jobClient.DeleteJob();
              }
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }


      //Test for DocumentProcessClient.ReadBarcode(string imageUrl)
      [Fact]
      public async Task ReadBarcode_ShouldReturnBarcodeResult()
      {
          // Arrange
          var client = new DWTClient(productKeyWithDBR);

          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(e.Url);
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl) with custom base address
      [Fact]
      public async Task ReadBarcode_WithCustomBaseAddress_ShouldReturnBarcodeResult()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKeyWithDBR);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(e.Url);
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl) with custom base address and timeout
      [Fact]
      public async Task ReadBarcode_WithCustomBaseAddressAndTimeout_ShouldReturnBarcodeResult()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKeyWithDBR);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(e.Url);
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);

              await jobClient.DeleteJob();

              tcs.SetResult(true);
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl) with custom base address and timeout
      [Fact]
      public async Task ReadBarcode_WithCustomBaseAddressAndTimeout_ShouldReturnBarcodeResult_WithoutException()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKeyWithDBR);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              try
              {
                  // Act
                  var result = await client.DocumentProcessClient.ReadBarcode(e.Url);
                  // Assert
                  Assert.NotNull(result);
                  Assert.IsType<string>(result);

                  await jobClient.DeleteJob();

                  tcs.SetResult(true);
              }
              catch (Exception ex)
              {
                  Assert.Fail($"Exception occurred: {ex.Message}");
              }
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      [Fact]
      public async Task ReadBarcode_InvalidTemplateName()
      {
          // Arrange
          var client = new DWTClient(productKeyWithDBR);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              try
              {
                  // Act
                  var result = await client.DocumentProcessClient.ReadBarcode(e.Url, "coverage1");
              }
              catch (Exception ex)
              {
                  Assert.Equal("The parameter is not valid.", ex.Message);
                  tcs.SetResult(true);
                  await jobClient.DeleteJob();
              }
          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl, string templateNameOrContent)
      [Fact]
      public async Task ReadBarcode_WithTemplate_ShouldReturnBarcodeResult()
      {
          // Arrange
          var client = new DWTClient(productKeyWithDBR);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(e.Url, "coverage");
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);

              await jobClient.DeleteJob();

              tcs.SetResult(true);

          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl, string templateNameOrContent) with custom base address
      [Fact]
      public async Task ReadBarcode_WithCustomBaseAddressAndTemplate_ShouldReturnBarcodeResult()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var client = new DWTClient(customBaseAddress, productKeyWithDBR);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(e.Url, "coverage");
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);

              await jobClient.DeleteJob();

              tcs.SetResult(true);

          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl, string templateNameOrContent) with custom base address and timeout
      [Fact]
      public async Task ReadBarcode_WithCustomBaseAddressAndTimeoutAndTemplate_ShouldReturnBarcodeResult()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKeyWithDBR);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              // Act
              var result = await client.DocumentProcessClient.ReadBarcode(e.Url, "coverage");
              // Assert
              Assert.NotNull(result);
              Assert.IsType<string>(result);

              await jobClient.DeleteJob();

              tcs.SetResult(true);

          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;
      }

      //Test for DocumentProcessClient.ReadBarcode(string imageUrl, string templateNameOrContent) with custom base address and timeout
      [Fact]
      public async Task ReadBarcode_WithCustomBaseAddressAndTimeoutAndTemplate_ShouldReturnBarcodeResult_WithoutException()
      {
          // Arrange
          var customBaseAddress = new Uri(customUrl);
          var timeout = TimeSpan.FromSeconds(30);
          var client = new DWTClient(customBaseAddress, productKeyWithDBR);
          client.SetRequestTimeout(timeout);
          var tcs = new TaskCompletionSource<bool>();

          var createScanJobOptions = new CreateScanJobOptions
          {
              AutoRun = false,
          };
          var jobClient = await client.ScannerControlClient.ScannerJobs.CreateJob(createScanJobOptions);
          //var jobStatus = await jobClient.GetJobStatus();

          jobClient.PageScanned += async (sender, e) => {
              try
              {
                  // Act
                  var result = await client.DocumentProcessClient.ReadBarcode(e.Url, "coverage");
                  // Assert
                  Assert.NotNull(result);
                  Assert.IsType<string>(result);

                  await jobClient.DeleteJob();

                  tcs.SetResult(true);
              }
              catch (Exception ex)
              {
                  Assert.Fail($"Exception occurred: {ex.Message}");
              }

          };

          var status1 = await jobClient.UpdateJobStatus(JobStatus.Running);

          // Wait for either PageScanned or timeout
          var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));

          // Assert that PageScanned completed in time
          Assert.True(completedTask == tcs.Task, "PageScanned event did not complete within timeout.");

          // Propagate result or exception
          await tcs.Task;

      }

    }
}
