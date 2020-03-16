using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Common.Helpers;
using Newtonsoft.Json;
using Ninject;
using Ninject.Extensions.Conventions;
using PaymentGateway.Controllers;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Common;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.Helpers;
using PaymentGateway.Util.Ninject;
using Rhino.Mocks;
using Service;
using Xunit;


namespace WebApi.Tests.Controllers
{

    public class ValuesControllerTest
    {
        #region members

        private IDbContext _dbContext;
        private IService<PaymentOrder> _paymentOrderService;
        private IService<PaymentTransactionLog> _paymentTransactionLogService;
        private ISageService _sageService;

        #endregion

        #region constructos

        public ValuesControllerTest()
        {
            SetUp();
        }

        #endregion

        #region tests

        /// <summary>
        /// Test mock up controller exists
        /// </summary>
        [Fact]
        public void GetController_Returns_ValuesController()
        {
            // Arrange
            var controller = GetController();


            // Act



            // Assert
            Assert.IsType<ValuesController>(controller);
        }

        [Fact]
        public void RequestToken_ReturnsValidToken()
        {
            // Arrange
            var controller = new ValuesController();

            // Act
            var result = controller.RequestToken("Portal");
            var decryptedString = EncryptionHelper.Decrypt(result);

            // Assert
            Assert.IsType<string>(result);
            Assert.DoesNotContain("Portal", result);
            Assert.Contains("Portal", decryptedString);
        }

        [Fact]
        public void RequestVendorTxCode_ReturnsValidCode()
        {
            // Arrange
            var controller = GetController();


            // mock service calls
            _sageService.Stub(x => x.GenerateVendorTxCode()).IgnoreArguments().Return("VendorTxCode");


            // Act
            var result = controller.RequestVendorTxCode(1);


            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("VendorTxCode", result);
        }

        [Fact]
        public void SendPaymentRequest_WithoutHeader_Returns_Error()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = controller.SendPaymentRequest(1);

            // Assert
            Assert.True(result.ContainsKey("Status"));
            Assert.True(result.ContainsValue("Error"));
            Assert.True(result.ContainsValue("Payment Details Missing"));
        }

        [Fact]
        public void SendPaymentRequest_Returns_Success()
        {
            // Arrange
            var controller = GetController();
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
                VendorTxCode = "VendorTxCode",
                TotalBookingFee = "200.00",
                NotificationUrl = ConfigurationHelper.NotificationUrl,
                BillingDetails = addressDetails,
                DeliveryDetails = addressDetails,
                CustomerEmail = "joe.bloggs@gmail.com",
                Basket = "1%3aCPD+Connect++Mindfulness-Based+Stress+Reduction+(MBSR)+-+9+week+course%3a%3a%3a%3a%3a200.00",
                IsMobile = false
            };


            // add paymentInfo to HEADER
            var jsonObj = JsonConvert.SerializeObject(paymentInfo);
            controller.Request.Headers.Add("X-PaymentInfo", jsonObj);

            // mock service calls
            var nextUrl =  "http://fake.wesite.com/Controller/NotificationURL?isMobile=False";

            var response = "VPSProtocol=2.30" + "\r\nVPSTxId=123456789" + "\r\nSecurityKey=**DUMMYKEY\r\nNextURL=" +
                           nextUrl + "\r\nStatusDetail=0000 : The Authorisation was Successful.\r\nStatus=OK\r\n";

            var responseDict = new Dictionary<string, object> {{"success", "true"}, { "Status", "Ok" }, { "StatusDetail", "Status Detail" }, { "VPSTxId", "123456789" } };

            _sageService.Stub(x => x.SendRequest("","","")).IgnoreArguments().Return(response);
            _sageService.Stub(x => x.PrepareNextUrl(false, ref response))
                .Return(responseDict);

            _paymentOrderService.Stub(x => x.Add(new PaymentOrder())).IgnoreArguments();
            _paymentTransactionLogService.Stub(x => x.Add(new PaymentTransactionLog())).IgnoreArguments();

            _dbContext.Stub(x => x.SaveChanges()).Return(1);

            // Act
            var result = controller.SendPaymentRequest(1);

            // Assert
            Assert.Equal(responseDict, result);
        }
        #endregion

        #region private methods

        private void SetUp()
        {
            // create mock services
            _paymentOrderService = MockRepository.GenerateStub<IService<PaymentOrder>>();
            _paymentTransactionLogService = MockRepository.GenerateStub<IService<PaymentTransactionLog>>();
            _sageService = MockRepository.GenerateStub<ISageService>();
            _dbContext = MockRepository.GenerateStub<IDbContext>();
        }

        /// <summary>
        /// Create an OffTheRunApiController
        /// </summary>
        /// <returns></returns>
        private ValuesController GetController()
        {
            var controller = new ValuesController(_paymentOrderService, _paymentTransactionLogService, _sageService, _dbContext);
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://local.paymentgateway.scot.nhs.uk/Values");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "id", "1" }, { "controller", "" } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, config);
            controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, routeData);

            return controller;
        }

        #endregion
    }
}
