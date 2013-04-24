/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Web.App.Models
{
    public class CompanyLogoModel
    {
        public CompanyLogoModel(string messageKey)
        {
            MessageKey = messageKey;
        }

        public string MessageKey { get; private set; }
    }
}
