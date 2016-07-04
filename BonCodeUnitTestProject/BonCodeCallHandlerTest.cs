using BonCodeIIS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using BonCodeAJP13;
using System.Web;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeCallHandlerTest and is intended
    ///to contain all BonCodeCallHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeCallHandlerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        ///A test for BonCodeCallHandler Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeCallHandlerConstructorTest()
        {
            BonCodeCallHandler target = new BonCodeCallHandler();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Finalize
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeIIS.dll")]
        public void FinalizeTest()
        {
            BonCodeCallHandler_Accessor target = new BonCodeCallHandler_Accessor(); // TODO: Initialize to an appropriate value
            target.Finalize();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetHeaders
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeIIS.dll")]
        public void GetHeadersTest()
        {
            BonCodeCallHandler_Accessor target = new BonCodeCallHandler_Accessor(); // TODO: Initialize to an appropriate value
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetHeaders(httpHeaders);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PrintFlush
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeIIS.dll")]
        public void PrintFlushTest()
        {
            BonCodeCallHandler_Accessor target = new BonCodeCallHandler_Accessor(); // TODO: Initialize to an appropriate value
            BonCodeAJP13PacketCollection flushCollection = null; // TODO: Initialize to an appropriate value
            target.PrintFlush(flushCollection);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ProcessRequest
        ///</summary>
        [TestMethod()]
        public void ProcessRequestTest()
        {
            BonCodeCallHandler target = new BonCodeCallHandler(); // TODO: Initialize to an appropriate value
            HttpContext context = null; // TODO: Initialize to an appropriate value
            target.ProcessRequest(context);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for TestBinary
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeIIS.dll")]
        public void TestBinaryTest()
        {
            BonCodeCallHandler_Accessor target = new BonCodeCallHandler_Accessor(); // TODO: Initialize to an appropriate value
            string contentType = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TestBinary(contentType);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsReusable
        ///</summary>
        [TestMethod()]
        public void IsReusableTest()
        {
            BonCodeCallHandler target = new BonCodeCallHandler(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsReusable;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
