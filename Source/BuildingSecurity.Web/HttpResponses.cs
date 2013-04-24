
using System.Net;
using System.Net.Http;

namespace BuildingSecurity.Web
{
    public static class HttpResponses
    {
        private static readonly HttpResponseMessage HttpResponseMessageNotFound = new HttpResponseMessage(HttpStatusCode.NotFound);
        private static readonly HttpResponseMessage HttpResponseMessageUnauthorized = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        private static readonly HttpResponseMessage HttpResponseMessageNotImplemented = new HttpResponseMessage(HttpStatusCode.NotImplemented);
        private static readonly HttpResponseMessage HttpResponseMessageBadRequest = new HttpResponseMessage(HttpStatusCode.BadRequest);

        public static HttpResponseMessage NotFoundMessage
        {
            get { return HttpResponseMessageNotFound; }
        }

        public static HttpResponseMessage UnauthorizedMessage
        {
            get { return HttpResponseMessageUnauthorized; }
        }

        public static HttpResponseMessage NotImplementedMessage
        {
            get { return HttpResponseMessageNotImplemented; }
        }

        public static HttpResponseMessage BadRequestMessage
        {
            get { return HttpResponseMessageBadRequest; }
        }
    }
}
