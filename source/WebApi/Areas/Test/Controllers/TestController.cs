using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json;
using PaymentGateway.Model.PaymentGateway.Common;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Helpers;

namespace PaymentGateway.Areas.Test.Controllers
{
    [CustomAuthorizationFilter]
    public class TestController : Controller
    {
        public string RootUrl => ConfigurationHelper.GetRootUrl(HttpContext.Request);
        public string GatewayUrlBase => RootUrl + "/api/values/";

        // GET: Security/TestApi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotificationUrl()
        {
            return View();
        }

        public ActionResult RequestVendorTxCode(bool wait)
        {
            var client = new WebClient();
            var url = GatewayUrlBase + "RequestToken?name=" + "Portal";
            var token = client.DownloadString(url);

            // add token to next request HEADER
            client.Headers.Add("X-AuthToken", token);

            if (wait)
            {
                Thread.Sleep(60000);
            }

            url = GatewayUrlBase + "RequestVendorTxCode?id=1";

            var vendorTxCode = client.DownloadString(url);

            ViewBag.VendorTxCode = vendorTxCode;

            return View();
        }

        public ActionResult RequestToken(string name)
        {
            var client = new WebClient();

            var url = GatewayUrlBase + "RequestToken?name=" + name;

            var token = client.DownloadString(url);

            ViewBag.Token = token;

            return View();
        }

        public ActionResult SendPaymentRequest()
        {
            const string appName = "Portal"; // must be registered in Payment Gateway

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;

            // 1. Request Token
            // =================
            var url = GatewayUrlBase + "RequestToken?name=" + appName;
            var token = client.DownloadString(url);


            // 2. Request VendorTxCode
            // ========================
            url = GatewayUrlBase + "RequestVendorTxCode?id=1";
            // add token to next request HEADER


            client.Headers.Add("X-AuthToken", token);
            //var vendorTxCode = client.DownloadString(url);
            var vendorTxCode = GenerateVendorTxCode();


            // 3. Create PaymentInfo object
            // =============================
            var addressDetails = new AddressDetails
            {
                Surname = "Bloggs",
                Firstnames = "Joe",
                Address1 = "The Brambles",
                Address2 = "Little Hedgely",
                City = "Burblington",
                PostCode = "NK12",
                Country = "GB",
                State = "",
                Phone = "0778514678"
            };

            var paymentInfo = new PaymentInfo
            {
                VendorTxCode = vendorTxCode,    
                TotalBookingFee = "200.00",
                AccountCode = "0948",
                CostCentre = "E30000",
                ProjectCode = "ENES6552",
                BookingId = 123456,
                NotificationUrl = ConfigurationHelper.GetRootUrl(HttpContext.Request) + ConfigurationHelper.NotificationUrl,
                BillingDetails = addressDetails,
                DeliveryDetails = addressDetails,
                CustomerEmail = "joe.bloggs@gmail.com",
                Basket = "1%3aCPD+Connect++Mindfulness-Based+Stress+Reduction+(MBSR)+-+9+week+course%3a%3a%3a%3a%3a200.00",
                IsMobile = false
            };


            // add paymentInfo to HEADER
            var jsonObj = JsonConvert.SerializeObject(paymentInfo);
            client.Headers.Add("X-PaymentInfo", jsonObj);

            url = GatewayUrlBase + "SendPaymentRequest?id=1";

            var json = client.DownloadString(url);
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            var nextUrl = "error";
            if (response.ContainsKey("NextURL")) { nextUrl = response["NextURL"].ToString(); }

            ViewBag.NextUrl = nextUrl;
            //ViewBag.NextUrl = ConfigurationHelper.GetRootUrl(HttpContext.Request) + ConfigurationHelper.NotificationUrl;
            return View(response);
        }

        public ActionResult RecordPaymentTransactionLog()
        {
            const string appName = "Portal"; // must be registered in Payment Gateway

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;

            // 1. Request Token
            // =================
            var url = GatewayUrlBase + "RequestToken?name=" + appName;
            var token = client.DownloadString(url);
            client.Headers.Add("X-AuthToken", token);

            // 2. Request VendorTxCode
            // ========================
            //url = GatewayUrlBase + "RequestVendorTxCode?id=1";
            // add token to next request HEADER

            // get last VendorTxCode from database
            PaymentTransactionLog paymentTransactionLog = null;
            using (var context = new PGContext())
            {
                var transactionLogs = context.PaymentTransactionLogs;
                paymentTransactionLog = transactionLogs.OrderByDescending(l => l.Id).FirstOrDefault();
            }

            paymentTransactionLog.AuthorisationStatus = "VALID";
            paymentTransactionLog.AuthorisationStatusDetail = "Test Status Detail";
            paymentTransactionLog.AuthorisationTime = DateTime.Now;
            paymentTransactionLog.CardType = "VISA";
            paymentTransactionLog.LastFourDigits = "0123";
            paymentTransactionLog.SecurityKey = "Security Key";



            //var vendorTxCode = client.DownloadString(url);
            //var vendorTxCode = GenerateVendorTxCode();

            //var now = DateTime.Now;

            // the Payment Transaction Log object will be populated from the response object from SendPaymentRequest()
            //var paymentTransactionLog = new PaymentTransactionLog(){VendorTxCode = vendorTxCode, AuthorisationTime = now, RegistrationTime = now};

            // add paymentTransactionLog to HEADER
            var jsonObj = JsonConvert.SerializeObject(paymentTransactionLog);
            client.Headers.Add("X-PaymentTransactionLog", jsonObj);

            url = GatewayUrlBase + "RecordPaymentTransaction?isMobile=false";
            var json = client.DownloadString(url);
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            return View();
        }

        #region GenerateVendorTXCode

        /// Generate Application side unique id for refund transaction
        /// and send it to SAGEPAY for refund processing.        
        /// <returns>string</returns>
        private static string GenerateVendorTxCode()
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
    }
}