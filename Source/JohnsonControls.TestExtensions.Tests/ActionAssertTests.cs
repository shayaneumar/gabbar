using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TestExtensions
{
    [TestClass]
    public class ActionAssertTests
    {
        [TestClass]
        public class Throws
        {
            [TestMethod]
            public void ExceptionIsThrown_TestPasses()
            {
                //arrange
                Action exceptionThrower = () => { throw new Exception();};
                //act
                ActionAssert.Throws<Exception>(exceptionThrower);

                //assert no exceptions were thrown and test passes
            }

            [TestMethod]
            public void ExceptionIsNotThrown_TestFails()
            {
                //arrange
                int x = 0;
                bool testWouldHaveFailed = false;
                try
                {
                    //act
                    ActionAssert.Throws<Exception>(() => x++);
                }
                catch (AssertFailedException)
                {
                    testWouldHaveFailed = true;
                }

                //assert
                Assert.IsTrue(testWouldHaveFailed);
            }

            [TestMethod]
            public void WrongTypeOfExceptionIsThrown_TestFails()
            {
                //arrange
                Action exceptionThrower = () => { throw new Exception(); };
                bool testWouldHaveFailed = false;
                try
                {
                    //act
                    ActionAssert.Throws<InvalidOperationException>(exceptionThrower);
                }
                catch (AssertFailedException)
                {
                    testWouldHaveFailed = true;
                }

                //assert
                Assert.IsTrue(testWouldHaveFailed);
            }

            [TestMethod]
            public void ArgumentExceptionThrownForWrongParameter_TestFails()
            {
                //arrange
// ReSharper disable NotResolvedInText
                Action exceptionThrower = () => { throw new ArgumentNullException("barn"); };
// ReSharper restore NotResolvedInText

                bool testWouldHaveFailed = false;
                try
                {
                    //act
                    ActionAssert.Throws<ArgumentException>(exceptionThrower,"farm");
                }
                catch (AssertFailedException)
                {
                    testWouldHaveFailed = true;
                }

                //assert
                Assert.IsTrue(testWouldHaveFailed);
            }

            [TestMethod]
            public void ArgumentExceptionThrownForCorrectParameter_TestFails()
            {
                //arrange
                // ReSharper disable NotResolvedInText
                Action exceptionThrower = () => { throw new ArgumentNullException("barn"); };
                // ReSharper restore NotResolvedInText

                //act
                ActionAssert.Throws<ArgumentException>(exceptionThrower, "barn");

                //assert no exceptions were thrown and test passes
            }
        }
    }
}
