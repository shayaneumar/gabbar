/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BuildingSecurity.WebApp.AcceptanceTests.Util
{
    public static class Simulator
    {
        public static void ExecuteScript(string scriptString)
        {
            object obj = new {script = scriptString};

            IRestResponse response = GetResponse(EnvironmentConfiguration.WebUiAddress, "api/v1/Simulator", Method.POST, obj, "script", "script", true);

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new Exception("Response.StatusCode = " + response.StatusDescription +
                                    "; The specified user may not have permission to execute simulations");
            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new Exception("Response.StatusCode = " + response.StatusDescription); // Internal Server Error            

            //Give the simulator time to run the simulation
            Thread.Sleep(1000);
        }

        public static void ExecuteRequest(params object[] events)
        {
            ExecuteScript(JsonConvert.SerializeObject(new {events}));
        }

        private static IRestResponse Login(RestClient client, string username, string password, bool ignoreErrors)
        {
            // Login
            var loginRequest = new RestRequest(Method.POST);
            loginRequest.AddParameter("UserName", username);
            loginRequest.AddParameter("Password", password);

            // Ignore SSL Policy Errors
            if (ignoreErrors)
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            var loginResponse = ExecuteRequest(client, loginRequest);
            if (loginResponse.ResponseStatus != ResponseStatus.Completed)
                throw new Exception("Login failed: " + loginResponse.ErrorMessage);

            // Get RequestToken
            var tokenRequest = new RestRequest("api/v1/RequestToken", Method.GET);
            var tokenResponse = ExecuteRequest(client, tokenRequest);

            return tokenResponse;
        }

        public static IRestResponse GetResponse(string url, string resource, Method method, object obj, string username, string password, bool ignoreErrors)
        {
            // Create RestClient
            var client = new RestClient(url) { CookieContainer = new CookieContainer() };

            // Login and retrieve tokenResponse
            var tokenResponse = Login(client, username, password, ignoreErrors);

            // Execute Simulator script
            var request = new RestRequest(resource, method) { RequestFormat = DataFormat.Json };

            // Append __RequestVerificationToken to a new object
            object body = (obj ?? new object()).AddProperty("__RequestVerificationToken", (string)JToken.Parse(tokenResponse.Content));

            request.AddBody(body);

            return ExecuteRequest(client, request);
        }

        /// <summary>
        /// Wrap RestClient.Execute to ensure a RestResponse (not null) is returned.
        /// This does not appear to be possible in any way; this method is mainly to simplify otherwise required checks for null (Warnings).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static IRestResponse ExecuteRequest(RestClient client, RestRequest request)
        {
            var response = client.Execute<object>(request) as RestResponse<object>;
            if (response == null) throw new Exception("RestClient returned a null Resonse object"); // This is impossible
            return response;
        }
    }
}
