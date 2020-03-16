using System;
using System.Collections.Generic;
using System.Web;
using Common;
using PaymentGateway.Util.Sage;

namespace PaymentGateway.ControllerHelper
{
    public static class PaymentHelper
    {
        public static Dictionary<PaymentReturnCode, string> ReturnUrLs = new Dictionary<PaymentReturnCode, string>(){
            {PaymentReturnCode.BookingError, "/IBooklet/BookingError"},
            {PaymentReturnCode.WaitingList, "/IBooklet/WaitingList"},
            {PaymentReturnCode.BookingConfirm, "/IBooklet/BookingConfirm"},
            {PaymentReturnCode.PaymentErrorNoParameters, "/IBooklet/PaymentErrorNoParameters"},
            {PaymentReturnCode.PaymentErrorTransactionNotFound, "/IBooklet/PaymentErrorTransactionNotFound"},
            {PaymentReturnCode.PaymentErrorKeyMismatch, "/IBooklet/PaymentErrorKeyMismatch"},
            {PaymentReturnCode.PaymentSuccess, "/IBooklet/BookingConfirm"},
            {PaymentReturnCode.PaymentMobileSuccess, "/Mobile/BookingConfirm"},
            {PaymentReturnCode.PaymentSuccessMessageNotSent, "/IBooklet/PaymentSuccessMessageNotSent"},
            {PaymentReturnCode.PaymentSuccessNotAuthed, "/IBooklet/PaymentSuccessNotAuthed"},
            {PaymentReturnCode.PaymentSuccessAbort, "/IBooklet/PaymentSuccessAbort"},
            {PaymentReturnCode.PaymentSuccessRejected, "/IBooklet/PaymentSuccessRejected"},
            {PaymentReturnCode.PaymentSuccessError, "/IBooklet/PaymentSuccessError"},
            {PaymentReturnCode.PaymentErrorTransactionUpdateError, "/IBooklet/PaymentErrorTransactionUpdateError"},
            {PaymentReturnCode.PaymentErrorNotAuthenticated, "/IBooklet/PaymentErrorNotAuthenticated"}
        };

        public static string Url(PaymentReturnCode returnCode)
        {
            return System.Configuration.ConfigurationManager.AppSettings["NotificationUrl"] + ReturnUrLs[returnCode];
        }

        public static string BuildPaymentRequest(string transactionType, string strVendorTxCode,
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
            strPost = strPost + "&TxType=" + transactionType;

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

            var connectTo = SageConfig.StrConnectTo;

            return strPost;
        }

        #region Payment Notification

        //public static Dictionary<string, string> PaymentNotification(HttpRequestBase Request, bool IsMobile)
        //{
        //    //values for our response (Part 4)
        //    var notificationStatus = string.Empty;
        //    var notificationRedirectURL = string.Empty;
        //    var notificationStatusDetail = string.Empty;

        //    //values in SagePay's request (Part 3)
        //    var strVPSProtocol = string.Empty;
        //    var strTxType = string.Empty;
        //    var strVendorTxCode = string.Empty;
        //    var strVPSTxId = string.Empty;
        //    var strStatus = string.Empty;
        //    var strStatusDetail = string.Empty;
        //    long lngTxAuthNo = 0;
        //    var strAVSCV2 = string.Empty;
        //    var strAddressResult = string.Empty;
        //    var strPostCodeResult = string.Empty;
        //    var strCV2Result = string.Empty;
        //    var intGiftAid = 0;
        //    var str3DSecureStatus = string.Empty;
        //    var strCAVV = string.Empty;
        //    var strAddressStatus = string.Empty;
        //    var strPayerStatus = string.Empty;
        //    var strCardType = string.Empty;
        //    var strLast4Digits = string.Empty;
        //    var strDeclineCode = string.Empty;
        //    var strExpiryDate = string.Empty;
        //    var strFraudResponse = string.Empty;
        //    var strBankAuthCode = string.Empty;
        //    var strVPSSignature = string.Empty;

        //    var strSecurityKey = string.Empty;

        //    var strMessage = string.Empty;

        //    var UserID = Guid.Empty;

        //    try
        //    {
        //        if (Request.Form == null)
        //        {
        //            notificationStatus = "INVALID";
        //            notificationStatusDetail = "Parameters were not included in the request.";
        //            notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorNoParameters); // "/IBooklet/PaymentErrorNoParameters";
        //        }
        //        else
        //        {
        //            strVendorTxCode = Request.Form["VendorTxCode"] ?? string.Empty;
        //            strStatus = Request.Form["Status"] ?? string.Empty;
        //            strVPSTxId = Request.Form["VPSTxId"] ?? string.Empty;


        //            //Using the VPSTxId and VendorTxCode, we can retrieve our SecurityKey from our database **
        //            //This enables us to validate the POST to ensure it came from the Sage Pay Systems **

        //            //Step 1 - Check that an order exists in the database
        //            strSecurityKey = PaymentTransactionBLL.PaymentOrderLogGetSecurityKey(strVendorTxCode, strVPSTxId);

        //            if (strSecurityKey.Length == 0)
        //            {
        //                notificationStatus = "INVALID";
        //                notificationStatusDetail = "Unable to find the transaction in our database.";
        //                notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorTransactionNotFound); // "/IBooklet/PaymentErrorTransactionNotFound";
        //            }
        //            else
        //            {
        //                //We've found the order in the database, so now we can validate the message
        //                //Now get the VPSSignature value from the POST, and the StatusDetail in case we need it

        //                strVPSSignature = Request.Form["VPSSignature"] ?? string.Empty;
        //                strStatusDetail = Request.Form["StatusDetail"] ?? string.Empty;


        //                // Retrieve the other fields, from the POST if they are present
        //                strVPSProtocol = Request.Form["VPSProtocol"] ?? string.Empty;
        //                strTxType = Request.Form["TxType"] ?? string.Empty;


        //                if (Request.Form["TxAuthNo"] != null)
        //                {
        //                    long result = 0;

        //                    if (long.TryParse(Request.Form["TxAuthNo"], out result))
        //                    {
        //                        lngTxAuthNo = long.Parse(Request.Form["TxAuthNo"]);
        //                    }
        //                }

        //                strAVSCV2 = Request.Form["AVSCV2"] ?? string.Empty;
        //                strAddressResult = Request.Form["AddressResult"] ?? string.Empty;
        //                strPostCodeResult = Request.Form["PostCodeResult"] ?? string.Empty;
        //                strCV2Result = Request.Form["CV2Result"] ?? string.Empty;

        //                if (Request.Form["GiftAid"] != null)
        //                {
        //                    intGiftAid = Convert.ToInt32(Request.Form["GiftAid"]);
        //                }

        //                str3DSecureStatus = Request.Form["3DSecureStatus"] ?? string.Empty;
        //                strCAVV = Request.Form["CAVV"] ?? string.Empty;
        //                strAddressStatus = Request.Form["AddressStatus"] ?? string.Empty;
        //                strPayerStatus = Request.Form["PayerStatus"] ?? string.Empty;
        //                strCardType = Request.Form["CardType"] ?? string.Empty;
        //                strLast4Digits = Request.Form["Last4Digits"] ?? string.Empty;
        //                strDeclineCode = Request.Form["DeclineCode"] ?? string.Empty;
        //                strExpiryDate = Request.Form["ExpiryDate"] ?? string.Empty;
        //                strFraudResponse = Request.Form["FraudResponse"] ?? string.Empty;
        //                strBankAuthCode = Request.Form["BankAuthCode"] ?? string.Empty;

        //                //Now we rebuilt the POST message, including our security key, and use the MD5 Hash
        //                //component that ships with the kit to create our own signature to compare with
        //                //the contents of the VPSSignature field in the POST.

        //                /*
        //                 MD5 signature of the concatenation of the values of: 
        //                 {VPSTxId } + VendorTxCode + Status + TxAuthNo + VendorName + AVSCV2 + SecurityKey + AddressResult + 
        //                 PostCodeResult + CV2Result + GiftAid + 3DSecureStatus + CAVV + AddressStatus + PayerStatus + CardType + Last4Digits + 
        //                 DeclineCode + ExpiryDate + FraudResponse + BankAuthCode 
        //                 */
        //                strMessage = strVPSTxId + strVendorTxCode + strStatus + lngTxAuthNo + SageConfig.StrVendorName + strAVSCV2 + strSecurityKey + strAddressResult +
        //                             strPostCodeResult + strCV2Result + intGiftAid + str3DSecureStatus + strCAVV + strAddressStatus + strPayerStatus + strCardType + strLast4Digits +
        //                             strDeclineCode + strExpiryDate + strFraudResponse + strBankAuthCode;

        //                var strMySignature = string.Empty;

        //                strMySignature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strMessage, "MD5");


        //                var sagePayBypassEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["SagePayBypassEnabled"] == "True";


        //                if ((strMySignature != strVPSSignature) && (str3DSecureStatus == "NOTAUTHED"))
        //                {
        //                    notificationStatus = "NOTAUTHED";
        //                    notificationStatusDetail = "3D Secure NOT Authenticated";
        //                    notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorNotAuthenticated);
        //                }
        //                else if ((strMySignature == strVPSSignature) && (str3DSecureStatus == "ERROR"))
        //                {
        //                    notificationStatus = "ERROR";
        //                    notificationStatusDetail = "3D Secure NOT Authenticated";
        //                    notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorNotAuthenticated);
        //                }
        //                else if ((strMySignature == strVPSSignature) && (str3DSecureStatus == "ATTEMPTONLY"))
        //                {
        //                    notificationStatus = "ATTEMPTONLY";
        //                    notificationStatusDetail = "3D Secure NOT Authenticated";
        //                    notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorNotAuthenticated);
        //                }
        //                else if ((strMySignature == strVPSSignature) && (str3DSecureStatus == "INCOMPLETE"))
        //                {
        //                    notificationStatus = "INCOMPLETE";
        //                    notificationStatusDetail = "3D Secure NOT Authenticated";
        //                    notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorNotAuthenticated);
        //                }
        //                //We can now compare our MD5 Hash signature with that from Server
        //                else if ((strMySignature != strVPSSignature) && !sagePayBypassEnabled)
        //                {
        //                    //If the signatures DON'T match, we should mark the order as tampered with, and
        //                    // send back a Status of INVALID and failure page RedirectURL

        //                    notificationStatus = "INVALID";
        //                    notificationStatusDetail = "Cannot match the MD5 Hash. Order might be tampered with.";
        //                    notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorNotAuthenticated);
        //                }
        //                else
        //                {
        //                    //Great, the signatures DO match, so we can update the database and redirect the user appropriately

        //                    /*--------------Insert Transaction Details------------------------*/

        //                    var userName = string.Empty;
        //                    var lstACC = new List<AccountBO>();

        //                    try
        //                    {
        //                        //Find out if this notification status is already OK (for not sending emails again etc..)
        //                        var isNotificationStatusOk = PaymentTransactionBLL.isPaymentNotificationOk(strVPSTxId);


        //                        //var objPaymentOrder = new PaymentOrderLogBO
        //                        //{
        //                        //    VPSProtocol = strVPSProtocol,
        //                        //    TxType = strTxType,
        //                        //    VendorTxCode = strVendorTxCode,
        //                        //    VPSTxId = strVPSTxId,
        //                        //    NotificationStatus = strStatus,
        //                        //    NotificationStatusDetail = strStatusDetail,
        //                        //    TxAuthNo = lngTxAuthNo,
        //                        //    AVSCV2 = strAVSCV2,
        //                        //    AddressResult = strAddressResult,
        //                        //    PostCodeResult = strPostCodeResult,
        //                        //    CV2Result = strCV2Result,
        //                        //    GiftAid = intGiftAid,
        //                        //    ThreeDSecureStatus = str3DSecureStatus,
        //                        //    CAVV = strCAVV,
        //                        //    AddressStatus = strAddressStatus,
        //                        //    PayerStatus = strPayerStatus,
        //                        //    CardType = strCardType,
        //                        //    Last4Digits = strLast4Digits,
        //                        //    VPSSignature = strVPSSignature,
        //                        //    NotificationTime = DateTime.Now
        //                        //};


        //                        // increments the Attempts field aswell
        //                        //lstACC = PaymentTransactionBLL.UpdatePaymentorderLog(objPaymentOrder);

        //                        var paymentAttemptNo = PaymentTransactionBLL.getFullPaymentTransactionLogDetails(strVPSTxId).Attempts;


        //                        foreach (AccountBO objAcc in lstACC)
        //                        {
        //                            UserID = objAcc.UserID;
        //                            userName = objAcc.UserName;
        //                            break;
        //                        }

        //                        // Now reply to Server to let the system know we've received the Notification POST
        //                        // Now decide where to redirect the customer

        //                        //LogToFile("Info", "strStatus");
        //                        //LogToFile("Info", strStatus);

        //                        switch (strStatus)
        //                        {
        //                            /**************** Check the Payment status and redirect to Success/Failure Page *********/
        //                            case "OK":
        //                            case "AUTHENTICATED":
        //                            case "REGISTERED":
        //                                try
        //                                {
        //                                    if (!isNotificationStatusOk)
        //                                    {
        //                                        var bookingsBll = new iBooklet_BookingsBLL();
        //                                        var checkoutHelper = new iBooklet_CheckoutHelper(new iBooklet_CheckoutBasketBLL());

        //                                        int? bookID = null;

        //                                        // Get all Basket bookings
        //                                        var transactionBookingsList = bookingsBll.BookingPaymentOrderSelect(UserID, strVPSTxId);

        //                                        // Check no BookingFee failed to calculate
        //                                        if (transactionBookingsList.Any(b => b.BookingFees < 0))
        //                                        {
        //                                            throw new iBooklet_FeeCalculationError();
        //                                        }

        //                                        // Get Bookings that will be moved to BOOKED status (i.e. for Courses which have places available)
        //                                        var movingToBookedList = transactionBookingsList.Where(b => b.PlacesAvailable > 0).ToList();

        //                                        // if only ONE booking has been processed then get the BookID of it and pass it in to be processed
        //                                        // this will be used in confirming Provisional Bookings but also on single booking transactions.
        //                                        // If there are more than one booking involved then we can assume its a Basket checkout and leave BookID as
        //                                        // null and process the entire Basket
        //                                        if (movingToBookedList.Count == 1)
        //                                        {
        //                                            iBooklet_BookingBO singleBooking = movingToBookedList[0];

        //                                            // if the single booking we found is a Provisional booking then change the Checkout Worker to the Provisional one
        //                                            // otherwise leave as the Basket worker
        //                                            if (singleBooking.BookingStatus == 9)
        //                                            {
        //                                                checkoutHelper.CheckoutWorker = new iBooklet_CheckoutProvisionalBLL();
        //                                                bookID = singleBooking.BookID;
        //                                            }
        //                                        }

        //                                        // Get Bookings that will be moved to WAITING LIST status (i.e. for Courses which have NO places available)
        //                                        List<iBooklet_BookingBO> movingToWaitingListList =
        //                                            checkoutHelper.CheckoutWorker.BookingsSelect(bookID, UserID)
        //                                                .Where(b => b.PlacesAvailable == 0).ToList();

        //                                        // Get Bookings that will be moved to APPLICATION PENDING status (i.e. for Courses which have PreEvaluationType = "Application")
        //                                        List<iBooklet_BookingBO> movingToApplicationPendingList =
        //                                            checkoutHelper.CheckoutWorker.BookingsSelect(bookID, UserID)
        //                                                .Where(b => b.PreEvaluationType == "Application").ToList();

        //                                        // Complete the booking by updating statuses and sending emails
        //                                        checkoutHelper.CompleteBookings(UserID, movingToBookedList,
        //                                            movingToWaitingListList, movingToApplicationPendingList, strVPSTxId);
        //                                    }

        //                                    notificationStatus = "OK";
        //                                    notificationStatusDetail = "";
        //                                    // detect if isMobile and sent to /Mobile/BookingoConfirm else sent to /IBooklet/BookingConfirm
        //                                    notificationRedirectURL = IsMobile
        //                                        ? Url(PaymentReturnCode.PaymentMobileSuccess)
        //                                        : Url(PaymentReturnCode.PaymentSuccess);
        //                                }
        //                                catch
        //                                {
        //                                    notificationStatus = "OK";
        //                                    notificationStatusDetail = "Email/SMS not sent.";
        //                                    notificationRedirectURL = Url(PaymentReturnCode.PaymentSuccessMessageNotSent);
        //                                }

        //                                break;
        //                            case "NOTAUTHED":
        //                                notificationStatus = "OK";
        //                                notificationStatusDetail = "";
        //                                notificationRedirectURL = Url(PaymentReturnCode.PaymentSuccessNotAuthed);
        //                                break;
        //                            case "ABORT":
        //                                {
        //                                    // If Number of Attempts to complete payment is greater than 1
        //                                    // i.e. The first attempt timed out and Sage is trying again
        //                                    // then we cleanup the transaction log and email the user
        //                                    if (paymentAttemptNo > 1)
        //                                    {
        //                                        //Process Clean Up and Email User
        //                                       // PaymentHelper.ProcessTransactionErrorCleanup(UserID, strVPSTxId, "ABORT");
        //                                    }

        //                                    notificationStatus = "OK";
        //                                    notificationStatusDetail = "";
        //                                    notificationRedirectURL = Url(PaymentReturnCode.PaymentSuccessAbort);
        //                                    break;
        //                                }
        //                            case "REJECTED":
        //                                notificationStatus = "OK";
        //                                notificationStatusDetail = "";
        //                                notificationRedirectURL = Url(PaymentReturnCode.PaymentSuccessRejected);
        //                                break;
        //                            case "ERROR":
        //                                notificationStatus = "OK";
        //                                notificationStatusDetail = "";
        //                                notificationRedirectURL = Url(PaymentReturnCode.PaymentSuccessError);
        //                                break;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                       // NesExceptionManager.getInstance().HandleException(ex);

        //                        notificationStatus = "ERROR";
        //                        notificationStatusDetail = "Error updating transaction log with payment details.";
        //                        notificationRedirectURL = Url(PaymentReturnCode.PaymentErrorTransactionUpdateError);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(ex);
        //    }

        //    var dict = new Dictionary<string, string>
        //    {
        //        {"Status", notificationStatus},
        //        {"StatusDetail", notificationStatusDetail},
        //        {"RedirectURL", notificationRedirectURL + "?userID=" + UserID}
        //    };

        //    return dict;
        //}

        #endregion
    }
}