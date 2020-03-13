using System.Collections.Generic;

namespace Service
{
    public interface ISageService
    {
 
        string GetSystemUrl(string environment, string strType);

        string GenerateVendorTxCode();

        string SendRequest(string postQuery, string transactionTypeForUrl, string environment);

        Dictionary<string, object> PrepareNextUrl(bool sagePayBypassEnabled, ref string response);


    }
}