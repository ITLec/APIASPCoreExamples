using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreWebAPITest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

//            var builder = new ConfigurationBuilder()
////.SetBasePath(Directory.GetCurrentDirectory())
//.AddJsonFile("awesomeConfig.json")
//.AddXmlFile("awesomeConfig.xml")
//.AddIniFile("awesomeConfig.ini")
//.AddCommandLine(args)
//.AddEnvironmentVariables()
//.AddInMemoryCollection(someSettings)
//.AddUserSecrets("awesomeSecrets")
//.AddAzureKeyVault("https://awesomevault.vault.azure.net/",
//"<clientId>", "<secret>");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
