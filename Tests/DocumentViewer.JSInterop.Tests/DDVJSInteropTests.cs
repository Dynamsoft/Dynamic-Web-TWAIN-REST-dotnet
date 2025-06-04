using DynamicWebTWAIN.Service;
using Dynamsoft.DocumentViewer;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Moq; // Add this to resolve 'Mock<>' error
using DynamicWebTWAIN.Service; // Ensure this is correct for 'IWebView'
using DynamicWebTWAIN.RestClient.Internal;
using DynamicWebTWAIN.RestClient;


namespace DocumentViewer.JSInterop.Tests
{

    public class DDVJSInteropTests
    {
        static readonly String productKey = "t0131DQEAAJ/lU28fZecBIvVDoVs4/k5Ks8uXHXt20fnA2utzW/9gEiH37ujt2ws6Fe8k2rQE845RQ+mf2YkuC/A9hMIQng8ppTmpxUpW0cZAt+oACSMQYQYijEGEKYgwARIGZjqGprG1CGcYGCRCmEDXpKUZIqyC2GFibmhoAgwGAPLTOE4=";


        // Test for the constructor of JSInterop 
        [Fact]
        public async Task TestJSInteropConstructor()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>(); // Mock IWebViewBridge instead of IWebView
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();

            options.ProductKey = productKey;

            // Pass the mock object to the JSInterop constructor
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object, // Correct type passed here
                serviceManager.Service.BaseAddress);

            Assert.NotNull(jsInterop);
        }

        // Test for the constructor of JSInterop with null options
        [Fact]
        public void TestJSInteropConstructorWithNullOptions()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Dynamsoft.DocumentViewer.JSInterop(
                    null,
                    mockWebView.Object,
                    serviceManager.Service.BaseAddress);
            });
        }

        // Test for the constructor of JSInterop with null webView
        [Fact]
        public void TestJSInteropConstructorWithNullWebView()
        {
            // Arrange
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Dynamsoft.DocumentViewer.JSInterop(
                    options,
                    null,
                    serviceManager.Service.BaseAddress);
            });
        }

        // Test for the constructor of JSInterop with null serviceBaseAddress
        [Fact]
        public void TestJSInteropConstructorWithNullServiceBaseAddress()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Dynamsoft.DocumentViewer.JSInterop(
                    options,
                    mockWebView.Object,
                    null);
            });
        }


        // Test for the constructor of JSInterop with invalid product key
        [Fact]
        public void TestJSInteropConstructorWithInvalidProductKey()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = "invalid_product_key";
            // Act & Assert
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
               options,
               mockWebView.Object, // Correct type passed here
               serviceManager.Service.BaseAddress);

            Assert.NotNull(jsInterop);

        }


        // Test for the constructor of JSInterop with invalid service base address
        [Fact]
        public void TestJSInteropConstructorWithInvalidServiceBaseAddress()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            // Act & Assert
            Assert.Throws<UriFormatException>(() =>
            {
                new Dynamsoft.DocumentViewer.JSInterop(
                    options,
                    mockWebView.Object,
                    new Uri("invalid_base_address")); // Fix: Convert string to Uri
            });
        }

      
        // Test for the EnsureInitializedAsync method
        [Fact]
        public async Task TestEnsureInitializedAsync()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            // Assert
            Assert.NotNull(jsInterop.Options);
            Assert.NotNull(jsInterop.WebView);
            Assert.NotNull(jsInterop.DWTClient);
            Assert.Equal(options.ProductKey, jsInterop.Options.ProductKey);
        }


        //Test for JSInterop.CreateScanToViewJob
        [Fact]
        public async Task TestCreateScanToViewJob()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            DynamicWebTWAIN.RestClient.CreateScanJobOptions jobOptions = new DynamicWebTWAIN.RestClient.CreateScanJobOptions();
            jobOptions.AutoRun = false;
            var job = await jsInterop.CreateScanToViewJob(jobOptions);
            // Assert
            Assert.NotNull(job);
        }

        //Test for JSInterop.CreateScanToViewJob with invalid options
        [Fact]
        public async Task TestCreateScanToViewJobWithInvalidOptions()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            DynamicWebTWAIN.RestClient.CreateScanJobOptions jobOptions = new DynamicWebTWAIN.RestClient.CreateScanJobOptions();
            jobOptions.AutoRun = false;
            jobOptions.Device = "invalid_scanner_name";
            // Assert
            await Assert.ThrowsAsync<DynamicWebTWAIN.RestClient.ApiException>(async () =>
            {
                await jsInterop.CreateScanToViewJob(jobOptions);
            });
        }

        //Test for JSInterop.CreateScanToViewJob with null options
        [Fact]
        public async Task TestCreateScanToViewJobWithNullOptions()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await jsInterop.CreateScanToViewJob(null);
            });
        }

        //Test for JSInterop.CreateScanToViewJob with invalid product key
        [Fact]
        public async Task TestCreateScanToViewJobWithInvalidProductKey()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = "invalid_product_key";
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            DynamicWebTWAIN.RestClient.CreateScanJobOptions jobOptions = new DynamicWebTWAIN.RestClient.CreateScanJobOptions();
            jobOptions.AutoRun = false;
            // Assert
            await Assert.ThrowsAsync<DynamicWebTWAIN.RestClient.ForbiddenException>(async () =>
            {
                await jsInterop.CreateScanToViewJob(jobOptions);
            });
        }

        //Test for JSInterop.StartJob
        [Fact]
        public async Task TestStartJob()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            DynamicWebTWAIN.RestClient.CreateScanJobOptions jobOptions = new DynamicWebTWAIN.RestClient.CreateScanJobOptions();
            jobOptions.AutoRun = false;
            var job = await jsInterop.CreateScanToViewJob(jobOptions);
            await jsInterop.StartJob(job);
            // Assert
            Assert.NotNull(job);
        }

        //Test for JSInterop.StartJob with invalid job
        [Fact]
        public async Task TestStartJobWithInvalidJob()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            DynamicWebTWAIN.RestClient.CreateScanJobOptions jobOptions = new DynamicWebTWAIN.RestClient.CreateScanJobOptions();
            jobOptions.AutoRun = false;
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await jsInterop.StartJob(null);
            });
        }


        //Test for JSInterop.ScanImageToView
        [Fact]
        public async Task TestScanImageToView()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            CreateScanJobOptions jobOptions = new CreateScanJobOptions();
            jobOptions.AutoRun = false;
            jobOptions.RequireWebsocket = true;
            await jsInterop.ScanImageToView(jobOptions);
        }

        //Test for JSInterop.ScanImageToView with invalid job
        [Fact]
        public async Task TestScanImageToViewWithInvalidJob()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await jsInterop.ScanImageToView(null);
            });
        }

        //Test for JSInterop.SetCursorToPan
        [Fact]
        public async Task TestSetCursorToPan()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            CreateScanJobOptions jobOptions = new CreateScanJobOptions();
            jobOptions.AutoRun = true;
            await jsInterop.ScanImageToView(jobOptions);
            // Assert
            await jsInterop.SetCursorToPan();
        }

        //Test for JSInterop.SetCursorToPan with invalid product key
        [Fact]
        public async Task TestSetCursorToPanWithInvalidProductKey()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = "invalid_product_key";
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            // Assert
            await Assert.ThrowsAsync<DynamicWebTWAIN.RestClient.ForbiddenException>(async () =>
            {
                await jsInterop.SetCursorToPan();
            });
        }

        //Test for JSInterop.SetCursorToCrop 
        [Fact]
        public async Task TestSetCursorToCrop()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            await jsInterop.SetCursorToCrop();
        }

        //Test for JSInterop.SetCursorToCrop with invalid product key
        [Fact]
        public async Task TestSetCursorToCropWithInvalidProductKey()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = "invalid_product_key";
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            // Assert
            await Assert.ThrowsAsync<DynamicWebTWAIN.RestClient.ForbiddenException>(async () =>
            {
                await jsInterop.SetCursorToCrop();
            });
        }

        //Test for JSInterop.RotateLeft for currentOnly
        [Fact]
        public async Task TestRotateLeftForCurrent()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            mockWebView.Setup(x => x.CallJsMethodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>())).ReturnsAsync(new object());

            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            CreateScanJobOptions jobOptions = new CreateScanJobOptions();
            jobOptions.AutoRun = true;
            await jsInterop.ScanImageToView(jobOptions);
            // Pass the required parameter 'currentOnly' as true or false
            await jsInterop.RotateLeft(true);

            // Assert（可根据需要添加断言验证调用）
            mockWebView.Verify(x =>
                x.CallJsMethodAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>()),
                Times.AtLeastOnce());
        }

        //Test for JSInterop.RotateLeft with invalid product key
        [Fact]
        public async Task TestRotateLeftForCurrentWithInvalidProductKey()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = "invalid_product_key";
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            //await jsInterop.EnsureInitializedAsync();
            // Assert
            await Assert.ThrowsAsync<DynamicWebTWAIN.RestClient.ForbiddenException>(async () =>
            {
                await jsInterop.RotateLeft(true);
            });
        }

        //Test for JSInterop.RotateLeft for all
        [Fact]
        public async Task TestRotateLeftForAll()
        {
            // Arrange
            var mockWebView = new Mock<IWebViewBridge>();
            JSInteropOptions options = new JSInteropOptions();
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.CreateService();
            options.ProductKey = productKey;
            var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                options,
                mockWebView.Object,
                serviceManager.Service.BaseAddress);
            // Act
            // Pass the required parameter 'currentOnly' as true or false
            await jsInterop.RotateLeft(false);
        }



        /*
                //Test for JSInterop.GetNextImage with invalid product key
                [Fact]
                public async Task TestGetNextImageWithInvalidProductKey()
                {
                    // Arrange
                    var mockWebView = new Mock<IWebViewBridge>();
                    JSInteropOptions options = new JSInteropOptions();
                    ServiceManager serviceManager = new ServiceManager();
                    serviceManager.CreateService();
                    options.ProductKey = "invalid_product_key";
                    var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                        options,
                        mockWebView.Object,
                        serviceManager.Service.BaseAddress);
                    // Act
                    //await jsInterop.EnsureInitializedAsync();
                    // Assert
                    await Assert.ThrowsAsync<DynamicWebTWAIN.RestClient.ForbiddenException>(async () =>
                    {
                        await jsInterop.GetNextImage();
                    });
                }

                //Test for JSInterop.GetNextImage with invalid job
                [Fact]
                public async Task TestGetNextImageWithInvalidJob()
                {
                    // Arrange
                    var mockWebView = new Mock<IWebViewBridge>();
                    JSInteropOptions options = new JSInteropOptions();
                    ServiceManager serviceManager = new ServiceManager();
                    serviceManager.CreateService();
                    options.ProductKey = productKey;
                    var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                        options,
                        mockWebView.Object,
                        serviceManager.Service.BaseAddress);
                    // Act
                    //await jsInterop.EnsureInitializedAsync();
                    // Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                    {
                        await jsInterop.GetNextImage(null);
                    });
                }

                //Test for JSInterop.GetNextImage with invalid job
                [Fact]
                public async Task TestGetNextImageWithInvalidJob2()
                {
                    // Arrange
                    var mockWebView = new Mock<IWebViewBridge>();
                    JSInteropOptions options = new JSInteropOptions();
                    ServiceManager serviceManager = new ServiceManager();
                    serviceManager.CreateService();
                    options.ProductKey = productKey;
                    var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                        options,
                        mockWebView.Object,
                        serviceManager.Service.BaseAddress);
                    // Act
                    //await jsInterop.EnsureInitializedAsync();
                    // Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                    {
                        await jsInterop.GetNextImage(null, null);
                    });
                }

                //Test for JSInterop.GetNextImage with invalid job
                [Fact]
                public async Task TestGetNextImageWithInvalidJob3()
                {
                    // Arrange
                    var mockWebView = new Mock<IWebViewBridge>();
                    JSInteropOptions options = new JSInteropOptions();
                    ServiceManager serviceManager = new ServiceManager();
                    serviceManager.CreateService();
                    options.ProductKey = productKey;
                    var jsInterop = new Dynamsoft.DocumentViewer.JSInterop(
                        options,
                        mockWebView.Object,
                        serviceManager.Service.BaseAddress);
                    // Act
                    //await jsInterop.EnsureInitializedAsync();
                    // Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                    {
                        await jsInterop.GetNextImage(null, null, null);
                    });
                }*/

    }
}

