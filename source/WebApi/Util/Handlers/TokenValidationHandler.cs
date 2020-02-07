using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Util.Handlers
{
    public class TokenValidationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var sessionId = "";
            var sessionCookie = request.Headers.GetCookies("ASP.NET_SessionID").FirstOrDefault();
            if (sessionCookie != null)
            {
                sessionId = sessionCookie["ASP.NET_SessionID"].Value;
            }

            if (!request.Headers.Contains("Authorization-Token"))
            {
                var msg = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Missing Authorization-Token")
                };
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(msg);
                return tcs.Task;
            }


            var token = request.Headers.GetValues("Authorization-Token").FirstOrDefault();


            try
            {
                var authorisedSession = RSAClass.Decrypt(token);
                if (authorisedSession != sessionId)
                {
                    var msg = new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("Unauthorized session")
                    };
                    var tcs = new TaskCompletionSource<HttpResponseMessage>();
                    tcs.SetResult(msg);
                    return tcs.Task;

                }
            }
            catch (RSAClass.RSAException)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Error encountered while attempting to process authorization token")
                };
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(msg);
                return tcs.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}