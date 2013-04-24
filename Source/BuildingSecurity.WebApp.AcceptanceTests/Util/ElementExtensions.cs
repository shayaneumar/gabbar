/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.ObjectModel;

namespace BuildingSecurity.WebApp.AcceptanceTests.Util
{
    public static class ElementExtensions
    {
        public static void Click(this Element element)
        {
            new HtmlControl(element).Click();
        }
    }
}
