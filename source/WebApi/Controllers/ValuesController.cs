using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Common.Helpers;
using Newtonsoft.Json;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Common;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Controllers;
using PaymentGateway.Util.Sage;
using Service;
using static System.String;

namespace PaymentGateway.Controllers
{
    [AllowAnonymous]
    public class ValuesController : AbstractApiController
    {
        #region members

        private readonly IDbContext _dbContext;
        private readonly IService<PaymentOrder> _paymentOrderService;
        private readonly IService<PaymentTransactionLog> _paymentTransactionLogService;

        #endregion

        #region constructors

        public ValuesController()
        {
        }

        public ValuesController(IService<PaymentOrder> paymentOrderService, IService<PaymentTransactionLog> paymentTransactionLogService, IDbContext dbContext)
        {
            _dbContext = dbContext;
            _paymentOrderService = paymentOrderService;
            _paymentTransactionLogService = paymentTransactionLogService;
        }

        #endregion

        private const string StrTransactionType = "PAYMENT";

        /// <summary>
        /// If application is registered (filter attribute) generate a token and return it to the calling app.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [ApplicationApiFilter]
        public string RequestToken(string name)
        {
            var stringToEncrypt = name + "~" + DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo);

            var token = EncryptionHelper.Encrypt(stringToEncrypt);

            return token;
        }

        /// <summary>
        /// Return a unique code to the Vendor for the current transaction.
        /// Requires:: Valid Token attached to the Request HEADER
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ValidTokenApiFilter]
        public string RequestVendorTxCode(int id)
        {
            var vendorTxCode =  SageConfig.GenerateVendorTxCode();
            
            return vendorTxCode;
        }

        /// <summary>
        /// Generate SagePay payment request.
        /// Requires:: Valid Token & VendorTxCode attached to the Request HEADER 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ValidTokenApiFilter]
        public Dictionary<string, object> SendPaymentRequest(int id)
        {
            var headers = Request.Headers;
            var contains = headers.Any(h => h.Key.Contains("X-PaymentInfo"));

            if (!contains)
                return new Dictionary<string, object>();


            var headerValues = Request.Headers.GetValues("X-PaymentInfo");
            var jsonInfo = headerValues.FirstOrDefault();

            var paymentInfo = JsonConvert.DeserializeObject<PaymentInfo>(jsonInfo);

            var vendorTxCode = paymentInfo.VendorTxCode;
            var totalBookingFee = paymentInfo.TotalBookingFee;
            var notificationUrl = paymentInfo.NotificationUrl;
            var customerEmail = paymentInfo.CustomerEmail;
            var basket = paymentInfo.Basket;
            var isMobile = paymentInfo.IsMobile;

            var billingDetails = "&BillingSurname=" + paymentInfo.BillingDetails.Surname +
                                 "&BillingFirstnames=" + paymentInfo.BillingDetails.Firstnames +
                                 "&BillingAddress1=" + paymentInfo.BillingDetails.Address1 +
                                 "&BillingAddress2=" + paymentInfo.BillingDetails.Address2 +
                                 "&BillingCity=" + paymentInfo.BillingDetails.City +
                                 "&BillingPostCode=" + paymentInfo.BillingDetails.PostCode +
                                 "&BillingCountry=" + paymentInfo.BillingDetails.Country +
                                 "&BillingState=" + paymentInfo.BillingDetails.State +
                                 "&BillingPhone=" + paymentInfo.BillingDetails.Phone;

            var deliveryDetails = "&DeliverySurname=" + paymentInfo.DeliveryDetails.Surname +
                                  "&DeliveryFirstnames=" + paymentInfo.DeliveryDetails.Firstnames +
                                  "&DeliveryAddress1=" + paymentInfo.DeliveryDetails.Address1 +
                                  "&DeliveryAddress2=" + paymentInfo.DeliveryDetails.Address2 +
                                  "&DeliveryCity=" + paymentInfo.DeliveryDetails.City +
                                  "&DeliveryPostCode=" + paymentInfo.DeliveryDetails.PostCode +
                                  "&DeliveryCountry=" + paymentInfo.DeliveryDetails.Country +
                                  "&DeliveryState=" + paymentInfo.DeliveryDetails.State +
                                  "&DeliveryPhone=" + paymentInfo.DeliveryDetails.Phone;

            var accountCode = paymentInfo.AccountCode;
            var bookingId = paymentInfo.BookingId;
            var costCentre = paymentInfo.CostCentre;
            var projectCode = paymentInfo.ProjectCode;

            return SendPaymentRequest(vendorTxCode, totalBookingFee, accountCode, bookingId, 
                costCentre, projectCode, notificationUrl, billingDetails, deliveryDetails,
                customerEmail, basket, isMobile);

        }

        [HttpGet]
        [ValidTokenApiFilter]
        public Dictionary<string, string> RecordPaymentTransaction(bool isMobile)
        {
            var dict = new Dictionary<string, string>();
            var notificationStatus = Empty;
            var notificationRedirectURL = Empty;
            var notificationStatusDetail = Empty;

            var headers = Request.Headers;
            var contains = headers.Any(h => h.Key.Contains("X-PaymentTransactionLog"));

            if (!contains)
            {
                notificationStatus = "INVALID";
                notificationStatusDetail = "Parameters were not included in the request.";
                
                dict.Add("Status", notificationStatus);
                dict.Add("StatusDetail", notificationStatusDetail);

                return dict;
            }



            var headerValues = Request.Headers.GetValues("X-PaymentTransactionLog");
            var jsonInfo = headerValues.FirstOrDefault();

            var postedTransactionLog = JsonConvert.DeserializeObject<PaymentTransactionLog>(jsonInfo);

            var transactionLogs = _paymentTransactionLogService.GetAll();

            var selectedTransactionLog = (from l in transactionLogs
                where l.VendorTxCode == postedTransactionLog.VendorTxCode 
                      && l.VPSTxID == postedTransactionLog.VPSTxID
                select l).FirstOrDefault();

            if (selectedTransactionLog != null)
            {
                selectedTransactionLog.AuthorisationStatus = postedTransactionLog.AuthorisationStatus;
                selectedTransactionLog.AuthorisationStatusDetail = postedTransactionLog.AuthorisationStatusDetail;
                selectedTransactionLog.AuthorisationTime = postedTransactionLog.AuthorisationTime;
                selectedTransactionLog.SecurityKey = postedTransactionLog.SecurityKey;
                selectedTransactionLog.CardType = postedTransactionLog.CardType;
                selectedTransactionLog.LastFourDigits = postedTransactionLog.LastFourDigits;

                _dbContext.Entry(selectedTransactionLog).State = EntityState.Modified;
            }
            else
            {
                selectedTransactionLog.AuthorisationStatus = "DUD";
                selectedTransactionLog.AuthorisationStatusDetail = "NOT FOUND";
                selectedTransactionLog.AuthorisationTime = DateTime.Now;                
                selectedTransactionLog.SecurityKey = "Dummy";
                selectedTransactionLog.CardType = "CARD";
                selectedTransactionLog.LastFourDigits = "0000";

                _dbContext.Entry(selectedTransactionLog).State = EntityState.Modified;
            }

            _dbContext.SaveChanges();


            dict = new Dictionary<string, string>();
            dict.Add("Status", notificationStatus);
            dict.Add("StatusDetail", notificationStatusDetail);

            return dict;
        }

        private Dictionary<string, object> SendPaymentRequest(string strVendorTxCode,
                string totalBookingFee,
                string accountCode,
                int bookingId,
                string costCentre,
                string projectCode,
                string notificationUrl,
                string billingDetails,
                string deliveryDetails,
                string customerEmail,
                string basket,
                bool isMobile)
        {
            //** Create the Server POST **
            //** For more details see the Server Protocol 2.23 **
            //** NB: Fields potentially containing non ASCII characters are URLEncoded when included in the POST **
            var strPost = "VPSProtocol=" + SageConfig.StrProtocol;

            //** PAYMENT by default. You can change this in the includes file **
            strPost = strPost + "&TxType=" + StrTransactionType;

            strPost = strPost + "&Vendor=" + SageConfig.StrVendorName;

            //** As generated above **
            strPost = strPost + "&VendorTxCode=" + strVendorTxCode;

            //** Optional: If you are a Sage Pay Partner and wish to flag the transactions with your unique partner id, 
            //it should be passed here
            if (SageConfig.StrPartnerID.Length > 0)
            {
                // ** You can change this in the includes file **
                // URLEncode(strPartnerID);
                strPost = strPost + "&ReferrerID=" + System.Web.HttpUtility.UrlEncode(SageConfig.StrPartnerID,
                              System.Text.Encoding.GetEncoding("ISO-8859-15"));
            }

            //** Formatted to 2 decimal places with leading digit but no commas or currency symbols **
            strPost = strPost + "&Amount=" + totalBookingFee;

            strPost = strPost + "&Currency=" + SageConfig.StrCurrency;

            //** Up to 100 chars of free format description **
            strPost = strPost + "&Description=" + System.Web.HttpUtility.UrlEncode(
                          "Services from NHS Education for Scotland",
                          System.Text.Encoding.GetEncoding("ISO-8859-15"));

            //** The Notification URL is the page to which Server calls back when a transaction completes **
            //** You can change this for each transaction, perhaps passing a session ID or state flag if you wish **
            strPost = strPost + "&NotificationUrl=" + notificationUrl;

            //** Billing Details **                    
            strPost = strPost + billingDetails;

            //** Delivery Details **
            strPost = strPost + deliveryDetails;

            strPost = strPost + "&CustomerEMail=" + customerEmail;
            strPost = strPost + "&Basket=" + basket;

            //** For charities registered for Gift Aid, set to 1 to display the Gift Aid check box on the payment pages **
            strPost = strPost + "&AllowGiftAid=0";

            //** Allow fine control over AVS/CV2 checks and rules by changing this value. 0 is Default **
            //** It can be changed dynamically, per transaction, if you wish. See the Server Protocol document **
            strPost = strPost + "&ApplyAVSCV2=0";

            //** Allow fine control over 3D-Secure checks and rules by changing this value. 0 is Default **
            //** It can be changed dynamically, per transaction, if you wish. See the Server Protocol document **
            strPost = strPost + "&Apply3DSecure=0";

            var profile = isMobile ? "&Profile=LOW" : "&Profile=NORMAL"; //If Mobile device set Profile to Low
            strPost = strPost + profile;


            var response = SageConfig.SendRequest(strPost, "purchase");

            return PrepareNextUrl(strVendorTxCode, totalBookingFee, accountCode, bookingId, costCentre, projectCode, ref response);
        }


        private Dictionary<string, object> PrepareNextUrl(string vendorTxCode, 
            string amount, 
            string accountCode, 
            int bookingId, string costCentre, string projectCode, ref string strResponse)
        {
            strResponse = strResponse.Replace("\r\n", ";'").Replace("'", "");
            var arr = new string[8];
            arr = strResponse.Split(';');
            var ht = new Dictionary<string, object>();

            var vpsTxId = Empty;
            var statusDetail = Empty;
            var securityKey = Empty;
            var status = Empty;

            ht.Add("success", true);
            for (int k = 0; k <= arr.Length - 1; k++)
            {
                string[] subArr = arr[k].Split('=');

                if (subArr[0] == "VPSTxId")
                {
                    ht.Add(subArr[0], subArr[1]);
                    vpsTxId = subArr[1];
                }

                if (subArr[0] == "SecurityKey")
                {
                    ht.Add(subArr[0], subArr[1]);
                    securityKey = subArr[1];
                }

                if (subArr[0] == "NextURL")
                {
                    var sagePayBypassEnabled =
                        System.Web.Configuration.WebConfigurationManager.AppSettings["SagePayBypassEnabled"] ==
                        "True";
                    if (sagePayBypassEnabled)
                    {
                        Object obj = (Object) (subArr[1] + "=False");
                        ht.Add(subArr[0], obj);
                    }
                    else
                    {
                        Object obj = (Object) (subArr[1] + "=" + vpsTxId);
                        ht.Add(subArr[0], obj);
                    }
                }

                if (subArr[0] == "StatusDetail")
                {
                    ht.Add(subArr[0], subArr[1]);
                    statusDetail = subArr[1];
                }

                if (subArr[0] == "Status")
                {
                    ht.Add(subArr[0], subArr[1]);
                    status = subArr[1];
                }
            }


            // INSERT PAYMENT ORDER LOG (SERVICE)
            var paymentOrder = new PaymentOrder()
            {
                Amount = Convert.ToDouble(amount),
                AccountCode = accountCode,
                BookingId = bookingId,
                CostCentre = costCentre,
                ProcessedDate = DateTime.Now,
                ProjectCode = projectCode,
                Status = status,
                StatusDetail = statusDetail,
                VendorTxCode = vendorTxCode,
                VPSTxID = vpsTxId
            };

            _paymentOrderService.Add(paymentOrder);

            // Create TransactionLog entry
            var transactionLog = new PaymentTransactionLog()
            {
                VendorTxCode = vendorTxCode,
                Amount = Convert.ToDecimal(amount),
                VPSTxID = vpsTxId,
                RegistrationStatus = status,
                RegistrationStatusDetail = statusDetail,
                RegistrationTime = DateTime.Now
            };

            _paymentTransactionLogService.Add(transactionLog);

            _dbContext.SaveChanges();

            //if registration is not successful redirect the url to error page            
            if (!status.Equals("OK"))
            {
                ht.Clear();
                ht.Add("success", false);
                ht.Add("msg", statusDetail);
            }

            return ht;

        }

    }
}
