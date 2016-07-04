using BonCodeAJP13;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13PacketHeadersTest and is intended
    ///to contain all BonCodeAJP13PacketHeadersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13PacketHeadersTest
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
        ///A test for BonCodeAJP13PacketHeaders Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13PacketHeadersConstructorTest()
        {
            BonCodeAJP13PacketHeaders target = new BonCodeAJP13PacketHeaders();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetAttributeByte
        ///</summary>
        [TestMethod()]
        public void GetAttributeByteTest()
        {
            string attributeKey = string.Empty; // TODO: Initialize to an appropriate value
            byte expected = 0; // TODO: Initialize to an appropriate value
            byte actual;
            actual = BonCodeAJP13PacketHeaders.GetAttributeByte(attributeKey);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHeaderBytes
        ///</summary>
        [TestMethod()]
        public void GetHeaderBytesTest()
        {
            string headerKey = string.Empty; // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = BonCodeAJP13PacketHeaders.GetHeaderBytes(headerKey);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetMethodByte
        ///</summary>
        [TestMethod()]
        public void GetMethodByteTest()
        {
            string methodKey = string.Empty; // TODO: Initialize to an appropriate value
            byte expected = 0; // TODO: Initialize to an appropriate value
            byte actual;
            actual = BonCodeAJP13PacketHeaders.GetMethodByte(methodKey);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTomcatHeaderString
        ///</summary>
        [TestMethod()]
        public void GetTomcatHeaderStringTest()
        {
            byte headerKey = 0; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = BonCodeAJP13PacketHeaders.GetTomcatHeaderString(headerKey);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PopulateAttributeTranslation
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void PopulateAttributeTranslationTest()
        {
            BonCodeAJP13PacketHeaders_Accessor.PopulateAttributeTranslation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PopulateHeaderTranslation
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void PopulateHeaderTranslationTest()
        {
            BonCodeAJP13PacketHeaders_Accessor.PopulateHeaderTranslation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PopulateMethodTranslation
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void PopulateMethodTranslationTest()
        {
            BonCodeAJP13PacketHeaders_Accessor.PopulateMethodTranslation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PopulateTomcatHeaders
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void PopulateTomcatHeadersTest()
        {
            BonCodeAJP13PacketHeaders_Accessor.PopulateTomcatHeaders();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
