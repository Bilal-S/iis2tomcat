using BonCodeAJP13.ServerPackets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13ForwardRequestTest and is intended
    ///to contain all BonCodeAJP13ForwardRequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13ForwardRequestTest
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
        ///A test for BonCodeAJP13ForwardRequest Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ForwardRequestConstructorTest()
        {
            byte method = 0; // TODO: Initialize to an appropriate value
            string protocol = string.Empty; // TODO: Initialize to an appropriate value
            string req_uri = string.Empty; // TODO: Initialize to an appropriate value
            string remote_addr = string.Empty; // TODO: Initialize to an appropriate value
            string remote_host = string.Empty; // TODO: Initialize to an appropriate value
            string server_name = string.Empty; // TODO: Initialize to an appropriate value
            ushort server_port = 0; // TODO: Initialize to an appropriate value
            bool is_ssl = false; // TODO: Initialize to an appropriate value
            int num_headers = 0; // TODO: Initialize to an appropriate value
            BonCodeAJP13ForwardRequest target = new BonCodeAJP13ForwardRequest(method, protocol, req_uri, remote_addr, remote_host, server_name, server_port, is_ssl, num_headers);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ForwardRequest Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ForwardRequestConstructorTest1()
        {
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13ForwardRequest target = new BonCodeAJP13ForwardRequest(httpHeaders);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ForwardRequest Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ForwardRequestConstructorTest2()
        {
            BonCodeAJP13ForwardRequest target = new BonCodeAJP13ForwardRequest();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BonCodeAJP13ForwardRequest Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13ForwardRequestConstructorTest3()
        {
            byte[] content = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13ForwardRequest target = new BonCodeAJP13ForwardRequest(content);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for CheckHeaders
        ///</summary>
        [TestMethod()]
        public void CheckHeadersTest()
        {
            BonCodeAJP13ForwardRequest target = new BonCodeAJP13ForwardRequest(); // TODO: Initialize to an appropriate value
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            NameValueCollection expected = null; // TODO: Initialize to an appropriate value
            NameValueCollection actual;
            actual = target.CheckHeaders(httpHeaders);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetKeyValue
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetKeyValueTest()
        {
            BonCodeAJP13ForwardRequest_Accessor target = new BonCodeAJP13ForwardRequest_Accessor(); // TODO: Initialize to an appropriate value
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            string keyName = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetKeyValue(httpHeaders, keyName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for KeyExists
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void KeyExistsTest()
        {
            BonCodeAJP13ForwardRequest_Accessor target = new BonCodeAJP13ForwardRequest_Accessor(); // TODO: Initialize to an appropriate value
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            string keyName = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.KeyExists(httpHeaders, keyName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for WritePacket
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void WritePacketTest()
        {
            BonCodeAJP13ForwardRequest_Accessor target = new BonCodeAJP13ForwardRequest_Accessor(); // TODO: Initialize to an appropriate value
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            target.WritePacket(httpHeaders);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for WritePacket
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void WritePacketTest1()
        {
            BonCodeAJP13ForwardRequest_Accessor target = new BonCodeAJP13ForwardRequest_Accessor(); // TODO: Initialize to an appropriate value
            byte method = 0; // TODO: Initialize to an appropriate value
            string protocol = string.Empty; // TODO: Initialize to an appropriate value
            string req_uri = string.Empty; // TODO: Initialize to an appropriate value
            string remote_addr = string.Empty; // TODO: Initialize to an appropriate value
            string remote_host = string.Empty; // TODO: Initialize to an appropriate value
            string server_name = string.Empty; // TODO: Initialize to an appropriate value
            ushort server_port = 0; // TODO: Initialize to an appropriate value
            bool is_ssl = false; // TODO: Initialize to an appropriate value
            int num_headers = 0; // TODO: Initialize to an appropriate value
            NameValueCollection httpHeaders = null; // TODO: Initialize to an appropriate value
            target.WritePacket(method, protocol, req_uri, remote_addr, remote_host, server_name, server_port, is_ssl, num_headers, httpHeaders);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for WritePacket
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void WritePacketTest2()
        {
            BonCodeAJP13ForwardRequest_Accessor target = new BonCodeAJP13ForwardRequest_Accessor(); // TODO: Initialize to an appropriate value
            byte[] transferContent = null; // TODO: Initialize to an appropriate value
            target.WritePacket(transferContent);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for WritePacketTest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void WritePacketTestTest()
        {
            BonCodeAJP13ForwardRequest_Accessor target = new BonCodeAJP13ForwardRequest_Accessor(); // TODO: Initialize to an appropriate value
            byte method = 0; // TODO: Initialize to an appropriate value
            string protocol = string.Empty; // TODO: Initialize to an appropriate value
            string req_uri = string.Empty; // TODO: Initialize to an appropriate value
            string remote_addr = string.Empty; // TODO: Initialize to an appropriate value
            string remote_host = string.Empty; // TODO: Initialize to an appropriate value
            string server_name = string.Empty; // TODO: Initialize to an appropriate value
            ushort server_port = 0; // TODO: Initialize to an appropriate value
            bool is_ssl = false; // TODO: Initialize to an appropriate value
            int num_headers = 0; // TODO: Initialize to an appropriate value
            target.WritePacketTest(method, protocol, req_uri, remote_addr, remote_host, server_name, server_port, is_ssl, num_headers);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
