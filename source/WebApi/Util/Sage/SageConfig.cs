using System;
using System.IO;
using System.Net;
using System.Text;
using PaymentGateway.Util.Helpers;

namespace PaymentGateway.Util.Sage
{
    public static class SageConfig
    {
        #region Static Config

        //******************************************** START- SAGEPAY CONFIGURATION SETTING ************************

        //**Set to SIMULATOR for the Simulator expert system, TEST for the Test Server **
        //**and LIVE in the live environment **
        public static String StrConnectTo = ConfigurationHelper.SageEnvironment;

        //** IMPORTANT.  Set the strYourSiteFQDN value to the Fully Qualified Domain Name of your server. **
        //** This should start http:// or https:// and should be the name by which our servers can call back to yours **
        //** i.e. it MUST be resolvable externally, and have access granted to the Sage Pay servers **
        //** examples would be https://www.mysite.com or http://212.111.32.22/ **
        //** NOTE: You should leave the final / in place. **

        /// This will fetch the server url of notification page from sagepay
        //private const String strYourSiteFQDN = "http://[your web site]/";
        public static string StrYourSiteFqdn = ConfigurationHelper.NotificationUrl;

        //** At the end of a Server transaction, the customer is redirected back to the completion page **
        //** on your site using a client-side browser redirect. On live systems, this page will always be **
        //** referenced using the strYourSiteFQDN value above.  During development and testing, however, it **
        //** is often the case that the development machine sits behind the same firewall as the server **
        //** hosting the kit, so your browser might not be able resolve external IPs or dns names. **
        //** e.g. Externally your server might have the IP 212.111.32.22, but behind the firewall it **
        //** may have the IP 192.168.0.99.  If your test machine is also on the 192.168.0.n network **
        //** it may not be able to resolve 212.111.32.22. **
        //** Set the strYourSiteInternalFQDN to the internal Fully Qualified Domain Name by which **
        //** your test machine can reach the server (in the example above you'd use http://192.168.0.99/) **
        //** If you are not on the same network as the test server, set this value to the same value **
        //** as strYourSiteFQDN above. **
        //** NOTE: You should leave the final / in place. **

        //private const String strYourSiteInternalFQDN = "http://[your web site]/";
        public static string StrYourSiteInternalFqdn = ConfigurationHelper.NotificationUrl;

        // "[your Sage Pay Vendor Name]"; 
        //** Set this value to the Vendor Name assigned to you by Sage Pay or chosen when you applied **
        public static String StrVendorName = "nhseducation";

        //** Set this to indicate the currency in which you wish to trade. You will need a merchant number in this currency **
        public static String StrCurrency = "GBP";

        //** Optional setting. If you are a Sage Pay Partner and wish to flag the transactions with your unique partner id set it here.
        public static String StrPartnerID = "";

        //**************************************************************************************************
        // Global Definitions for this site
        //**************************************************************************************************
        public const String StrProtocol = "2.23";


        //******************************************** END- SAGEPAY CONFIGURATION SETTING ************************

        #endregion

        #region Sage URLS

        public static string SystemUrl(string strConnectTo, string strType)
        {
            string strSystemUrl = "";

            if (strConnectTo == "LIVE")
            {
                switch (strType)
                {
                    case "abort":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/abort.vsp";
                        break;
                    case "authorise":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/authorise.vsp";
                        break;
                    case "cancel":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/cancel.vsp";
                        break;
                    case "purchase":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/vspserver-register.vsp";
                        break;
                    case "refund":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/refund.vsp";
                        break;
                    case "release":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/release.vsp";
                        break;
                    case "repeat":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/repeat.vsp";
                        break;
                    case "void":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/void.vsp";
                        break;
                    case "3dcallback":
                        strSystemUrl = "https://live.sagepay.com/gateway/service/direct3dcallback.vsp";
                        break;
                    case "showpost":
                        strSystemUrl = "https://test.sagepay.com/showpost/showpost.asp";
                        break;
                }
            }
            else if (strConnectTo == "TEST")
            {
                switch (strType)
                {
                    case "abort":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/abort.vsp";
                        break;
                    case "authorise":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/authorise.vsp";
                        break;
                    case "cancel":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/cancel.vsp";
                        break;
                    case "purchase":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/vspserver-register.vsp";
                        break;
                    case "refund":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/refund.vsp";
                        break;
                    case "release":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/release.vsp";
                        break;
                    case "repeat":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/repeat.vsp";
                        break;
                    case "void":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/void.vsp";
                        break;
                    case "3dcallback":
                        strSystemUrl = "https://test.sagepay.com/gateway/service/direct3dcallback.vsp";
                        break;
                    case "showpost":
                        strSystemUrl = "https://test.sagepay.com/showpost/showpost.asp";
                        break;
                }
            }
            else
            {
                switch (strType)
                {
                    case "abort":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorAbortTx";
                        break;
                    case "authorise":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorAuthoriseTx";
                        break;
                    case "cancel":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorCancelTx";
                        break;
                    case "purchase":
                        strSystemUrl = "https://test.sagepay.com/simulator/VSPServerGateway.asp?Service=VendorRegisterTx";
                        break;
                    case "refund":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorRefundTx";
                        break;
                    case "release":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorReleaseTx";
                        break;
                    case "repeat":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorRepeatTx";
                        break;
                    case "void":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorVoidTx";
                        break;
                    case "3dcallback":
                        strSystemUrl = "https://test.sagepay.com/simulator/vspserverCallback.asp";
                        break;
                    case "showpost":
                        strSystemUrl = "https://test.sagepay.com/showpost/showpost.asp";
                        break;
                }
            }

            return strSystemUrl;
        }

        #endregion

        #region GenerateVendorTXCode

        /// Generate Application side unique id for refund transaction
        /// and send it to SAGEPAY for refund processing.        
        /// <returns>string</returns>
        public static string GenerateVendorTxCode()
        {
            string strVendorTxCode = "";
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            strVendorTxCode = string.Format("{0:x}", i - DateTime.Now.Ticks);

            return strVendorTxCode;
        }

        #endregion

        #region Send Request to Sage

        public static string SendRequest(string PostQuery, string TransactionTypeForURL)
        {
            UTF8Encoding objUtfEncode = new UTF8Encoding();
            byte[] arrRequest = null;
            Stream objStreamReq = default(Stream);
            HttpWebRequest objHttpRequest = default(HttpWebRequest);
            StreamReader objStreamRes = default(StreamReader);
            HttpWebResponse objHttpResponse = default(HttpWebResponse);
            Uri objUri = new Uri(SystemUrl(StrConnectTo.ToUpper(), TransactionTypeForURL.ToLower()));

            objHttpRequest = (HttpWebRequest)WebRequest.Create(objUri);
            objHttpRequest.KeepAlive = false;
            objHttpRequest.Method = "POST";

            objHttpRequest.ContentType = "application/x-www-form-urlencoded";

            arrRequest = objUtfEncode.GetBytes(PostQuery);
            objHttpRequest.ContentLength = arrRequest.Length;
            objStreamReq = objHttpRequest.GetRequestStream();
            objStreamReq.Write(arrRequest, 0, arrRequest.Length);
            objStreamReq.Close();

            objHttpResponse = (HttpWebResponse)objHttpRequest.GetResponse();
            objStreamRes = new StreamReader(objHttpResponse.GetResponseStream(), Encoding.ASCII);
            string response = objStreamRes.ReadToEnd();
            objStreamRes.Close();

            return response;        
        }

        #endregion
    }
}