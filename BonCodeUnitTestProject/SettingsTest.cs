using BonCodeAJP13.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for SettingsTest and is intended
    ///to contain all SettingsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SettingsTest
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
        ///A test for Settings Constructor
        ///</summary>
        [TestMethod()]
        public void SettingsConstructorTest()
        {
            Settings target = new Settings();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Default
        ///</summary>
        [TestMethod()]
        public void DefaultTest()
        {
            Settings actual;
            actual = Settings.Default;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FlushThreshold
        ///</summary>
        [TestMethod()]
        public void FlushThresholdTest()
        {
            Settings target = new Settings(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.FlushThreshold;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LogDir
        ///</summary>
        [TestMethod()]
        public void LogDirTest()
        {
            Settings target = new Settings(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.LogDir;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LogLevel
        ///</summary>
        [TestMethod()]
        public void LogLevelTest()
        {
            Settings target = new Settings(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.LogLevel;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MaxConnections
        ///</summary>
        [TestMethod()]
        public void MaxConnectionsTest()
        {
            Settings target = new Settings(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.MaxConnections;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Port
        ///</summary>
        [TestMethod()]
        public void PortTest()
        {
            Settings target = new Settings(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Port;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Server
        ///</summary>
        [TestMethod()]
        public void ServerTest()
        {
            Settings target = new Settings(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Server;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
