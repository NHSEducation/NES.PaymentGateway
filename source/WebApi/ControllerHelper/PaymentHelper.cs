using PaymentGateway.Util.Sage;

namespace PaymentGateway.ControllerHelper
{
    public static class PaymentHelper
    {
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
    }
}