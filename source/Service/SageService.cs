using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace Service
{
    public class SageService : ISageService
    {
        #region private members



        #endregion

        #region constructors

        public SageService()
        {

        }

        #endregion

        #region methods

        /// <summary>
        /// Get the URL based on target environment [LIVE | TEST] & type of call
        /// </summary>
        /// <param name="environment">[TEST | LIVE]</param>
        /// <param name="strType">Action for Url</param>
        /// <returns></returns>
        public string GetSystemUrl(string environment, string strType)
        {
                var strSystemUrl = "";

                switch (environment)
                {
                    case "LIVE":
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

                        break;
                    case "TEST":
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

                        break;
                    default:
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

                        break;
                }

                return strSystemUrl;
        }

        /// <summary>
        /// Generate a Unique transaction code
        /// </summary>
        /// <returns></returns>
        public string GenerateVendorTxCode()
        {
            var strVendorTxCode = "";
            long i = 1;

            foreach (var b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            strVendorTxCode = $"{i - DateTime.Now.Ticks:x}";

            return strVendorTxCode;
        }

        /// <summary>
        /// Send the request to SagePay
        /// </summary>
        /// <param name="postQuery"></param>
        /// <param name="transactionTypeForUrl"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public string SendRequest(string postQuery, string transactionTypeForUrl, string environment)
        {
            var objUtfEncode = new UTF8Encoding();
            byte[] arrRequest = null;
            var objStreamReq = default(Stream);
            var objHttpRequest = default(HttpWebRequest);
            var objStreamRes = default(StreamReader);
            var objHttpResponse = default(HttpWebResponse);
            var systemUrl = GetSystemUrl(environment, transactionTypeForUrl);
            var objUri = new Uri(systemUrl);

            objHttpRequest = (HttpWebRequest)WebRequest.Create(objUri);
            objHttpRequest.KeepAlive = false;
            objHttpRequest.Method = "POST";

            objHttpRequest.ContentType = "application/x-www-form-urlencoded";

            arrRequest = objUtfEncode.GetBytes(postQuery);
            objHttpRequest.ContentLength = arrRequest.Length;
            objStreamReq = objHttpRequest.GetRequestStream();
            objStreamReq.Write(arrRequest, 0, arrRequest.Length);
            objStreamReq.Close();

            objHttpResponse = (HttpWebResponse)objHttpRequest.GetResponse();
            objStreamRes = new StreamReader(objHttpResponse.GetResponseStream(), Encoding.ASCII);

            var response = objStreamRes.ReadToEnd();
            objStreamRes.Close();

            return response;
        }

        public Dictionary<string, object> PrepareNextUrl(bool sagePayBypassEnabled, ref string response)
        {
            response = response.Replace("\r\n", ";'").Replace("'", "");
            var arr = new string[8];
            arr = response.Split(';');
            var sageResponseDict = new Dictionary<string, object>();

            var vpsTxId = string.Empty;
            var statusDetail = string.Empty;
            var securityKey = string.Empty;
            var status = string.Empty;

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

                    if (sagePayBypassEnabled)
                    {
                        Object obj = (Object)(subArr[1] + "=False");
                        sageResponseDict.Add(subArr[0], obj);
                    }
                    else
                    {
                        Object obj = (Object)(subArr[1] + "=" + vpsTxId);
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
