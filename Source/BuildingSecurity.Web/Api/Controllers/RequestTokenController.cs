/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Linq;
using System.Web.Helpers;
using System.Web.Http;
using System.Xml.Linq;

namespace BuildingSecurity.Web.Api.Controllers
{
    public class RequestTokenController : ApiController
    {
        [AllowAnonymous]
        public string Get()
        {
            XDocument xmlDoc = XDocument.Parse(AntiForgery.GetHtml().ToHtmlString());

            string token = (from tutorial in xmlDoc.Descendants("input")
                            where (string)tutorial.Attribute("name") == "__RequestVerificationToken"
                            select tutorial.Attribute("value")).First().Value;
            
            return token;
        }
    }
}