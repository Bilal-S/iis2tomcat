using BonCodeAJP13.TomcatPackets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for TomcatSendHeadersTest and is intended
    ///to contain all TomcatSendHeadersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TomcatSendHeadersTest
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
        ///A test for TomcatSendHeaders Constructor
        ///</summary>
        [TestMethod()]
        public void TomcatSendHeadersConstructorTest()
        {
            byte[] content = null; // TODO: Initialize to an appropriate value
            TomcatSendHeaders target = new TomcatSendHeaders(content);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for TomcatSendHeaders Constructor
        ///</summary>
        [TestMethod()]
        public void TomcatSendHeadersConstructorTest1()
        {
            TomcatSendHeaders target = new TomcatSendHeaders();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetHeaders
        ///</summary>
        [TestMethod()]
        public void GetHeadersTest()
        {
            TomcatSendHeaders target = new TomcatSendHeaders(); // TODO: Initialize to an appropriate value
            NameValueCollection expected = null; // TODO: Initialize to an appropriate value
            NameValueCollection actual;
            actual = target.GetHeaders();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetStatus
        ///</summary>
        [TestMethod()]
        public void GetStatusTest()
        {
            TomcatSendHeaders target = new TomcatSendHeaders(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetStatus();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TestHeader
        ///</summary>
        [TestMethod()]
        public void TestHeaderTest()
        {
            TomcatSendHeaders target = new TomcatSendHeaders(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TestHeader();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
