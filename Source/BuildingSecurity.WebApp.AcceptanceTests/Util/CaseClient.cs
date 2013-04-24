/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Collections;
using Newtonsoft.Json;
using RestSharp;

namespace BuildingSecurity.WebApp.AcceptanceTests.Util
{
    public class CaseClient
    {
        public static IEnumerable<Case> GetCaseListFor(string user)
        {
            string login = GetLoginFor(user);
            IRestResponse response = Simulator.GetResponse(EnvironmentConfiguration.WebUiAddress, "api/v1/Cases", Method.GET, null,
                                                           login, login, true);
            var cases = JsonConvert.DeserializeObject<DataChunk<Case>>(response.Content);
            return cases.Data;
        }

        public static Case CreateCase(string caseTitle, string user)
        {
            string login = GetLoginFor(user);
            var caseModel = new {title = caseTitle};
            IRestResponse response = Simulator.GetResponse(EnvironmentConfiguration.WebUiAddress, "api/v1/Cases", Method.POST, caseModel, login, login, true);

            string caseUrl = string.Format("api/v1/Cases/{0}", JsonConvert.DeserializeObject<string>(response.Content));
            response = Simulator.GetResponse(EnvironmentConfiguration.WebUiAddress, caseUrl, Method.GET, null, login, login, true);

            return JsonConvert.DeserializeObject<Case>(response.Content);
        }

        private static string GetLoginFor(string user)
        {
            return string.Format("{0}ViaWebApi", user);
        }
    }
}
