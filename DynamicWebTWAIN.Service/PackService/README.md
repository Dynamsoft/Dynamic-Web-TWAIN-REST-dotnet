## Dynamic Web TWAIN Service

You can embed [Dynamic Web TWAIN](https://www.dynamsoft.com/web-twain/overview/) service in your app with this package to add document scanning function to your app via the service's [REST API](https://www.dynamsoft.com/web-twain/docs/extended-usage/restful-api.html).

The service will stop when your app exits.

### Usage

Start a local service:

```csharp
var serviceManager = new ServiceManager();
serviceManager.CreateService();
```

Stop a running service:

```csharp
serviceManager.Dispose();
```

In addition, if you need to copy the service files and web resources in the service package to your project, you need to add the following to your project.

```xml
<!--update the path based on your setup-->
<Import Project="..\..\DynamicWebTWAIN.Service\runtimes.targets" />
```
