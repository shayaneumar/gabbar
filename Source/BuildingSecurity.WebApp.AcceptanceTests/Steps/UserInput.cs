/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using ArtOfTest.WebAii.Controls.HtmlControls;
using BuildingSecurity.WebApp.AcceptanceTests.BrowserDriver;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class UserInput
    {
        [Given("I have entered (.*) in the (.*) field")]
        public void GivenIHaveEnteredSomeTestInSomeField(string text, string id)
        {
            EnterText<HtmlInputControl>(id, text);
        }

        [When(@"I clear the (.*) field")]
        public void WhenIClearTheCaseTitleField(string id)
        {
            EnterText<HtmlInputControl>(id, "");
        }

        [When("I press the (.*) button")]
        public void WhenIPressSomeButton(string buttonName)
        {
            PressButton(buttonName);
        }

        [When("I click the (.*) link in the site navigation menu")]
        public void WhenIClickASiteNavigationLink(string linkname)
        {
            var navMenu = WebBrowser.CurrentBrowser.Find.ByExpression<HtmlControl>("class=nav");
            var link = navMenu.Find.ByExpression<HtmlControl>("TagName=a", "InnerText=" + linkname);
            Assert.IsNotNull(link, "Could not find a link with name=" + linkname);
            link.Click();
        }

        [Given(@"I have reset to default")]
        public void GivenIHaveResetToDefault()
        {
            WhenIPressSomeButton("restore defaults");
        }

        public static HtmlControl GetAcceptButton(string containerId)
        {
            var titleContainer = new HtmlControl(WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + containerId));
            titleContainer.Find.ByExpression("class=~btn-accept").Wait.ForExists(5000);
            return titleContainer.Find.ByExpression<HtmlControl>("class=~btn-accept");
        }

        public static void EnterText<T>(string id, string text) where T : HtmlControl, new()
        {
            var field = WebBrowser.CurrentBrowser.Find.ByName<T>(id);
            field = field ?? WebBrowser.CurrentBrowser.Find.ById<T>(id);
            Assert.IsNotNull(field, "Could not find a field with name or id=" + id);
            field.UserSetInputValue(text);
        }

        public static void PressButton(string id)
        {
            var button = WebBrowser.CurrentBrowser.Find.ByName<HtmlControl>(id.ToLowerInvariant());
            button = button ?? WebBrowser.CurrentBrowser.Find.ById<HtmlControl>(id);
            Assert.IsNotNull(button, "Could not find a button with name or id=" + id);
            button.Click();
        }

        [Then(@"the (.*) button is disabled")]
        public void ThenTheButtonIsDisabled(string id)
        {
            var button = WebBrowser.CurrentBrowser.Find.ByName<HtmlControl>(id.ToLowerInvariant());
            button = button ?? WebBrowser.CurrentBrowser.Find.ById<HtmlControl>(id);
            Assert.IsNotNull(button, "Could not find a button with name or id=" + id);
            Assert.IsFalse(button.IsEnabled, "Button with name or id=" + id + " was enabled when it should be diabled");
        }
    }
}
