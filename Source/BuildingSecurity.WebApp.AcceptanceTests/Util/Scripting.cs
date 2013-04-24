/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Globalization;
using Newtonsoft.Json.Linq;

namespace BuildingSecurity.WebApp.AcceptanceTests.Util
{
    public static class Scripting
    {
        private static JObject CreateScript()
        {
            return new JObject {{"events", new JArray()}};
        }

        private static JObject ClearCases()
        {
            return new JObject { { "at", "-0.0:0:0.1" }, { "name", "ClearCases" }, { "value", "" } };
        }

        private static JObject CreateCase(string title, string createdBy, string status)
        {
            var caseDetails = new JObject {{"Title", title}};
            if (createdBy != null) caseDetails.Add("CreatedBy", createdBy);
            if (status != null) caseDetails.Add("Status", status);

            var createCase = new JObject { { "at", "-0.0:0:0.0" }, { "name", "CreateCase" }, { "value", caseDetails } };

            return createCase;
        }

        private static JObject CreateCaseNote(string caseGuidId, string text)
        {
            var caseNoteData = new JObject { { "CaseGuidId", caseGuidId }, { "Text", text } };
            var createCase = new JObject { { "at", "-0.0:0:0.0" }, { "name", "CreateCaseNote" }, { "value", caseNoteData } };

            return createCase;
        }

        public static string GetScriptCreateCase(string title, string createdBy = "simulator", string status = "open", bool clearCases = true)
        {
            JObject script = CreateScript();

            if(clearCases) ((JArray)script["events"]).Add(ClearCases());

            ((JArray)script["events"]).Add(CreateCase(title, createdBy, status));

            return script.ToString();
        }

        public static string GetScriptCreateCases(int caseCount)
        {
            JObject script = CreateScript();

            ((JArray)script["events"]).Add(ClearCases());

            for (int index = 1; index <= caseCount; index++)
            {
                ((JArray)script["events"]).Add(CreateCase("Case " + (index).ToString(CultureInfo.InvariantCulture), null, "open"));
            }

            return script.ToString();
        }

        public static string GetScriptCreateCaseNote(string caseId, string text)
        {
            JObject script = CreateScript();
            ((JArray)script["events"]).Add(CreateCaseNote(caseId, text));

            return script.ToString();
        }
    }
}
