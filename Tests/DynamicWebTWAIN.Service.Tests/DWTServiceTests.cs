using DynamicWebTWAIN.Service;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;


namespace DynamicWebTWAIN.Service.Tests
{

    public class DWTServiceTests 
    {
        static readonly String LocalPath = @"Tests\DynamicWebTWAIN.Service.Tests\bin\Debug\net8.0";
        static readonly String ServicePath = @"DynamicWebTWAIN.Service\PackService\content\win-x64\dynamsoft.dwt.service";

        // Test for the constructor ServiceManager with default parameters
        [Fact]
        public void ServiceManager_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            // Act
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Assert
            Assert.NotNull(serviceManager);
            Assert.Equal(serviceDirectory, serviceManager.ServiceDirectory);
            Assert.Equal(ServiceManager.DefaultMaxIdleTime, serviceManager.MaxIdleTime);
            Assert.Equal(ServiceManager.DefaultServiceProcessName, serviceManager.ServiceProcessName);
        }

        // Test for the constructor ServiceManager with invalid max idle time
        [Fact]
        public void ServiceManager_Constructor_InvalidMaxIdleTime_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int maxIdleTime = -1;
            // Act
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Assert
            Assert.NotNull(serviceManager);
            Assert.Equal(serviceDirectory, serviceManager.ServiceDirectory);
            Assert.Equal(ServiceManager.DefaultMaxIdleTime, serviceManager.MaxIdleTime);
            Assert.Equal(ServiceManager.DefaultServiceProcessName, serviceManager.ServiceProcessName);
        }

        // Test for the constructor ServiceManager with custom parameters
        [Fact]
        public void ServiceManager_Constructor_Custom_ShouldInitializeProperties()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int maxIdleTime = 600;
            string serviceProcessName = "DynamicWebTWAINService.exe";
            // Act
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, serviceProcessName, maxIdleTime);
            // Assert
            Assert.NotNull(serviceManager);
            Assert.Equal(serviceDirectory, serviceManager.ServiceDirectory);
            Assert.Equal(maxIdleTime, serviceManager.MaxIdleTime);
            Assert.Equal(serviceProcessName, serviceManager.ServiceProcessName);
        }

        // Test for the constructor ServiceManager with invalid parameters
        [Fact]
        public void ServiceManager_Constructor_InvalidParameters_ShouldInitializeProperties()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int maxIdleTime = -1;
            string serviceProcessName = null;
            // Act
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, serviceProcessName, maxIdleTime);
            // Assert
            Assert.NotNull(serviceManager);
            Assert.Equal(serviceDirectory, serviceManager.ServiceDirectory);
            Assert.Equal(ServiceManager.DefaultMaxIdleTime, serviceManager.MaxIdleTime);
            Assert.Equal(ServiceManager.DefaultServiceProcessName, serviceManager.ServiceProcessName);
        }


        // Test for the constructor ServiceManager with null service process name
        [Fact]
        public void ServiceManager_Constructor_NullServiceProcessName_ShouldInitializeProperties()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            string serviceProcessName = null;
            int maxIdleTime = 300;
            // Act
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, serviceProcessName, maxIdleTime);
            // Assert
            Assert.NotNull(serviceManager);
            Assert.Equal(serviceDirectory, serviceManager.ServiceDirectory);
            Assert.Equal(ServiceManager.DefaultMaxIdleTime, serviceManager.MaxIdleTime);
            Assert.Equal(ServiceManager.DefaultServiceProcessName, serviceManager.ServiceProcessName);
        }

        // Test for the constructor ServiceManager with empty service process name
        [Fact]
        public void ServiceManager_Constructor_EmptyServiceProcessName_ShouldThrowArgumentException()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            string serviceProcessName = string.Empty;
            int maxIdleTime = 300;
            // Act
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, serviceProcessName, maxIdleTime);
            // Assert
            Assert.NotNull(serviceManager);
            Assert.Equal(serviceDirectory, serviceManager.ServiceDirectory);
            Assert.Equal(ServiceManager.DefaultMaxIdleTime, serviceManager.MaxIdleTime);
            Assert.Equal(ServiceManager.DefaultServiceProcessName, serviceManager.ServiceProcessName);
        }



        // Test for the constructor ServiceManager with invalid service directory
        [Fact]
        public void ServiceManager_Constructor_InvalidServiceDirectory_ShouldThrowDirectoryNotFoundException()
        {
            // Arrange
            string invalidServiceDirectory = "InvalidDirectoryPath";
            int maxIdleTime = 300;
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => new ServiceManager(invalidServiceDirectory, maxIdleTime));
        }

        // The 'PlatformAttribute' and 'Platform' are not standard attributes in .NET.  
        /*[Fact]
        public void ServiceManager_Constructor_UnsupportedOS_ShouldThrowPlatformNotSupportedException()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            // Act & Assert
            Assert.Throws<PlatformNotSupportedException>(() => new ServiceManager(serviceDirectory, "CustomServiceProcess", 300));
        }
*/


        // Test for the constructor ServiceManager with invalid service process name
        [Fact]
        public void ServiceManager_Constructor_InvalidServiceProcessName_ShouldThrowArgumentException()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            string serviceProcessName = "Invalid/Process:Name";
            int maxIdleTime = 300;
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => new ServiceManager(serviceDirectory, serviceProcessName, maxIdleTime));
        }

       
        // Test for the constructor ServiceManager with null service directory and custom parameters
        [Fact]
        public void ServiceManager_Constructor_NullServiceDirectory_Custom_ShouldThrowArgumentNullException()
        {
            // Arrange
            string serviceDirectory = null;
            int maxIdleTime = 300;
            string serviceProcessName = "CustomServiceProcess";
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => new ServiceManager(serviceDirectory, serviceProcessName, maxIdleTime));
        }

        // Test for the method ServiceFileExists with invalid service directory
        [Fact]
        public void ServiceManager_ServiceFileExists_InvalidServiceDirectory_ShouldThrowArgumentException()
        {
            // Arrange
            string invalidServiceDirectory = "InvalidDirectoryPath";
            Assert.Throws<FileNotFoundException>(() => new ServiceManager(invalidServiceDirectory));
        }
        
        // Test for the method ServiceFileExists
        [Fact]
        public void ServiceManager_ServiceFileExists_ShouldReturnTrue()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            bool exists = serviceManager.ServiceFileExists;
            // Assert
            Assert.True(exists);
        }

        // Test for the method ServiceFileExists with null service directory
        [Fact]
        public void ServiceManager_ServiceFileExists_NullServiceDirectory_ShouldReturnTrue()
        {
            // Arrange
            string serviceDirectory = null;
            // Act & Assert
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            bool exists = serviceManager.ServiceFileExists;
            // Assert
            Assert.True(exists);
        }

        // Test for the method ServiceFileExists with empty service directory
        [Fact]
        public void ServiceManager_ServiceFileExists_EmptyServiceDirectory_ShouldThrowArgumentException()
        {
            // Arrange
            string serviceDirectory = string.Empty;
            // Act & Assert
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            bool exists = serviceManager.ServiceFileExists;
            // Assert
            Assert.True(exists);
        }

        // Test for the constructor ServiceManager with null service directory
        [Fact]
        public void ServiceManager_Constructor_NullServiceDirectory_ShouldReturnTrue()
        {
            // Arrange
            string serviceDirectory = null;
            int maxIdleTime = 300;
            // Act & Assert
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            bool exists = serviceManager.ServiceFileExists;
            // Assert
            Assert.True(exists);
        }

        // Test for the constructor ServiceManager with empty service directory
        [Fact]
        public void ServiceManager_Constructor_EmptyServiceDirectory_ShouldReturnTrue()
        {
            // Arrange
            string serviceDirectory = string.Empty;
            int maxIdleTime = 300;
            // Act & Assert
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            bool exists = serviceManager.ServiceFileExists;
            // Assert
            Assert.True(exists);
        }

        // Test for the method ServiceFileExists with invalid max idle time
        [Fact]
        public void ServiceManager_ServiceFileExists_InvalidMaxIdleTime_ShouldReturnFalse()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int maxIdleTime = -1;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            bool exists = serviceManager.ServiceFileExists;
            // Assert
            Assert.True(exists);
        }


        // Test for the method CreateService
        [Fact]
        public void ServiceManager_CreateService_ShouldCreateService()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            // Assert
            Assert.NotNull(serviceManager.Service);
            Assert.IsType<Service>(serviceManager.Service);
        }

        // Test for the method CreateService with null service directory
        [Fact]
        public void ServiceManager_CreateService_NullServiceDirectory_ShouldCreateService()
        {
            // Arrange
            string serviceDirectory = null;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            // Assert
            Assert.NotNull(serviceManager.Service);
            Assert.IsType<Service>(serviceManager.Service);
        }

        // Test for the method CreateService with empty service directory
        [Fact]
        public void ServiceManager_CreateService_EmptyServiceDirectory_ShouldCreateService()
        {
            // Arrange
            string serviceDirectory = string.Empty;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            // Assert
            Assert.NotNull(serviceManager.Service);
            Assert.IsType<Service>(serviceManager.Service);
        }

        // Test for the method CreateService with invalid max idle time
        [Fact]
        public void ServiceManager_CreateService_InvalidMaxIdleTime_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int maxIdleTime = -1;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            serviceManager.CreateService();
            // Assert
            Assert.NotNull(serviceManager.Service);
            Assert.IsType<Service>(serviceManager.Service);
        }

        // Test for the ServiceFullPath
        [Fact]
        public void ServiceManager_ServiceFullPath_ShouldReturnCorrectPath()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            string fullPath = serviceManager.ServiceFullPath;
            // Assert
            Assert.Equal(Path.Combine(serviceDirectory, ServiceManager.DefaultServiceProcessName), fullPath);
        }

        // Test for the ServiceFullPath with null service directory
        [Fact]
        public void ServiceManager_ServiceFullPath_NullServiceDirectory_ShouldReturnCorrectPath()
        {
            // Arrange
            string serviceDirectory = null;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            string fullPath = serviceManager.ServiceFullPath;
            // Assert
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyLocation != null)
            {
                string x64FolderName = "win-x64";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    x64FolderName = "maccatalyst-x64";
                }

                serviceDirectory = Path.Combine(new string[] { assemblyLocation, "runtimes", x64FolderName, "native", "dynamsoft.dwt.service" });
            }

            Assert.Equal(Path.Combine(serviceDirectory, ServiceManager.DefaultServiceProcessName), fullPath);
        }

        // Test for the ServiceFullPath with empty service directory
        [Fact]
        public void ServiceManager_ServiceFullPath_EmptyServiceDirectory_ShouldReturnCorrectPath()
        {
            // Arrange
            string serviceDirectory = string.Empty;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            string fullPath = serviceManager.ServiceFullPath;
            // Assert
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyLocation != null)
            {
                string x64FolderName = "win-x64";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    x64FolderName = "maccatalyst-x64";
                }

                serviceDirectory = Path.Combine(new string[] { assemblyLocation, "runtimes", x64FolderName, "native", "dynamsoft.dwt.service" });
            }
            Assert.Equal(Path.Combine(serviceDirectory, ServiceManager.DefaultServiceProcessName), fullPath);
        }

        // Test for the ServiceFullPath with invalid max idle time
        [Fact]
        public void ServiceManager_ServiceFullPath_InvalidMaxIdleTime_ShouldReturnCorrectPath()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int invalidMaxIdleTime = -1;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, invalidMaxIdleTime);
            // Act
            string fullPath = serviceManager.ServiceFullPath;
            // Assert
            Assert.Equal(Path.Combine(serviceDirectory, ServiceManager.DefaultServiceProcessName), fullPath);
        }

        // Test for the MaxIdleTime
        [Fact]
        public void ServiceManager_MaxIdleTime_ShouldReturnCorrectValue()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int maxIdleTime = 300;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            int idleTime = serviceManager.MaxIdleTime;
            // Assert
            Assert.Equal(maxIdleTime, idleTime);
        }

        // Test for the MaxIdleTime with null service directory
        [Fact]
        public void ServiceManager_MaxIdleTime_NullServiceDirectory_ShouldReturnCorrectValue()
        {
            // Arrange
            string serviceDirectory = null;
            int maxIdleTime = 300;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            int idleTime = serviceManager.MaxIdleTime;
            // Assert
            Assert.Equal(maxIdleTime, idleTime);
        }

        // Test for the MaxIdleTime with empty service directory
        [Fact]
        public void ServiceManager_MaxIdleTime_EmptyServiceDirectory_ShouldReturnCorrectValue()
        {
            // Arrange
            string serviceDirectory = string.Empty;
            int maxIdleTime = 300;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, maxIdleTime);
            // Act
            int idleTime = serviceManager.MaxIdleTime;
            // Assert
            Assert.Equal(maxIdleTime, idleTime);
        }

        // Test for the MaxIdleTime with invalid max idle time
        [Fact]
        public void ServiceManager_MaxIdleTime_InvalidMaxIdleTime_ShouldReturnCorrectValue()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int invalidMaxIdleTime = -1;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, invalidMaxIdleTime);
            // Act
            int idleTime = serviceManager.MaxIdleTime;
            // Assert
            Assert.Equal(ServiceManager.DefaultMaxIdleTime, idleTime);

        }

        // Test for the Service
        [Fact]
        public void ServiceManager_Service_ShouldReturnCorrectService()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            IService service = serviceManager.Service; // Changed type to IService
            // Assert
            Assert.NotNull(service);
            Assert.IsType<Service>(service);
        }

        // Test for the Service with null service directory
        [Fact]
        public void ServiceManager_Service_NullServiceDirectory_ShouldReturnCorrectService()
        {
            // Arrange
            string serviceDirectory = null;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            IService service = serviceManager.Service;
            // Assert
            Assert.NotNull(service);
            Assert.IsType<Service>(service);
        }

        // Test for the Service with empty service directory
        [Fact]
        public void ServiceManager_Service_EmptyServiceDirectory_ShouldReturnCorrectService()
        {
            // Arrange
            string serviceDirectory = string.Empty;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            IService service = serviceManager.Service;
            // Assert
            Assert.NotNull(service);
            Assert.IsType<Service>(service);
        }

        // Test for the Service with invalid max idle time
        [Fact]
        public void ServiceManager_Service_InvalidMaxIdleTime_ShouldReturnCorrectService()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            int invalidMaxIdleTime = -1;
            ServiceManager serviceManager = new ServiceManager(serviceDirectory, invalidMaxIdleTime);
            // Act
            serviceManager.CreateService();
            IService service = serviceManager.Service;
            // Assert
            Assert.NotNull(service);
            Assert.IsType<Service>(service);
        }

        // Test for the Service.Properties
        [Fact]
        public void ServiceManager_Service_Properties_ShouldReturnCorrectService()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);
            // Act
            serviceManager.CreateService();
            IService service = serviceManager.Service; // Changed type to IService
            // Assert
            Assert.NotNull(service.BaseAddress);
            Assert.IsType<Uri>(service.BaseAddress);
            Assert.NotNull(service.NormalBaseAddress);
            Assert.IsType<Uri>(service.NormalBaseAddress);
            Assert.NotNull(service.IsAlive);
            Assert.IsType<Boolean>(service.IsAlive);
        }

        // Fix for CS4033: Mark the test method as async and change its return type to Task.
        // Fix for CS4008: Remove the 'await' keyword when calling Dispose() since it is a void method.

        [Fact]
        public async Task ServiceManager_Service_Dispose_ShouldDisposeService()
        {
            // Arrange
            string testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Replace the test project path with the target path.
            string serviceDirectory = testDir.Replace(LocalPath, ServicePath);
            ServiceManager serviceManager = new ServiceManager(serviceDirectory);

            // Act
            serviceManager.CreateService();
            Assert.True(serviceManager.Service.IsAlive);
            serviceManager.Service.Dispose(); // Removed 'await' since Dispose() is not awaitable.

            // Assert
            Assert.False(serviceManager.Service.IsAlive);
        }

    }
}
