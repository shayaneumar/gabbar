/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Web.App.Models
{
    public class ReportsErrorModel
    {
        public ReportsErrorModel(string errorMessage, bool canViewSystemSetup)
        {
            ErrorMessage = errorMessage;
            CanViewSystemSetup = canViewSystemSetup;
        }

        public string ErrorMessage { get; private set; }
        public bool CanViewSystemSetup { get; private set; }
    }
}

