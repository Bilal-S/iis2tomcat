using System;
using Xunit;
using BonCodeAJP13;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Unit tests for BonCodeAJP13Packet class
    /// Since BonCodeAJP13Packet is abstract, we use a testable concrete implementation
    /// </summary>
    public class PacketTests
    {
        /// <summary>
        /// Testable concrete implementation of the abstract BonCodeAJP13Packet
        /// </summary>
        private class TestablePacket : BonCodeAJP13Packet
        {
            public TestablePacket() : base() { }
            public TestablePacket(byte[] buffer) : base(buffer) { }
            public TestablePacket(string content) : base(content) { }

            public void SetByteStore(byte[] data)
            {
                p_ByteStore = data;
            }

            public byte[] GetByteStore()
            {
                return p_ByteStore;
            }

            // Expose protected methods for testing
            public static int TestSetByte(byte[] data, byte value, int pos) => SetByte(data, value, pos);
            public static int TestGetByte(byte[] data, ref byte value, int pos) => GetByte(data, ref value, pos);
            public static int TestSetInt16(byte[] data, ushort value, int pos) => SetInt16(data, value, pos);
            public static int TestGetInt16(byte[] data, ref ushort value, int pos) => GetInt16(data, ref value, pos);
            public static int TestSetInt32(byte[] data, int value, int pos) => SetInt32(data, value, pos);
            public static int TestGetInt32(byte[] data, ref int value, int pos) => GetInt32(data, ref value, pos);
            public static int TestSetInt64(byte[] data, long value, int pos) => SetInt64(data, value, pos);
            public static int TestGetInt64(byte[] data, ref long value, int pos) => GetInt64(data, ref value, pos);
            public static int TestSetUInt16(byte[] data, ushort value, int pos) => SetUInt16(data, value, pos);
            public static int TestGetUInt16(byte[] data, ref ushort value, int pos) => GetUInt16(data, ref value, pos);
            public static int TestSetUInt32(byte[] data, uint value, int pos) => SetUInt32(data, value, pos);
            public static int TestGetUInt32(byte[] data, ref uint value, int pos) => GetUInt32(data, ref value, pos);
            public static int TestSetDouble(byte[] data, double value, int pos) => SetDouble(data, value, pos);
            public static int TestGetDouble(byte[] data, ref double value, int pos) => GetDouble(data, ref value, pos);
            public static int TestSetString(byte[] data, string value, int pos) => SetString(data, value, pos);
            public static int TestGetString(byte[] data, ref string value, int pos) => GetString(data, ref value, pos);
            public static int TestSetByteArray(byte[] data, byte[] value, int pos) => SetByteArray(data, value, pos);
            public static int TestGetByteArray(byte[] data, ref byte[] value, int pos) => GetByteArray(data, ref value, pos);
            public static byte[] TestFlipArray(byte[] data) => FlipArray(data);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_Empty_CreatesInstance()
        {
            // Act
            var packet = new TestablePacket();

            // Assert
            Assert.NotNull(packet);
            Assert.Equal(0, packet.PacketType);
        }

        [Fact]
        public void Constructor_WithNullBuffer_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<Exception>(() => new TestablePacket((byte[])null));
        }

        [Fact]
        public void Constructor_WithTooShortBuffer_ThrowsException()
        {
            // Arrange
            var shortBuffer = new byte[2]; // Too short

            // Act & Assert
            Assert.Throws<Exception>(() => new TestablePacket(shortBuffer));
        }

        [Fact]
        public void Constructor_WithValidBuffer_SetsUserDataLength()
        {
            // Arrange
            var buffer = new byte[10]; // Minimum valid length

            // Act
            var packet = new TestablePacket(buffer);

            // Assert - User data length should be buffer length - 4 control bytes
            Assert.Equal((ushort)6, packet.UserDataLength);
        }

        [Fact]
        public void Constructor_WithNullString_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<Exception>(() => new TestablePacket((string)null));
        }

        [Fact]
        public void Constructor_WithEmptyString_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<Exception>(() => new TestablePacket(""));
        }

        [Fact]
        public void Constructor_WithValidString_SetsUserDataLength()
        {
            // Arrange
            var content = new string('A', 100); // Create string that's long enough

            // Act
            var packet = new TestablePacket(content);

            // Assert
            Assert.True(packet.UserDataLength > 0);
        }

        #endregion

        #region Byte Tests

        [Fact]
        public void SetByte_WritesCorrectValue()
        {
            // Arrange
            var data = new byte[10];
            byte value = 0xAB;

            // Act
            var newPos = TestablePacket.TestSetByte(data, value, 0);

            // Assert
            Assert.Equal(1, newPos);
            Assert.Equal(0xAB, data[0]);
        }

        [Fact]
        public void GetByte_ReadsCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x12, 0x34, 0x56 };
            byte value = 0;

            // Act
            var newPos = TestablePacket.TestGetByte(data, ref value, 0);

            // Assert
            Assert.Equal(1, newPos);
            Assert.Equal(0x12, value);
        }

        #endregion

        #region Int16 Tests

        [Fact]
        public void SetInt16_WritesCorrectValue()
        {
            // Arrange
            var data = new byte[10];
            ushort value = 0x1234;

            // Act
            var newPos = TestablePacket.TestSetInt16(data, value, 0);

            // Assert
            Assert.Equal(2, newPos);
            // Big Endian: high byte first
            Assert.Equal(0x12, data[0]);
            Assert.Equal(0x34, data[1]);
        }

        [Fact]
        public void GetInt16_ReadsCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x12, 0x34, 0x00, 0x00 };
            ushort value = 0;

            // Act
            var newPos = TestablePacket.TestGetInt16(data, ref value, 0);

            // Assert
            Assert.Equal(2, newPos);
            Assert.Equal((ushort)0x1234, value);
        }

        #endregion

        #region Int32 Tests

        [Fact]
        public void SetInt32_WritesCorrectValue()
        {
            // Arrange
            var data = new byte[10];
            int value = 0x12345678;

            // Act
            var newPos = TestablePacket.TestSetInt32(data, value, 0);

            // Assert
            Assert.Equal(4, newPos);
            // Big Endian
            Assert.Equal(0x12, data[0]);
            Assert.Equal(0x34, data[1]);
            Assert.Equal(0x56, data[2]);
            Assert.Equal(0x78, data[3]);
        }

        [Fact]
        public void GetInt32_ReadsCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x00 };
            int value = 0;

            // Act
            var newPos = TestablePacket.TestGetInt32(data, ref value, 0);

            // Assert
            Assert.Equal(4, newPos);
            Assert.Equal(0x12345678, value);
        }

        #endregion

        #region Int64 Tests

        [Fact]
        public void SetInt64_WritesCorrectValue()
        {
            // Arrange
            var data = new byte[20];
            long value = 0x0102030405060708;

            // Act
            var newPos = TestablePacket.TestSetInt64(data, value, 0);

            // Assert
            Assert.Equal(8, newPos);
            Assert.Equal(0x01, data[0]);
            Assert.Equal(0x08, data[7]);
        }

        [Fact]
        public void GetInt64_ReadsCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            long value = 0;

            // Act
            var newPos = TestablePacket.TestGetInt64(data, ref value, 0);

            // Assert
            Assert.Equal(8, newPos);
            Assert.Equal(0x0102030405060708, value);
        }

        #endregion

        #region UInt16 Tests

        [Fact]
        public void SetUInt16_WritesCorrectValue()
        {
            // Arrange
            var data = new byte[10];
            ushort value = 0xABCD;

            // Act
            var newPos = TestablePacket.TestSetUInt16(data, value, 0);

            // Assert
            Assert.Equal(2, newPos);
            Assert.Equal(0xAB, data[0]);
            Assert.Equal(0xCD, data[1]);
        }

        [Fact]
        public void GetUInt16_ReadsCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0xAB, 0xCD, 0x00 };
            ushort value = 0;

            // Act
            var newPos = TestablePacket.TestGetUInt16(data, ref value, 0);

            // Assert
            Assert.Equal(2, newPos);
            Assert.Equal((ushort)0xABCD, value);
        }

        #endregion

        #region UInt32 Tests

        [Fact]
        public void SetUInt32_WritesCorrectValue()
        {
            // Arrange
            var data = new byte[10];
            uint value = 0x12345678;

            // Act
            var newPos = TestablePacket.TestSetUInt32(data, value, 0);

            // Assert
            Assert.Equal(4, newPos);
            Assert.Equal(0x12, data[0]);
            Assert.Equal(0x78, data[3]);
        }

        [Fact]
        public void GetUInt32_ReadsCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            uint value = 0;

            // Act
            var newPos = TestablePacket.TestGetUInt32(data, ref value, 0);

            // Assert
            Assert.Equal(4, newPos);
            Assert.Equal((uint)0x12345678, value);
        }

        #endregion

        #region Double Tests

        [Fact]
        public void SetAndGetDouble_RoundTrip()
        {
            // Arrange
            var data = new byte[20];
            double originalValue = 3.14159265358979;

            // Act
            TestablePacket.TestSetDouble(data, originalValue, 0);
            double readValue = 0;
            TestablePacket.TestGetDouble(data, ref readValue, 0);

            // Assert
            Assert.Equal(originalValue, readValue, 10);
        }

        #endregion

        #region String Tests

        [Fact]
        public void SetString_WritesWithLengthPrefix()
        {
            // Arrange
            var data = new byte[20];
            string value = "AB";

            // Act
            var newPos = TestablePacket.TestSetString(data, value, 0);

            // Assert
            Assert.Equal(5, newPos); // 2 (length) + 2 (chars) + 1 (terminator)
            Assert.Equal(0, data[0]); // Length high byte
            Assert.Equal(2, data[1]); // Length low byte = 2
            Assert.Equal((byte)'A', data[2]);
            Assert.Equal((byte)'B', data[3]);
            Assert.Equal(0, data[4]); // Terminator
        }

        [Fact]
        public void GetString_ReadsWithLengthPrefix()
        {
            // Arrange
            var data = new byte[] { 0x00, 0x05, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', 0x00 };
            string value = "";

            // Act
            var newPos = TestablePacket.TestGetString(data, ref value, 0);

            // Assert
            Assert.Equal(8, newPos); // 2 (length) + 5 (chars) + 1 (terminator)
            Assert.Equal("Hello", value);
        }

        [Fact]
        public void SetString_WithNull_WritesEmpty()
        {
            // Arrange
            var data = new byte[20];

            // Act
            var newPos = TestablePacket.TestSetString(data, null, 0);

            // Assert
            Assert.Equal(3, newPos); // 2 (length=0) + 0 (chars) + 1 (terminator)
            Assert.Equal(0, data[0]); // Length high byte
            Assert.Equal(0, data[1]); // Length low byte = 0
        }

        #endregion

        #region ByteArray Tests

        [Fact]
        public void SetByteArray_WritesWithLengthPrefix()
        {
            // Arrange
            var data = new byte[20];
            var value = new byte[] { 0x01, 0x02, 0x03 };

            // Act
            var newPos = TestablePacket.TestSetByteArray(data, value, 0);

            // Assert
            Assert.Equal(5, newPos); // 2 (length) + 3 (bytes)
            Assert.Equal(0, data[0]); // Length high byte
            Assert.Equal(3, data[1]); // Length low byte
            Assert.Equal(0x01, data[2]);
            Assert.Equal(0x02, data[3]);
            Assert.Equal(0x03, data[4]);
        }

        [Fact]
        public void GetByteArray_ReadsWithLengthPrefix()
        {
            // Arrange
            var data = new byte[] { 0x00, 0x03, 0x01, 0x02, 0x03, 0x00, 0x00 };
            byte[] value = null;

            // Act
            var newPos = TestablePacket.TestGetByteArray(data, ref value, 0);

            // Assert
            Assert.Equal(5, newPos);
            Assert.NotNull(value);
            Assert.Equal(3, value.Length);
            Assert.Equal(0x01, value[0]);
            Assert.Equal(0x02, value[1]);
            Assert.Equal(0x03, value[2]);
        }

        #endregion

        #region FlipArray Tests

        [Fact]
        public void FlipArray_ReversesArray()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            // Act
            var result = TestablePacket.TestFlipArray(data);

            // Assert
            Assert.Equal(new byte[] { 0x04, 0x03, 0x02, 0x01 }, result);
        }

        [Fact]
        public void FlipArray_WithSingleElement_ReturnsSame()
        {
            // Arrange
            var data = new byte[] { 0x42 };

            // Act
            var result = TestablePacket.TestFlipArray(data);

            // Assert
            Assert.Equal(new byte[] { 0x42 }, result);
        }

        #endregion

        #region GetDataBytes Tests

        [Fact]
        public void GetDataBytes_WhenNull_ReturnsNull()
        {
            // Arrange
            var packet = new TestablePacket();

            // Act
            var result = packet.GetDataBytes();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetDataBytes_WhenSet_ReturnsData()
        {
            // Arrange
            var packet = new TestablePacket();
            var testData = new byte[] { 0x01, 0x02, 0x03 };
            packet.SetByteStore(testData);

            // Act
            var result = packet.GetDataBytes();

            // Assert
            Assert.Equal(testData, result);
        }

        #endregion

        #region PrintPacket Tests

        [Fact]
        public void PrintPacket_WhenNull_ReturnsNullString()
        {
            // Arrange
            var packet = new TestablePacket();

            // Act
            var result = packet.PrintPacket();

            // Assert
            Assert.Contains("<null packet>", result);
        }

        [Fact]
        public void PrintPacket_WithData_ReturnsFormattedString()
        {
            // Arrange
            var packet = new TestablePacket();
            packet.SetByteStore(new byte[] { 0x05, 0x00, 0x02, (byte)'A', (byte)'B', 0x00 });

            // Act
            var result = packet.PrintPacket();

            // Assert
            Assert.Contains("Type:5", result);
            Assert.Contains("begin of packet", result);
            Assert.Contains("end of packet", result);
        }

        #endregion
    }
}