using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Common.Helpers;
using Newtonsoft.Json;
using PaymentGateway.ControllerHelper;
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
        private readonly ISageService _sageService;

        #endregion

        #region constructors

        public ValuesController()
        {
        }

        public ValuesController(IService<PaymentOrder> paymentOrderService, IService<PaymentTransactionLog> paymentTransactionLogService, ISageService sageService, IDbContext dbContext)
        {
            _dbContext = dbContext;
            _paymentOrderService = paymentOrderService;
            _paymentTransactionLogService = paymentTransactionLogService;
            _sageService = sageService;
        }

        #endregion

        private const string transactionType = "PAYMENT";

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
            var vendorTxCode =  _sageService.GenerateVendorTxCode();

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
            {
                var dict = new Dictionary<string, object>
                {
                    {"Status", "Error"}, {"StatusDetail", "Payment Details Missing"}
                };

                return dict;
            }

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

            var strPost = PaymentHelper.BuildPaymentRequest(transactionType, vendorTxCode, totalBookingFee,
                accountCode, bookingId, costCentre, projectCode, notificationUrl, billingDetails, deliveryDetails,
                customerEmail, basket, isMobile);

            var connectTo = SageConfig.StrConnectTo;

            var response = _sageService.SendRequest(strPost, "purchase", connectTo);

            //var sageResponseDict = PrepareNextUrl(vendorTxCode, totalBookingFee, accountCode, bookingId, costCentre, projectCode, ref response);
            var sageResponseDict = _sageService.PrepareNextUrl(false, ref response);

            var success = Convert.ToBoolean(sageResponseDict["success"].ToString());

            if (success)
            {
                
                var status = sageResponseDict["Status"].ToString();
                var statusDetail = sageResponseDict["StatusDetail"].ToString();
                var vPsTxID = sageResponseDict["VPSTxId"].ToString();


                // Insert Payment Order Log
                var paymentOrder = new PaymentOrder()
                {
                    Amount = Convert.ToDouble(totalBookingFee),
                    AccountCode = accountCode,
                    BookingId = bookingId,
                    CostCentre = costCentre,
                    ProcessedDate = DateTime.Now,
                    ProjectCode = projectCode,
                    Status = status,
                    StatusDetail = statusDetail,
                    VendorTxCode = vendorTxCode,
                    VPSTxID = vPsTxID
                };

                _paymentOrderService.Add(paymentOrder);

                // Create TransactionLog entry
                var transactionLog = new PaymentTransactionLog()
                {
                    VendorTxCode = vendorTxCode,
                    Amount = Convert.ToDecimal(totalBookingFee),
                    VPSTxID = vPsTxID,
                    RegistrationStatus = status,
                    RegistrationStatusDetail = statusDetail,
                    RegistrationTime = DateTime.Now
                };

                _paymentTransactionLogService.Add(transactionLog);

                // commit changes to database
                _dbContext.SaveChanges();

            }

            return sageResponseDict;
        }

        [HttpGet]
        [ValidTokenApiFilter]
        public Dictionary<string, string> RecordPaymentTransaction(bool isMobile)
        {
            var dict = new Dictionary<string, string>();
            var notificationStatus = Empty;
            var notificationRedirectUrl = Empty;
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

            // get the transaction log for the vendor code and transaction id.
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
                selectedTransactionLog = new PaymentTransactionLog
                {
                    AuthorisationStatus = "DUD",
                    AuthorisationStatusDetail = "NOT FOUND",
                    AuthorisationTime = DateTime.Now,
                    SecurityKey = "Dummy",
                    CardType = "CARD",
                    LastFourDigits = "0000"
                };


                _dbContext.Entry(selectedTransactionLog).State = EntityState.Modified;
            }

            _dbContext.SaveChanges();


            dict = new Dictionary<string, string>
            {
                {"Status", notificationStatus}, {"StatusDetail", notificationStatusDetail}
            };

            return dict;
        }

        [HttpGet]
        [ValidTokenApiFilter]
        public bool CheckOrderExists()
        {
            var returnValue = false;

            return returnValue;
        }

        #region NOT USED

        private Dictionary<string, object> PrepareNextUrl(ref string response)
        {
            response = response.Replace("\r\n", ";'").Replace("'", "");
            var arr = new string[8];
            arr = response.Split(';');
            var sageResponseDict = new Dictionary<string, object>();

            var vpsTxId = Empty;
            var statusDetail = Empty;
            var securityKey = Empty;
            var status = Empty;

            sageResponseDict.Add("success", true);

            for (var k = 0; k <= arr.Length - 1; k++)
            {
                var subArr = arr[k].Split('=');

                if (subArr[0] == "VPSTxId")
                {
                    sageResponseDict.Add(subArr[0], subArr[1]);
                    vpsTxId = subArr[1];
                }

                if (subArr[0] == "SecurityKey")
                {
                    sageResponseDict.Add(subArr[0], subArr[1]);
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
                        sageResponseDict.Add(subArr[0], obj);
                    }
                    else
                    {
                        Object obj = (Object) (subArr[1] + "=" + vpsTxId);
                        sageResponseDict.Add(subArr[0], obj);
                    }
                }

                if (subArr[0] == "StatusDetail")
                {
                    sageResponseDict.Add(subArr[0], subArr[1]);
                    statusDetail = subArr[1];
                }

                if (subArr[0] == "Status")
                {
                    sageResponseDict.Add(subArr[0], subArr[1]);
                    status = subArr[1];
                }
            }


            //// INSERT PAYMENT ORDER LOG (SERVICE)
            //var paymentOrder = new PaymentOrder()
            //{
            //    Amount = Convert.ToDouble(amount),
            //    AccountCode = accountCode,
            //    BookingId = bookingId,
            //    CostCentre = costCentre,
            //    ProcessedDate = DateTime.Now,
            //    ProjectCode = projectCode,
            //    Status = status,
            //    StatusDetail = statusDetail,
            //    VendorTxCode = vendorTxCode,
            //    VPSTxID = vpsTxId
            //};

            //_paymentOrderService.Add(paymentOrder);

            //// Create TransactionLog entry
            //var transactionLog = new PaymentTransactionLog()
            //{
            //    VendorTxCode = vendorTxCode,
            //    Amount = Convert.ToDecimal(amount),
            //    VPSTxID = vpsTxId,
            //    RegistrationStatus = status,
            //    RegistrationStatusDetail = statusDetail,
            //    RegistrationTime = DateTime.Now
            //};

            //_paymentTransactionLogService.Add(transactionLog);

            //_dbContext.SaveChanges();

            //if registration is not successful redirect the url to error page

            if (!status.Equals("OK"))
            {
                sageResponseDict.Clear();
                sageResponseDict.Add("success", false);
                sageResponseDict.Add("msg", statusDetail);
            }

            return sageResponseDict;
        }

        #endregion
    }
}
