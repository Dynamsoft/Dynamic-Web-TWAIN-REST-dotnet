## Dynamic Web TWAIN Service Finder

This package helps you find the running [Dynamic Web TWAIN](https://www.dynamsoft.com/web-twain/overview/) services via Bonjour.

Example usage:

```csharp
IServiceInfo[] results = await ServiceFinder.DiscoverServicesAsync();
```

