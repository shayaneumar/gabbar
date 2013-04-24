/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using ArtOfTest.WebAii.Controls.HtmlControls;
using BuildingSecurity.WebApp.AcceptanceTests.BrowserDriver;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    public class AlarmDisplayOptionInput
    {
        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        public string Color { get; set; }
        public string Sound { get; set; }
    }

    [Binding]
    public class AlarmDisplayOptions
    {
        private readonly Dictionary<string, string> _colorLookup = new Dictionary<string, string>
                                                                       {
                                                                           { "red", "#E3150E" },
                                                                           { "orange", "#FE973F" },
                                                                           { "yellow", "#F3F150" },
                                                                           { "blue", "#36B6F8" },
                                                                           { "green", "#32DE9B" },
                                                                           { "clear", "" },
                                                                       };

        [Given(@"I enter configure alarms like this:")]
        public void GivenIEnterConfigureAlarmsLikeThis(Table table)
        {
            var options = table.CreateSet<AlarmDisplayOptionInput>().ToList();
            var allNumericInputs = WebBrowser.CurrentBrowser.Find.AllByTagName<HtmlInputNumber>("input").ToList();
            var allColorInputs = WebBrowser.CurrentBrowser.Find.AllByTagName<HtmlInputHidden>("input").Where(IsColorControl).ToList();//This is a bad way of finding this
            var allSoundInputs = WebBrowser.CurrentBrowser.Find.AllByTagName<HtmlSelect>("select").ToList();


            for (int i = 0; i < options.Count; i++ )
            {
                var option = options[i];
                var upperLimit = allNumericInputs[i];
                var color = allColorInputs[i];
                var sound = allSoundInputs[i];
                upperLimit.UserSetInputValue(option.UpperLimit);
                color.UserSetInputValue(_colorLookup[option.Color.ToLower()]);
                sound.UserSelectByText(option.Sound);
            }
        }

        private static bool IsColorControl(HtmlInputControl control)
        {
            var dataType = control.Attributes.FirstOrDefault(x => x.Name == "data-type");
            return dataType != null && dataType.Value == "color";
        }
    }
}
