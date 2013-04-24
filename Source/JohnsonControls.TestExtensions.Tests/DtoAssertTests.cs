/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TestExtensions
{
    [TestClass]
    public class DtoAssertTests
    {
        public class ExampleDto1
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public double DoubleValue { get; set; }
            public double FieldDouble;
            public IEnumerable<string> ListOfStrings { get; set; }
        }

        public class ExampleDto2
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public double DoubleValue { get; set; }
            public double FieldDouble;
            public IEnumerable<string> ListOfStrings { get; set; }
        }

        public class ComplexDto1
        {
            public string StringField;
            public ExampleDto1 Dto1 { get; set; }

            [ExcludeFromEquality]
            public string ExcludedProperty { get; set; }

            [ExcludeFromEquality]
            public string ExcludedField;
        }

        [TestClass]
        public class AreEqual
        {
            [TestMethod]
            public void ExpectedEqualsActual_AssertPasses()
            {
                //Arrange
                var expected = new ExampleDto1
                                   {
                                       StringValue = "example",
                                       DoubleValue = 1.0,
                                       IntValue = 2,
                                       ListOfStrings = new[] {"Foo"}
                                   };
                var actual = new ExampleDto1
                                 {
                                     StringValue = "example",
                                     DoubleValue = 1.0,
                                     IntValue = 2,
                                     ListOfStrings = new[] {"Foo"}
                                 };

                //Act
                DtoAssert.AreEqual(expected, actual);

                //Assert Test Passed
            }

            [TestMethod]
            public void ExpectedHasDifferentEnumerationThanActual_AssertFails()
            {
                object expected = new ExampleDto1 {ListOfStrings = new[] {"Foo"}};
                object actual = new ExampleDto2 {ListOfStrings = new[] {"Bar"}};

                //Act
                bool assertFailed = false;
                try
                {
                    DtoAssert.AreEqual(expected, actual);
                }
                catch (AssertFailedException)
                {
                    assertFailed = true;
                }

                //Assert
                Assert.IsTrue(assertFailed);
            }

            [TestMethod]
            public void ExpectedNotSameTypeAsActual_AssertFails()
            {
                //Arrange
                object expected = new ExampleDto1 {StringValue = "example", DoubleValue = 1.0, IntValue = 2,};
                object actual = new ExampleDto2 {StringValue = "example", DoubleValue = 1.0, IntValue = 2,};

                //Act
                bool assertFailed = false;
                try
                {
                    DtoAssert.AreEqual(expected, actual);
                }
                catch (AssertFailedException)
                {
                    assertFailed = true;
                }

                //Assert
                Assert.IsTrue(assertFailed);
            }

            [TestMethod]
            public void ExpectedHasDifferentStringValueThanActual_AssertFails()
            {
                var expected = new ExampleDto1
                                   {
                                       StringValue = "example",
                                   };
                var actual = new ExampleDto1
                                 {
                                     StringValue = "example2",
                                 };


                bool assertFailed = false;
                try
                {
                    DtoAssert.AreEqual(expected, actual);
                }
                catch (AssertFailedException)
                {
                    assertFailed = true;
                }

                Assert.IsTrue(assertFailed);
            }
            
            [TestMethod]
            public void ExpectedHasDifferentFieldDoubleThanActual_AssertFails()
            {
                var expected = new ExampleDto1
                {
                    FieldDouble = 2.0,
                };
                var actual = new ExampleDto1
                {
                    FieldDouble = 100,
                };


                bool assertFailed = false;
                try
                {
                    DtoAssert.AreEqual(expected, actual);
                }
                catch (AssertFailedException)
                {
                    assertFailed = true;
                }

                Assert.IsTrue(assertFailed);
            }

            [TestMethod]
            public void ComplexDtosAreEqual_AssertPasses()
            {
                //Arrange
                var expected = new ComplexDto1 {Dto1 = new ExampleDto1 {DoubleValue = 1.0, IntValue = 10}};
                var actual = new ComplexDto1 {Dto1 = new ExampleDto1 {DoubleValue = 1.0, IntValue = 10}};

                //Act
                DtoAssert.AreEqual(expected, actual);

                //Assert test passes
            }

            [TestMethod]
            public void ComplexDtosAreNotEqual_AssertFails()
            {
                //Arrange
                var expected = new ComplexDto1 {Dto1 = new ExampleDto1 {DoubleValue = 1.0, IntValue = 10}};
                var actual = new ComplexDto1 {Dto1 = new ExampleDto1 {DoubleValue = 1.0, IntValue = 20}};

                //Act
                bool assertFailed = false;
                try
                {
                    DtoAssert.AreEqual(expected, actual);
                }
                catch (AssertFailedException)
                {
                    assertFailed = true;
                }

                //Assert
                Assert.IsTrue(assertFailed);
            }

            [TestMethod]
            public void ComplexDtosAreEqualExceptForExcludedProperty_AssertPasses()
            {
                //Arrange
                var expected = new ComplexDto1 { ExcludedProperty = "1" };
                var actual = new ComplexDto1 { ExcludedProperty = "2" };

                //Act
                DtoAssert.AreEqual(expected,actual);

                //Assert test passes
            }

            [TestMethod]
            public void ComplexDtosAreEqualExceptForExcludedField_AssertPasses()
            {
                //Arrange
                var expected = new ComplexDto1 { ExcludedField = "1" };
                var actual = new ComplexDto1 { ExcludedField = "2" };

                //Act
                DtoAssert.AreEqual(expected, actual);

                //Assert test passes
            }

            [TestMethod]
            public void CollectionOfDtoObjectsThatAreEqual()
            {
                // Arrange
                var expected = new[]
                                   {
                                       new ExampleDto1
                                           {
                                               DoubleValue = 3,
                                               FieldDouble = 4.5,
                                               IntValue = 6,
                                               ListOfStrings = null,
                                               StringValue = "hello"
                                           }
                                   };
                var actual = new[]
                                   {
                                       new ExampleDto1
                                           {
                                               DoubleValue = 3,
                                               FieldDouble = 4.5,
                                               IntValue = 6,
                                               ListOfStrings = null,
                                               StringValue = "hello"
                                           }
                                   };

                // Act
                DtoAssert.AreEqual(expected, actual);

                // Assert test passes (requires no check)


            }

            [TestMethod]
            public void CollectionOfDtoObjectsThatAreNotEqual()
            {
                // Arrange - The StringValue of the dto objects are different.
                var expected = new[]
                                   {
                                       new ExampleDto1
                                           {
                                               DoubleValue = 3,
                                               FieldDouble = 4.5,
                                               IntValue = 6,
                                               ListOfStrings = null,
                                               StringValue = "hello2"
                                           }
                                   };
                var actual = new[]
                                   {
                                       new ExampleDto1
                                           {
                                               DoubleValue = 3,
                                               FieldDouble = 4.5,
                                               IntValue = 6,
                                               ListOfStrings = null,
                                               StringValue = "hello"
                                           }
                                   };

                // Act
                var exceptionThrown = false;
                try
                {
                    DtoAssert.AreEqual(expected, actual);
                }
                catch(AssertFailedException)
                {
                    exceptionThrown = true;
                }

                // Assert
                Assert.IsTrue(exceptionThrown, "Expected AssertionFailedException");

            }
        }
    }
}