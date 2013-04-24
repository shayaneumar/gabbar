/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using ArtOfTest.WebAii.Exceptions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class SessionManagement
    {
        public static void LogInAs(string username, string password )
        {
            string currentUser = "";
            try
            {
                currentUser = WebBrowser.CurrentBrowser.Actions.InvokeScript("currentUser");
            }
            catch (ExecuteCommandException)
            { }

            if (!string.Equals(currentUser, username, StringComparison.InvariantCultureIgnoreCase))
            {
                WebBrowser.CleanUp();
                var nav = new Navigation();
                nav.GivenIAmOnSomePage("sign in");
                var input = new UserInput();
                input.GivenIHaveEnteredSomeTestInSomeField(username, "username");
                input.GivenIHaveEnteredSomeTestInSomeField(password, "password");
                input.WhenIPressSomeButton("sign in");
                WebBrowser.CurrentBrowser.WaitForElement(10000, "class=~navbar"); //Give it 5seconds to find header
            }
        }

        [Given("I am logged in as (.*)")]
        public void GivenIAmLoggedInAs(string username)
        {
            LogInAs(username, username);
        }

        [Then("I should be signed in as (.*)")]
        public void ThenIShouldBeSignedInAs(string username)
        {
            WebBrowser.CurrentBrowser.WaitForElement(10000,"class=~navbar");//Give it 5seconds to find header
            var currentUser = GetCurrentUser();
            Assert.AreEqual(username.ToLower(), currentUser.ToLower());
        }

        public static string GetCurrentUser()
        {
            return WebBrowser.CurrentBrowser.Actions.InvokeScript("currentUser");
        }
    }
}
