using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using Newtonsoft.Json.Linq;
using IApiAuthorizationFilter = System.Web.Http.Filters.IAuthorizationFilter;

// ReSharper disable CheckNamespace
namespace System.Web.Mvc
// ReSharper restore CheckNamespace
{
    public class CsrfPreventionFilter : IAuthorizationFilter, IApiAuthorizationFilter
    {
        private readonly ConcurrentDictionary<string, TokenSelector> _tokenValidators = new ConcurrentDictionary<string, TokenSelector>();

        /// <summary>
        /// Extracts the RequestVerificationToken from a HttpRequest.
        /// If a token is not found in the request null is returned.
        /// </summary>
        delegate string TokenSelector(HttpRequestBase request);

        public CsrfPreventionFilter()
        {
            SetRequestTokenSelector(@"application/json", JsonTokenSelector);
            SetRequestTokenSelector(@"multipart/form-data", FormTokenSelector);
            SetRequestTokenSelector(@"application/x-www-form-urlencoded", FormTokenSelector);
        }

        public static string FormTokenSelector(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var rawValue = request.Form["__RequestVerificationToken"];
            return string.IsNullOrWhiteSpace(rawValue) ? null : rawValue;
        }

        public static string JsonTokenSelector(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            long startingPosition = request.InputStream.Position;

            var buffer = new byte[4096];
            int lastReadCount;
            int usedBytesInBuffer = 0;
            while ((lastReadCount = request.InputStream.Read(buffer, usedBytesInBuffer, buffer.Length - usedBytesInBuffer)) > 0)
            {
                usedBytesInBuffer += lastReadCount;
                if(usedBytesInBuffer == buffer.Length)
                {
                    Array.Resize(ref buffer,usedBytesInBuffer*2);
                }
            }
            Array.Resize(ref buffer, usedBytesInBuffer);//prune array
            var chars = request.ContentEncoding.GetChars(buffer);
            var content = new string(chars);

            request.InputStream.Position = startingPosition;//Leave stream how we found it


            var jsonRequest = JObject.Parse(content);
            return (string)jsonRequest["__RequestVerificationToken"];
        }

        public void SetRequestTokenSelector(string contentType, Func<HttpRequestBase,string> selector)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            if (selector == null) throw new ArgumentNullException("selector");

            _tokenValidators[contentType.ToUpperInvariant()] = new TokenSelector(selector);
        }

        public bool AllowMultiple
        {
            get { return true; }
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null) throw new ArgumentNullException("actionContext");
            if (continuation == null) throw new ArgumentNullException("continuation");
            if (!actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                Validate(new HttpContextWrapper(HttpContext.Current).Request);
            }
            return continuation();
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            if (!filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                Validate(filterContext.HttpContext.Request);
            }
        }

        private void Validate(HttpRequestBase request)
        {
            //GET and HEAD methods SHOULD NOT have the significance of taking an action other than retrieval.
            //Thus they are immune from CSRF (unless misused).
            if (@"GET".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase) ||
                @"HEAD".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var contentType = request.ContentType.ToUpperInvariant().Split(';')[0].Trim();
            TokenSelector tokenSelector;
            if (_tokenValidators.TryGetValue(contentType, out tokenSelector))
            {
                AntiForgery.Validate(GetTokenFromCookie(request), tokenSelector(request));
            }
            else
            {
                throw new HttpAntiForgeryException("No token selector registered for content-type=" + contentType);
            }
        }

        private static string GetTokenFromCookie(HttpRequestBase request)
        {
            var httpCookie = request.Cookies[AntiForgeryConfig.CookieName];
            return httpCookie != null ? httpCookie.Value : null;
        }
    }
}
