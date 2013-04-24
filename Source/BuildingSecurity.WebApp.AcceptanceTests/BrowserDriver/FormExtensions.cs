/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;

namespace BuildingSecurity.WebApp.AcceptanceTests.BrowserDriver
{
    public static class FormExtensions
    {
        public static void UserSetInputValue(this HtmlControl input, string newValue)
        {
            input.Focus();
            input.InvokeEvent(ScriptEventType.OnFocus);
            input.SetValue("value", newValue);
            input.InvokeEvent(ScriptEventType.OnBlur);
            input.InvokeEvent(ScriptEventType.OnChange);
        }

        public static void UserSelectByText(this HtmlSelect input, string newValue)
        {
            input.Focus();
            input.InvokeEvent(ScriptEventType.OnFocus);
            input.SelectByText(newValue);
            input.InvokeEvent(ScriptEventType.OnBlur);
            input.InvokeEvent(ScriptEventType.OnChange);
        }
    }
}
