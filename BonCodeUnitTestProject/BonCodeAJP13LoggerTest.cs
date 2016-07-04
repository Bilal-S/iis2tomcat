using BonCodeAJP13;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13LoggerTest and is intended
    ///to contain all BonCodeAJP13LoggerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13LoggerTest
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
        ///A test for BonCodeAJP13Logger Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13LoggerConstructorTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Mutex loggerMutex = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Logger target = new BonCodeAJP13Logger(fileName, loggerMutex);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetAssemblyDirectory
        ///</summary>
        [TestMethod()]
        public void GetAssemblyDirectoryTest()
        {
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = BonCodeAJP13Logger.GetAssemblyDirectory();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetLogDir
        ///</summary>
        [TestMethod()]
        public void GetLogDirTest()
        {
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = BonCodeAJP13Logger.GetLogDir();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LogException
        ///</summary>
        [TestMethod()]
        public void LogExceptionTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Mutex loggerMutex = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Logger target = new BonCodeAJP13Logger(fileName, loggerMutex); // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            int onlyAboveLogLevel = 0; // TODO: Initialize to an appropriate value
            target.LogException(e, message, onlyAboveLogLevel);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LogMessage
        ///</summary>
        [TestMethod()]
        public void LogMessageTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Mutex loggerMutex = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Logger target = new BonCodeAJP13Logger(fileName, loggerMutex); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            int onlyAboveLogLevel = 0; // TODO: Initialize to an appropriate value
            target.LogMessage(message, onlyAboveLogLevel);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LogMessageAndType
        ///</summary>
        [TestMethod()]
        public void LogMessageAndTypeTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Mutex loggerMutex = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Logger target = new BonCodeAJP13Logger(fileName, loggerMutex); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            string messageType = string.Empty; // TODO: Initialize to an appropriate value
            int onlyAboveLogLevel = 0; // TODO: Initialize to an appropriate value
            target.LogMessageAndType(message, messageType, onlyAboveLogLevel);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LogPacket
        ///</summary>
        [TestMethod()]
        public void LogPacketTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Mutex loggerMutex = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Logger target = new BonCodeAJP13Logger(fileName, loggerMutex); // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet packet = null; // TODO: Initialize to an appropriate value
            bool logAllways = false; // TODO: Initialize to an appropriate value
            int onlyAboveLogLevel = 0; // TODO: Initialize to an appropriate value
            target.LogPacket(packet, logAllways, onlyAboveLogLevel);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LogPackets
        ///</summary>
        [TestMethod()]
        public void LogPacketsTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Mutex loggerMutex = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Logger target = new BonCodeAJP13Logger(fileName, loggerMutex); // TODO: Initialize to an appropriate value
            BonCodeAJP13PacketCollection packets = null; // TODO: Initialize to an appropriate value
            bool logAllways = false; // TODO: Initialize to an appropriate value
            int onlyAboveLogLevel = 0; // TODO: Initialize to an appropriate value
            target.LogPackets(packets, logAllways, onlyAboveLogLevel);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
