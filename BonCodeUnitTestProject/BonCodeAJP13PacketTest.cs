using BonCodeAJP13;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13PacketTest and is intended
    ///to contain all BonCodeAJP13PacketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13PacketTest
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
        ///A test for FlipArray
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void FlipArrayTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = BonCodeAJP13Packet_Accessor.FlipArray(data);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetByte
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetByteTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte value = 0; // TODO: Initialize to an appropriate value
            byte valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetByte(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetByteArray
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetByteArrayTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte[] value = null; // TODO: Initialize to an appropriate value
            byte[] valueExpected = null; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetByteArray(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        internal virtual BonCodeAJP13Packet CreateBonCodeAJP13Packet()
        {
            // TODO: Instantiate an appropriate concrete class.
            BonCodeAJP13Packet target = null;
            return target;
        }

        /// <summary>
        ///A test for GetDataBytes
        ///</summary>
        [TestMethod()]
        public void GetDataBytesTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = target.GetDataBytes();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataString
        ///</summary>
        [TestMethod()]
        public void GetDataStringTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetDataString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDateTime
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetDateTimeTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            DateTime value = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime valueExpected = new DateTime(); // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetDateTime(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDouble
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetDoubleTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            double value = 0F; // TODO: Initialize to an appropriate value
            double valueExpected = 0F; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetDouble(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt16
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetInt16Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            ushort value = 0; // TODO: Initialize to an appropriate value
            ushort valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetInt16(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt32
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetInt32Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            int value = 0; // TODO: Initialize to an appropriate value
            int valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetInt32(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt64
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetInt64Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            long value = 0; // TODO: Initialize to an appropriate value
            long valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetInt64(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPacket
        ///</summary>
        [TestMethod()]
        public void GetPacketTest()
        {
            byte[] Buffer = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet expected = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet actual;
            actual = BonCodeAJP13Packet.GetPacket(Buffer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSimpleByteArray
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetSimpleByteArrayTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte[] value = null; // TODO: Initialize to an appropriate value
            byte[] valueExpected = null; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int length = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetSimpleByteArray(data, ref value, pos, length);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetString
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetStringTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            string value = string.Empty; // TODO: Initialize to an appropriate value
            string valueExpected = string.Empty; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetString(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUInt16
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetUInt16Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            ushort value = 0; // TODO: Initialize to an appropriate value
            ushort valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetUInt16(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUInt32
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetUInt32Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            uint value = 0; // TODO: Initialize to an appropriate value
            uint valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetUInt32(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUInt64
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetUInt64Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            ulong value = 0; // TODO: Initialize to an appropriate value
            ulong valueExpected = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.GetUInt64(data, ref value, pos);
            Assert.AreEqual(valueExpected, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUserDataBytes
        ///</summary>
        [TestMethod()]
        public void GetUserDataBytesTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = target.GetUserDataBytes();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUserDataString
        ///</summary>
        [TestMethod()]
        public void GetUserDataStringTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetUserDataString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PrintPacket
        ///</summary>
        [TestMethod()]
        public void PrintPacketTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.PrintPacket();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PrintPacketHeader
        ///</summary>
        [TestMethod()]
        public void PrintPacketHeaderTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.PrintPacketHeader();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetByte
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetByteTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte value = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetByte(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetByteArray
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetByteArrayTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte[] value = null; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetByteArray(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetDateTime
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetDateTimeTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            DateTime value = new DateTime(); // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetDateTime(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetDouble
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetDoubleTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            double value = 0F; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetDouble(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetInt16
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetInt16Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            ushort value = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetInt16(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetInt32
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetInt32Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            int value = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetInt32(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetInt64
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetInt64Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            long value = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetInt64(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetSimpleByteArray
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetSimpleByteArrayTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            byte[] value = null; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetSimpleByteArray(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetString
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetStringTest()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            string value = string.Empty; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetString(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetUInt16
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetUInt16Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            ushort Value = 0; // TODO: Initialize to an appropriate value
            int Pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetUInt16(data, Value, Pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetUInt32
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetUInt32Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            uint value = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetUInt32(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetUInt64
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SetUInt64Test()
        {
            byte[] data = null; // TODO: Initialize to an appropriate value
            ulong value = 0; // TODO: Initialize to an appropriate value
            int pos = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = BonCodeAJP13Packet_Accessor.SetUInt64(data, value, pos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        internal virtual BonCodeAJP13Packet_Accessor CreateBonCodeAJP13Packet_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            BonCodeAJP13Packet_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for WriteBonCodeAJP13Header
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void WriteBonCodeAJP13HeaderTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet_Accessor target = new BonCodeAJP13Packet_Accessor(param0); // TODO: Initialize to an appropriate value
            byte[] data = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.WriteBonCodeAJP13Header(data);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Length
        ///</summary>
        [TestMethod()]
        public void LengthTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            ushort actual;
            actual = target.Length;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PacketID
        ///</summary>
        [TestMethod()]
        public void PacketIDTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            byte actual;
            actual = target.PacketID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PacketLength
        ///</summary>
        [TestMethod()]
        public void PacketLengthTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.PacketLength;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PacketType
        ///</summary>
        [TestMethod()]
        public void PacketTypeTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            byte actual;
            actual = target.PacketType;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for UserDataLength
        ///</summary>
        [TestMethod()]
        public void UserDataLengthTest()
        {
            BonCodeAJP13Packet target = CreateBonCodeAJP13Packet(); // TODO: Initialize to an appropriate value
            ushort actual;
            actual = target.UserDataLength;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
