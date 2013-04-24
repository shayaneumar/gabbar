using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    
    
    /// <summary>
    ///This is a test class for P2000AlarmDetailsRequestWrapperTest and is intended
    ///to contain all P2000AlarmDetailsRequestWrapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class P2000AlarmDetailsRequestWrapperTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for P2000AlarmDetailsRequestWrapper Constructor
        ///</summary>
        [TestMethod]
        public void P2000AlarmDetailsRequestWrapperConstructorTest()
        {
            const string xml = @"
                <P2000Request>
                  <AlarmDetailsRequest>
                    <AlarmDetailsFilter>
                      <AlarmGuid>
                        <CV>235337E0-B192-4259-82AE-C0D10C64C831</CV>
                      </AlarmGuid>
                    </AlarmDetailsFilter>
                  </AlarmDetailsRequest>
                </P2000Request>
            ";

            P2000AlarmDetailsRequestWrapper expected = (new XmlSerializer<P2000AlarmDetailsRequestWrapper>()).Deserialize(xml);
            var actual =
                new P2000AlarmDetailsRequestWrapper(
                    new AlarmDetailsRequest(
                        new AlarmDetailsFilter("235337E0-B192-4259-82AE-C0D10C64C831"), null, null));

            DtoAssert.AreEqual(expected, actual);
        }
    }
}
