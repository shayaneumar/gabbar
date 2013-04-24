/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ArtOfTest.Common;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using BuildingSecurity.WebApp.AcceptanceTests.Util;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class TextOutput
    {
        [Then(@"I see ""(.*)""")]
        public void ThenISee(string text)
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "textcontent=~" + text);
            Assert.IsNotNull(element, text + " was not found on page.");
        }

        [Then(@"I see ""(.*)"" in the (.*) list")]
        public void ThenISee(string text, string elementName)
        {
            bool found = ItemExistsInList(text, elementName);

            // Validate that content is not null
            Assert.IsTrue(found, string.Format("{0} List does not contain '{1}'", elementName, text));
        }

        [Then(@"I see (\d+).* in the (.*) list")]
        public void ThenISee(int count, string elementName)
        {
            Assert.AreEqual(count, GetListChildren(Transformer.GetElementIdByName(elementName)).Count(), string.Format("{0} list element count is not equal to ", count.ToString(CultureInfo.InvariantCulture)));
        }

        [Then(@"I see zero.* in the (.*) list")]
        public void ThenISeZero(string elementName)
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + Transformer.GetElementIdByName(elementName));

            // Validate that the element is of ElementType.OrderedList or ElementType.UnorderedList
            Assert.IsTrue((element.ElementType == ElementType.OrderedList) || (element.ElementType == ElementType.UnorderedList), string.Format("{0} element not of expected type List", elementName));

            // Validate that Children contains the specified count of Children
            Assert.AreEqual(0, element.Children.Count, string.Format("{0} list element count is not equal to 0", elementName));
        }

        [Given(@"the (.*) field is empty")]
        public void GivenTheTitleFieldIsEmpty(string elementName)
        {
            ValidateValue("", elementName);
        }

        [Then(@"I see ""(.*)"" for the (.*)")]
        public void ThenISeeFor(string expectedValue, string elementName)
        {
            ValidateValue(expectedValue, elementName);
        }

        private static void ValidateValue(string expectedValue, string elementName)
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "id=" + Transformer.GetElementIdByName(elementName));

            // Get the value from the element InnerText
            string value = element.InnerText;

            // Validate that the value equals createdBy
            StringAssert.AreEqualIgnoringCase(expectedValue, value, string.Format("{0} element value is not equal to {1}", elementName, expectedValue));
        }

        [Then(@"a success message should appear")]
        public void ThenASuccessMessageShouldAppear()
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "class=~alert-success");
            Assert.IsNotNull(element, "No success message appeared");
        }

        [Then(@"an error message should appear")]
        public void ThenAnErrorMessageShouldAppear()
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "class=~alert-error");
            Assert.IsNotNull(element, "No error message appeared");
        }

        [Then(@"I see a (.*) error")]
        public void ThenAnErrorMessageShouldAppear(string errorQualifier)
        {
            var element = WebBrowser.CurrentBrowser.WaitForElement(5000, "class=~alert-error");
            Assert.IsNotNull(element, "No error message appeared");

            // Get the value from the element InnerText
            string value = element.InnerText;
            string expectedValue = GetErrorMessage(errorQualifier);

            // Validate that the value equals createdBy
            StringAssert.Contains(expectedValue.ToUpperInvariant(), value.ToUpperInvariant(), string.Format("Error message is not equal to {0}", expectedValue));
        }

        [Then(@"I should not see (.*) in the site navigation menu")]
        public void ThenIShouldNotSeeAnItemInTheSiteNavigationMenu(string linkname)
        {
            var navMenu = WebBrowser.CurrentBrowser.Find.ByExpression<HtmlControl>("class=nav");
            var link = navMenu.Find.ByExpression<HtmlAnchor>("InnerText=" + linkname);
            Assert.IsNull(link, "Could not find a link with name=" + linkname);
        }

        private static readonly Dictionary<string, string> ErrorQualifierErrorMessage = new Dictionary<string, string>
            {
                {"resource not found", "Resource not found"},
            };

        private static string GetErrorMessage(string errorQualifier)
        {
            if (ErrorQualifierErrorMessage.ContainsKey(errorQualifier))
                return ErrorQualifierErrorMessage[errorQualifier];

            return null;
        }

        public static bool ItemExistsInList(string text, string elementName)
        {
            var children = GetListChildren(Transformer.GetElementIdByName(elementName));
            return children.Any(child => child.Match(new HtmlFindExpression("textcontent=~" + text)) || child.Match(new HtmlFindExpression("innertext=~" + text)));
        }

        public static IEnumerable<Element> GetListChildren(string elementId)
        {
            // Create a hierarchical HtmlFindExpression constraint 
            var listItemExpression = new HtmlFindExpression("tagname=li");
            var listExpression = new HtmlFindExpression("id=" + elementId);
            listItemExpression.AddHierarchyConstraint(new HierarchyConstraint(listExpression, -1));

            // Wait for the list to have children
            WebBrowser.CurrentBrowser.WaitForElement(listItemExpression, 5000, false);
            var list = WebBrowser.CurrentBrowser.WaitForElement(listExpression, 5000, false);

            // Validate that the element is of ElementType.OrderedList or ElementType.UnorderedList
            Assert.IsTrue((list.ElementType == ElementType.OrderedList) || (list.ElementType == ElementType.UnorderedList), string.Format("{0} element not of expected type List", elementId));

            return list.Children;
        }
    }
}
