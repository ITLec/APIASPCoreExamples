using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPITest.Security
{

    public class DataProtectionHelper
    {
        IDataProtector _protector;
        private ITimeLimitedDataProtector _timeLimitedProtector;

        // the 'provider' parameter is provided by DI  
        public DataProtectionHelper(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("Contoso.democlass.v1");
            _timeLimitedProtector = _protector.ToTimeLimitedDataProtector();
        }

        public string Protect(string input)
        {

            // protect the payload  
            string protectedPayload = _timeLimitedProtector.Protect(input, lifetime: TimeSpan.FromSeconds(50));

            return protectedPayload;
        }

        public string Unprotect(string protectedPayload)
        {

            // protect the payload  
            string unprotectedPayload = _timeLimitedProtector.Unprotect(protectedPayload);

            return unprotectedPayload;
        }
    }

}
