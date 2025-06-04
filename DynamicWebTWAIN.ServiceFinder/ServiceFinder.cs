using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zeroconf;

namespace DynamicWebTWAIN.ServiceFinder
{
    public interface IServiceAddress
    {
        /// <summary>
        /// Gets the service address.
        /// </summary>
        /// <returns>The service address as a string.</returns>
        Uri Address { get; }
        /// <summary>
        /// Determines whether the service uses HTTPS.
        /// </summary>
        /// <returns>True if the service uses HTTPS; otherwise, false.</returns>
        bool IsHttps { get; }
    }
    public interface IServiceInfo
    {
        IServiceAddress[] Addresses { get; }

        string Name { get; }

        string Tags { get; }
    }

    public class ServiceAddress : IServiceAddress
    {
        public ServiceAddress(Uri address, bool isHttps)
        {
            Address = address;
            IsHttps = isHttps;
        }
        public Uri Address { get; set; }
        public bool IsHttps { get; set; }
    }

    public class ServiceInfo : IServiceInfo
    {
        public ServiceInfo(IServiceAddress[] addresses, string name, string tags)
        {
            Addresses = addresses;
            Name = name;
            Tags = tags;
        }
        public IServiceAddress[] Addresses { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
    }

    public class ServiceFinder
    {
        private static List<string> SplitModules(string input)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string currentToken = "";

            foreach (char c in input)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentToken.Trim());
                    currentToken = "";
                }
                else
                {
                    currentToken += c;
                }
            }

            result.Add(currentToken.Trim());

            return result;
        }

        public static async Task<IServiceInfo[]> DiscoverServicesAsync()
        {
            List<ServiceInfo> result = new List<ServiceInfo>();
            try
            {
                // Specify the service type to search for (e.g., "_http._tcp.local.")
                string serviceType = "_privet._tcp.local.";

                Console.WriteLine("Searching for Bonjour services...");

                // Discover services
                var responses = await ZeroconfResolver.ResolveAsync(serviceType);

                foreach (var response in responses)
                {
                    foreach (var service in response.Services)
                    {
                        foreach (var prop in service.Value.Properties)
                        {
                            string type;
                            if (prop.TryGetValue("type", out type))
                            {
                                if (type.Equals("dynamsoft_private_cloud_scan", StringComparison.OrdinalIgnoreCase))
                                {
                                    string modules;
                                    if (prop.TryGetValue("modules", out modules))
                                    {
                                        // is a json array
                                        //["1, 9, 0, 0428",2,18625,18622,2,18626,18623,"dbr_9620318","dwasm2_19000318","upload_1900318","ddm_19100428","dwt_19100428"]
                                        //Console.WriteLine($"modules: {modules}");
                                    }
                                    string tags;
                                    if (prop.TryGetValue("tags", out tags))
                                    {
                                        //Console.WriteLine($"tags: {tags}");
                                    }

                                    List<string> listModules = SplitModules(modules);

                                    if (null != listModules)
                                    {
                                        int index = 0;
                                        List<ServiceAddress> serviceAddress = new List<ServiceAddress>();

                                        int httpPortCount = 0;
                                        int httpPortCountIndex = 1;
                                        int httpsPortCount = 0;
                                        int httpsPortCountIndex = listModules.Count;

                                        foreach (var item in listModules)
                                        {
                                            if (index == httpPortCountIndex)
                                            {
                                                if (int.TryParse(item, out httpPortCount))
                                                {
                                                    httpsPortCountIndex = httpPortCount + httpPortCountIndex + 1;
                                                }
                                            }
                                            else if (index == httpsPortCountIndex)
                                            {
                                                if (!int.TryParse(item,out httpsPortCount))
                                                {
                                                    httpsPortCountIndex = httpsPortCount = 0;
                                                }
                                            }
                                            else if (index > httpPortCountIndex && index < httpsPortCountIndex)
                                            {
                                                ServiceAddress serviceAddressItem = new ServiceAddress(new Uri($"http://{response.IPAddress}:{item}"), false);
                                                serviceAddress.Add(serviceAddressItem);
                                            }
                                            else if (index > httpsPortCountIndex && index <= (httpsPortCountIndex + httpsPortCount))
                                            {
                                                ServiceAddress serviceAddressItem = new ServiceAddress(new Uri($"https://{response.IPAddress}:{item}"), true);
                                                serviceAddress.Add(serviceAddressItem);
                                            }

                                            ++index;
                                        }

                                        ServiceInfo serviceInfo = new ServiceInfo(
                                           serviceAddress.ToArray(),
                                           response.DisplayName,
                                           tags);
                                        result.Add(serviceInfo);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //Console.WriteLine($"Service Name: {response.DisplayName}");
                    //Console.WriteLine($"Host: {response.IPAddress}");
                    //Console.WriteLine($"IP Address: {string.Join(", ", response.IPAddresses)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error discovering services: {ex.Message}");
            }

            return result.ToArray();
        }
    }
}
