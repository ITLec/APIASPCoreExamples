using System;
using System.Threading;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleDataProtection
{
    class Program
    {
        static void Main(string[] args)
        {
            // add data protection services  
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();


            var instance = ActivatorUtilities.CreateInstance<democlass>(services);

            while (true)
            {
                instance.RunSample();
            }
        }
    }
    public class democlass
    {
        IDataProtector _protector;
        private ITimeLimitedDataProtector _timeLimitedProtector;

        // the 'provider' parameter is provided by DI  
        public democlass(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("Contoso.democlass.v1");
            _timeLimitedProtector=  _protector.ToTimeLimitedDataProtector();
        }

        public void RunSample()
        {
            Console.Write("Enter input: ");
            string input = Console.ReadLine();

            // protect the payload  
            string protectedPayload = _timeLimitedProtector.Protect(input,lifetime: TimeSpan.FromSeconds(2));
            Console.WriteLine($"Protect returned: {protectedPayload}");

            Thread.Sleep(2000);

            // unprotect the payload  
            string unprotectedPayload = _timeLimitedProtector.Unprotect(protectedPayload);
            Console.WriteLine($"Unprotect returned: {unprotectedPayload}");
            Console.ReadLine();
        }
    }
}
