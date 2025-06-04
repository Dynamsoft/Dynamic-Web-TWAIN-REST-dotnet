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