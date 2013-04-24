/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.ObjectModel;
using BuildingSecurity.WebApp.AcceptanceTests.ContextHelpers;
using BuildingSecurity.WebApp.AcceptanceTests.Util;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.BuildingSecurity.Pseudo.Client;
using JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using RestSharp;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class CaseManagement
    {
        private const string NoteEntryFieldId = "case-note-entry";
        private static string _lastNoteText;
        private static string _lastNoteTextNotAdded;

        [Given(@"I am logged in with view case privileges")]
        [Given(@"I am logged in with edit case privileges")]
        public void GivenIAmLoggedInWithAPrivilege()
        {
            SessionManagement.LogInAs(@"cardkey", @"master");
        }

        [Given(@"I am logged in without view case privileges")]
        public void GivenIAmLoggedInWithoutViewCasePrivileges()
        {
            SessionManagement.LogInAs(@"NOCASEMANVIEW", @"master");
        }

        [Given(@"I am viewing a case")]
        public void ViewingCase()
        {
            NavigateToACase();
        }

        //TODO: replaced with given at least 1 case exists.
        [Given(@"a case exists with the title ""(.*)""")]
        public void GivenACaseExistsWith(string caseTitle)
        {
            Simulator.ExecuteScript(Scripting.GetScriptCreateCase(caseTitle));
        }

        [Given(@"an open case exists")]
        public void GivenAnOpenCaseExists()
        {
            Simulator.ExecuteScript(Scripting.GetScriptCreateCase("open Case", createdBy: SessionManagement.GetCurrentUser()));
        }

        [Given(@"a closed case exists")]
        public void GivenAClosedCaseExists()
        {
            //TODO: figure out how we want to handle the feature context.
            //Right now this, and GivenAnOpenCaseExists modify the same context value, creating a temporal dependency.
            Simulator.ExecuteScript(Scripting.GetScriptCreateCase("closed case", createdBy: SessionManagement.GetCurrentUser(), status:"closed"));
        }

        [Given(@"(\d+) cases exist")]
        public void NumberOfCasesExists(int count)
        {
            Simulator.ExecuteScript(Scripting.GetScriptCreateCases(count));
        }

        [Given(@"I have entered a valid case title")]
        public void GivenIHaveEnteredAValidCaseTitle()
        {
            EnterCaseTitle("Doughnut theft at 507");
        }

        [Given(@"I am viewing an open case")]
        public void GivenIAmViewingAnOpenCase()
        {
            //New cases are open, so lets just create a new case an then show it
            var newCase = CaseClient.CreateCase(Guid.NewGuid().ToString(), SessionManagement.GetCurrentUser());
            NavigateToCase(newCase.Id);
        }

        [Given(@"I have entered a case title with extra whitespace")]
        public void GivenIHaveEnteredACaseTitleWithExtraWhitespace()
        {
            EnterCaseTitle("   Case starting with whitespace   ");
        }

        private static void EnterCaseTitle(string text)
        {
            UserInput.EnterText<HtmlInputControl>("title", text);
            ScenarioContext.Current["caseTitle"] = text;
        }

        [When(@"I navigate to a case")]
        public void WhenINavigateToACase()
        {
            NavigateToACase();
        }

        private static void NavigateToACase()
        {
            var aCase = CaseClient.GetCaseListFor(SessionManagement.GetCurrentUser()).FirstOrDefault() ??
                        CaseClient.CreateCase(Guid.NewGuid().ToString(), SessionManagement.GetCurrentUser());

            NavigateToCase(aCase.Id);
        }

        [When(@"I navigate to a nonexistent case")]
        public void WhenINavigateToNonexistentCase()
        {
            NavigateToCase(Guid.NewGuid().ToString());
        }

        [When(@"I view open cases")]
        public void WhenViewOpenCases()
        {
            UserInput.PressButton("filterOpen");
        }

        [When(@"I view all cases")]
        [Given(@"I am viewing all cases")]
        public void WhenViewAllCases()
        {
            UserInput.PressButton("filterAll");
        }

        [Then(@"I see the case's title")]
        public void ThenISeeTheCaseTitle()
        {
            var actualTitle = ViewedCaseContext.After.Title;
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=case-title");
            Assert.AreEqual(actualTitle, element.TextContent,"Case title is not being displayed correctly.");
        }

        [Then(@"I see the case's creator")]
        public void ThenISeeTheCaseCreator()
        {
            var actualCreator = ViewedCaseContext.After.CreatedBy;
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=case-created-by");
            Assert.AreEqual(actualCreator, element.TextContent, "Case creator is not being displayed correctly.");
        }

        [Then(@"I see the case's owner")]
        public void ThenISeeTheCaseOwner()
        {
            var actualOwner = ViewedCaseContext.After.Owner ?? "";
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=case-owner");
            Assert.AreEqual(actualOwner, element.TextContent, "Case owner is not being displayed correctly.");
        }

        [Then(@"I see the case's creation date and time")]
        public void ThenISeeTheCaseCreationTime()
        {
            var actualTime = ViewedCaseContext.After.CreatedDateTimeString;
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=case-created-date-time");
            Assert.AreEqual(actualTime, element.TextContent, "Case creation time is not being displayed correctly.");
        }

        [Then(@"the case's title changes to what I entered")]
        public void ThenTheCaseSTitleChangesToWhatIEntered()
        {
            string actualTitle = ViewedCaseContext.After.Title;
            var enteredTitle = (string)ScenarioContext.Current["lastEditedTitle"];

            Assert.AreEqual(enteredTitle, actualTitle, string.Format("The current title '{0}' is not equal to the expected title '{1}'", actualTitle, enteredTitle));
        }

        [Then(@"the case's title changes to a trimmed version of what I entered")]
        public void ThenTheCaseSTitleChangesToATrimmedVersionOfWhatIEntered()
        {
            string actualTitle = ViewedCaseContext.After.Title;
            var enteredTitle = (string)ScenarioContext.Current["lastEditedTitle"];
            string trimmedEnteredTitle = enteredTitle.Trim();

            Assert.AreEqual(trimmedEnteredTitle, actualTitle, string.Format("The current title '{0}' is not equal to the expected title '{1}'", actualTitle, trimmedEnteredTitle));
        }

        [Then(@"the title does not change")]
        public void ThenTheTitleDoesNotChange()
        {
            var caseBefore = ViewedCaseContext.Before;
            var caseAfter = ViewedCaseContext.After;

            Assert.AreEqual(caseBefore.Title, caseAfter.Title, "The case title changed, from '{0}' to '{1}'",caseBefore.Title, caseAfter.Title);
        }

        [Then(@"I see an error message")]
        public void ThenISeeAnErrorMessage()
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "class=~alert-error");
            Assert.IsNotNull(element, "No error message appeared");
        }

        [Given(@"I have entered a note, but not added it")]
        public void GivenIHaveEnteredANoteButNotAddedIt()
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + NoteEntryFieldId);
            _lastNoteTextNotAdded = "I'm not adding this";

            // Set the element InnerText
            element.SetValue("value", _lastNoteTextNotAdded);
        }

        [When(@"I cancel adding a note")]
        public void WhenICancelAddingANote()
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + NoteEntryFieldId);
            element.SetValue("value", "");
        }

        [When(@"I add a note")]
        public void WhenIAddANote()
        {
            AddNote("Note 1");
        }

        [When(@"I add a note with (.*)")]
        public void WhenAddAMultiLineNote(string noteType)
        {
            string text = "Note 1";
            switch (noteType)
            {
                case "multiple lines": text = "Note 1\nLine 2"; break;
                case "extra white space": text = "  Note 1  "; break;
            }

            AddNote(text);
        }

        [When(@"I change the case's title")]
        public void WhenIChangeTheCaseSTitle()
        {
            EditCaseTitle(Guid.NewGuid().ToString() + "New Case Title");
        }

        [When(@"I change the case's title to one that is too long")]
        public void WhenIChangeTheCaseSTitleToOneThatIsTooLong()
        {
            EditCaseTitle(Guid.NewGuid().ToString() + "New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long. New Case Title that is too long.");
        }

        private static void EditCaseTitle(string title)
        {
            // Find the Case Title (read-only field)
            const string caseTitleId = "case-title";
            var caseTitleElement = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + caseTitleId);
            Assert.IsNotNull(caseTitleElement, "Could not find a field with id=" + caseTitleId);

            // Click on the Case Title to display the editable field
            ScenarioContext.Current["lastEditedTitle"] = title;
            caseTitleElement.Click();

            // Set the new value
            UserInput.EnterText<HtmlInputControl>("edit-case-title", title);

            // Find the Accept button
            var acceptButton = UserInput.GetAcceptButton("case-title-container");

            // Click the Accept button
            acceptButton.Click();
        }

        [Then(@"a case is created with the title I entered")]
        public void ThenACaseIsCreatedWithTheTitleIEntered()
        {
            Assert.IsNotNull(GetCaseByTitle(ScenarioContext.Current["caseTitle"].ToString()), "Case not found");
        }

        [Then(@"a case is created with the title trimmed")]
        public void ThenACaseIsCreatedWithTheTitleTrimmed()
        {
            Assert.IsNotNull(GetCaseByTitle(ScenarioContext.Current["caseTitle"].ToString().Trim()), "Case not found");
        }

        public static JToken GetCaseByTitle(string title)
        {
            object obj = new {};

            IRestResponse response = Simulator.GetResponse(EnvironmentConfiguration.WebUiAddress, "api/v1/Cases",
                                                           Method.GET, obj, "script", "script", true);
            JObject cases = JObject.Parse(response.Content);

            for (int caseIndex = 0; caseIndex < cases["Data"].Count(); caseIndex++)
            {
                if (cases["Data"][caseIndex]["title"].ToString() == title)
                {
                    return cases["Data"][caseIndex];
                }
            }
            return null;
        }

        [Then(@"the note is added")]
        public void ThenTheNoteIsAdded()
        {
            var caseBefore = ViewedCaseContext.Before;
            var caseAfter = ViewedCaseContext.After;

            //Filter out any notes that previously existed on case
            var newNotes = caseAfter.Notes.Where(n => caseBefore.Notes.All(oldNote => oldNote.Id != n.Id));

            Assert.IsTrue(newNotes.Any(n => n.Text == _lastNoteText), "A note with text {0} was not added to case {1}", _lastNoteText, caseBefore.Id);
        }

        [Then(@"a trimmed version of the note is added")]
        public void ThenATrimmedVersionOfTheNoteIsAdded()
        {
            var caseBefore = ViewedCaseContext.Before;
            var caseAfter = ViewedCaseContext.After;

            //Filter out any notes that previously existed on case
            var newNotes = caseAfter.Notes.Where(n => caseBefore.Notes.All(oldNote => oldNote.Id != n.Id));

            //TODO:Refactor this to share a private method with ThenTheNoteIsAdded
            Assert.IsTrue(newNotes.Any(n => n.Text == _lastNoteText.Trim()), "A note with text {0} was not added to case {1}", _lastNoteText, caseBefore.Id);
        }

        [Then(@"I see an empty note input area")]
        public void ThenISeeAnEmptyNoteInputArea()
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + NoteEntryFieldId);

            // Get the value from the element InnerText
            string value = element.InnerText;
            StringAssert.AreEqualIgnoringCase("", value, string.Format("note input area element value is not empty"));
        }

        [Then(@"no note should be added")]
        public void ThenNoNoteShouldBeAdded()
        {
            var caseBefore = ViewedCaseContext.Before;
            var caseAfter = ViewedCaseContext.After;

            //Filter out any notes that previously existed on case
            var newNotes = caseAfter.Notes.Where(n => caseBefore.Notes.All(oldNote => oldNote.Id != n.Id)).ToList();

            //TODO:Refactor this to share a private method with ThenTheNoteIsAdded
            Assert.IsEmpty(newNotes, "No new notes were expected, yet {0} were.", newNotes.Count);
        }

        [Then(@"I see all open cases in the cases list")]
        public void ThenISeeAllOpenCasesInTheOpenCasesList()
        {
            IEnumerable<Element> children = TextOutput.GetListChildren("caselist").ToList();

            foreach (var c in CaseContext.After.Values.Where(x => x.StatusEnum == CaseStatus.Open))
            {
                Assert.IsTrue(children.Any(e => e.IdAttributeValue == c.Id), "Case list does not contain case with Id="+ c.Id);
            }
        }

        [Then(@"I see no closed cases in the cases list")]
        public void ThenISeeNoClosedCasesInTheOpenCasesList()
        {
            /**
             * TODO: This should be refactored such that we can get the ids of all the cases in the list
             * Rather than searching the list for ids matching case ids.
             **/
            IEnumerable<Element> children = TextOutput.GetListChildren("caselist").ToList();

            foreach (var c in CaseContext.After.Values.Where(x => x.StatusEnum == CaseStatus.Closed))
            {
                Assert.IsFalse(children.Any(e => e.IdAttributeValue == c.Id), "Unexpected case found in case list id=" + c.Id);
            }
        }

        [When(@"a case is created")]
        public void WhenACaseIsCreated()
        {
            CaseClient.CreateCase(caseTitle: IdHelpers.GetId(), user: SessionManagement.GetCurrentUser());
        }

        [Then(@"I see the new case in the case list")]
        public void ThenISeeTheNewCaseInTheCaseList()
        {
            var newCase = CaseContext.After[CaseContext.After.Keys.Except(CaseContext.Before.Keys).Single()];
            WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + newCase.Id);
        }

        [When(@"a case title is changed")]
        public void WhenACaseTitleIsChanged()
        {
            var victim = CaseContext.Before.Keys.First();
            var request = new
                {
                    at = TimeSpan.Zero,
                    name = "UpdateCase",
                    value = new CaseData {Id = victim, Title = Guid.NewGuid().ToString()}
                };
            Simulator.ExecuteRequest(request);
        }

        [Then(@"I see the changed case title in the case list")]
        public void ThenISeeTheChangedCaseTitleInTheCaseList()
        {
            var changedCase = CaseContext.After.Single(y => y.Value.Title != CaseContext.Before[y.Key].Title).Value;
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + changedCase.Id, "InnerText=~" + changedCase.Title);
            Assert.IsNotNull(element, "Case title not changed");
        }

        private static void AddNote(string text)
        {
            _lastNoteText = text;

            UserInput.EnterText<HtmlTextArea>(NoteEntryFieldId, text);
            UserInput.PressButton("create");
        }

        public static JToken GetCase(string caseId, IEnumerable<JToken> cases)
        {
            return cases.FirstOrDefault(caseDetails => caseDetails["id"].ToString() == caseId);
        }

        private static void NavigateToCase(string caseId)
        {
            WebBrowser.NavigateTo(EnvironmentConfiguration.WebUiAddress + string.Format("/Cases/{0}", caseId));
        }
    }
}
