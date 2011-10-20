using BonCodeAJP13;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Timers;
using System.Net.Sockets;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13ServerConnectionTest and is intended
    ///to contain all BonCodeAJP13ServerConnectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13ServerConnectionTest
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
        ///A test for BonCodeAJP13ServerConnection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ServerConnectionConstructorTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet singlePacket = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(server, port, singlePacket);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ServerConnection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ServerConnectionConstructorTest1()
        {
            BonCodeAJP13PacketCollection packetsToSend = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(packetsToSend);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ServerConnection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ServerConnectionConstructorTest2()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            BonCodeAJP13PacketCollection packetsToSend = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(server, port, packetsToSend);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ServerConnection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ServerConnectionConstructorTest3()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ServerConnection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ServerConnectionConstructorTest4()
        {
            BonCodeAJP13Packet singlePacket = null; // TODO: Initialize to an appropriate value
            bool delayConnection = false; // TODO: Initialize to an appropriate value
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(singlePacket, delayConnection);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ServerConnection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ServerConnectionConstructorTest5()
        {
            BonCodeAJP13Packet singlePacket = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(singlePacket);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddPacketToSendQueue
        ///</summary>
        [TestMethod()]
        public void AddPacketToSendQueueTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet singlePacket = null; // TODO: Initialize to an appropriate value
            target.AddPacketToSendQueue(singlePacket);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AnalyzePackage
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void AnalyzePackageTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            byte[] receiveBuffer = null; // TODO: Initialize to an appropriate value
            bool skipFlush = false; // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = target.AnalyzePackage(receiveBuffer, skipFlush);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BeginConnection
        ///</summary>
        [TestMethod()]
        public void BeginConnectionTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            target.BeginConnection();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ByteSearch
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void ByteSearchTest()
        {
            byte[] searchIn = null; // TODO: Initialize to an appropriate value
            byte[] searchBytes = null; // TODO: Initialize to an appropriate value
            int start = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13ServerConnection_Accessor.ByteSearch(searchIn, searchBytes, start);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CheckMutex
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void CheckMutexTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            target.CheckMutex();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CloseConnectionNoError
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void CloseConnectionNoErrorTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            target.CloseConnectionNoError(message);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ComunicateWithTomcat
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void ComunicateWithTomcatTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            target.ComunicateWithTomcat();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ConnectionError
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void ConnectionErrorTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            string messageType = string.Empty; // TODO: Initialize to an appropriate value
            target.ConnectionError(message, messageType);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ConnectionError
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void ConnectionErrorTest1()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            target.ConnectionError();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for FlipArray
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void FlipArrayTest()
        {
            byte[] Data = null; // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = BonCodeAJP13ServerConnection_Accessor.FlipArray(Data);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt16B
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetInt16BTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            byte[] Data = null; // TODO: Initialize to an appropriate value
            int Pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetInt16B(Data, Pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HandleConnection
        ///</summary>
        [TestMethod()]
        public void HandleConnectionTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            target.HandleConnection();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ProcessFlush
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void ProcessFlushTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            target.ProcessFlush();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for p_CreateConnection
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void p_CreateConnectionTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            BonCodeAJP13PacketCollection packetsToSend = null; // TODO: Initialize to an appropriate value
            target.p_CreateConnection(packetsToSend);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for p_KeepAliveTimer_Elapsed
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void p_KeepAliveTimer_ElapsedTest()
        {
            BonCodeAJP13ServerConnection_Accessor target = new BonCodeAJP13ServerConnection_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            ElapsedEventArgs e = null; // TODO: Initialize to an appropriate value
            target.p_KeepAliveTimer_Elapsed(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AbortConnection
        ///</summary>
        [TestMethod()]
        public void AbortConnectionTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.AbortConnection = expected;
            actual = target.AbortConnection;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FlushDelegateFunction
        ///</summary>
        [TestMethod()]
        public void FlushDelegateFunctionTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            FlushPacketsCollectionDelegate expected = null; // TODO: Initialize to an appropriate value
            target.FlushDelegateFunction = expected;
            Assert.Inconclusive("Write-only properties cannot be verified.");
        }

        /// <summary>
        ///A test for Port
        ///</summary>
        [TestMethod()]
        public void PortTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Port = expected;
            actual = target.Port;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReceivedDataCollection
        ///</summary>
        [TestMethod()]
        public void ReceivedDataCollectionTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            BonCodeAJP13PacketCollection actual;
            actual = target.ReceivedDataCollection;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Server
        ///</summary>
        [TestMethod()]
        public void ServerTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Server = expected;
            actual = target.Server;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetTcpClient
        ///</summary>
        [TestMethod()]
        public void SetTcpClientTest()
        {
            BonCodeAJP13ServerConnection target = new BonCodeAJP13ServerConnection(); // TODO: Initialize to an appropriate value
            TcpClient expected = null; // TODO: Initialize to an appropriate value
            target.SetTcpClient = expected;
            Assert.Inconclusive("Write-only properties cannot be verified.");
        }
    }
}
