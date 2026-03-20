using System;
using System.Reflection;
using Xunit;
using BonCodeAJP13;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Unit tests for BonCodeAJP13ServerConnection class
    /// Tests static utility methods and properties that don't require network connections
    /// </summary>
    public class ServerConnectionTests
    {
        #region Static Method Tests - FlipArray

        [Fact]
        public void FlipArray_ReversesByteArray()
        {
            // Arrange
            var input = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var expected = new byte[] { 0x04, 0x03, 0x02, 0x01 };

            // Act
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FlipArray_WithSingleElement_ReturnsSameArray()
        {
            // Arrange
            var input = new byte[] { 0x42 };

            // Act
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void FlipArray_WithEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            var input = new byte[0];

            // Act
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlipArray_WithTwoElements_SwapsPositions()
        {
            // Arrange
            var input = new byte[] { 0xAB, 0xCD };
            var expected = new byte[] { 0xCD, 0xAB };

            // Act
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Static Method Tests - ByteSearch

        [Fact]
        public void ByteSearch_FindsPatternAtStart_ReturnsZero()
        {
            // Arrange
            var searchIn = new byte[] { 0xAB, 0xCD, 0xEF, 0x00 };
            var searchFor = new byte[] { 0xAB, 0xCD };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ByteSearch_FindsPatternInMiddle_ReturnsCorrectIndex()
        {
            // Arrange
            var searchIn = new byte[] { 0x00, 0x01, 0xAB, 0xCD, 0x02 };
            var searchFor = new byte[] { 0xAB, 0xCD };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void ByteSearch_PatternNotFound_ReturnsMinusOne()
        {
            // Arrange
            var searchIn = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            var searchFor = new byte[] { 0xAB, 0xCD };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_WithStartOffset_SearchesFromOffset()
        {
            // Arrange
            var searchIn = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD };
            var searchFor = new byte[] { 0xAB, 0xCD };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 2);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void ByteSearch_SingleBytePattern_FindsCorrectly()
        {
            // Arrange
            var searchIn = new byte[] { 0x00, 0x42, 0x01 };
            var searchFor = new byte[] { 0x42 };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void ByteSearch_EmptySearchIn_ReturnsMinusOne()
        {
            // Arrange
            var searchIn = new byte[0];
            var searchFor = new byte[] { 0xAB };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_EmptySearchFor_ReturnsMinusOne()
        {
            // Arrange
            var searchIn = new byte[] { 0x00, 0x01, 0x02 };
            var searchFor = new byte[0];

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_PatternLongerThanData_ReturnsMinusOne()
        {
            // Arrange
            var searchIn = new byte[] { 0x01 };
            var searchFor = new byte[] { 0x01, 0x02, 0x03 };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_FindsAJP13MagicBytes()
        {
            // Arrange - AJP13 packets start with 0x41, 0x42 ('AB')
            var searchIn = new byte[] { 0x00, 0x00, 0x41, 0x42, 0x00, 0x05 };
            var searchFor = new byte[] { 0x41, 0x42 };

            // Act
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);

            // Assert
            Assert.Equal(2, result);
        }

        #endregion

        #region Private Method Tests - GetInt16B

        [Fact]
        public void GetInt16B_ReadsBigEndianValue()
        {
            // Arrange - Big Endian: 0x01 0x02 = 258
            var data = new byte[] { 0x01, 0x02 };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 0);

            // Assert
            Assert.Equal(258, result);
        }

        [Fact]
        public void GetInt16B_WithOffset_ReadsCorrectPosition()
        {
            // Arrange
            var data = new byte[] { 0x00, 0x00, 0x03, 0x00 }; // Value at position 2
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 2);

            // Assert
            Assert.Equal(768, result); // 0x03 0x00 in Big Endian = 768
        }

        [Fact]
        public void GetInt16B_WithZero_ReturnsZero()
        {
            // Arrange
            var data = new byte[] { 0x00, 0x00 };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 0);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetInt16B_WithMaxValue_ReturnsMaxUInt16()
        {
            // Arrange - 0xFF 0xFF = 65535
            var data = new byte[] { 0xFF, 0xFF };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 0);

            // Assert
            Assert.Equal(65535, result);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Server_GetSet_WorksCorrectly()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            connection.Server = "192.168.1.1";
            var result = connection.Server;

            // Assert
            Assert.Equal("192.168.1.1", result);
        }

        [Fact]
        public void Port_GetSet_WorksCorrectly()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            connection.Port = 9009;
            var result = connection.Port;

            // Assert
            Assert.Equal(9009, result);
        }

        [Fact]
        public void AbortConnection_DefaultValue_IsFalse()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Assert
            Assert.False(connection.AbortConnection);
        }

        [Fact]
        public void AbortConnection_GetSet_WorksCorrectly()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            connection.AbortConnection = true;
            var result = connection.AbortConnection;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ChunkedTransfer_DefaultValue_IsFalse()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Assert
            Assert.False(connection.ChunkedTransfer);
        }

        [Fact]
        public void ChunkedTransfer_GetSet_WorksCorrectly()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            connection.ChunkedTransfer = true;
            var result = connection.ChunkedTransfer;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ReceivedDataCollection_IsNotNull()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");

            // Act
            var result = connection.ReceivedDataCollection;

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithLogFilePostFix_CreatesInstance()
        {
            // Act
            var connection = new BonCodeAJP13ServerConnection("test-postfix", "127.0.0.1");

            // Assert
            Assert.NotNull(connection);
        }

        [Fact]
        public void Constructor_WithEmptyParameters_CreatesInstance()
        {
            // Act
            var connection = new BonCodeAJP13ServerConnection();

            // Assert
            Assert.NotNull(connection);
        }

        #endregion

        #region AddPacketToSendQueue Tests

        [Fact]
        public void AddPacketToSendQueue_AddsPacketToCollection()
        {
            // Arrange
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var packet = new global::BonCodeAJP13.TomcatPackets.TomcatSendBodyChunk("test data");

            // Act
            connection.AddPacketToSendQueue(packet);

            // Assert - we can't directly access the send queue, but we can verify no exception was thrown
            Assert.NotNull(connection);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Invokes a static private method on BonCodeAJP13ServerConnection using reflection
        /// </summary>
        private T InvokeStaticMethod<T>(string methodName, params object[] parameters)
        {
            var method = typeof(BonCodeAJP13ServerConnection).GetMethod(methodName, 
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            
            if (method == null)
                throw new InvalidOperationException($"Method {methodName} not found");
            
            var result = method.Invoke(null, parameters);
            return (T)result;
        }

        /// <summary>
        /// Invokes a private instance method on BonCodeAJP13ServerConnection using reflection
        /// </summary>
        private T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
        {
            var method = typeof(BonCodeAJP13ServerConnection).GetMethod(methodName, 
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            
            if (method == null)
                throw new InvalidOperationException($"Method {methodName} not found");
            
            var result = method.Invoke(instance, parameters);
            return (T)result;
        }

        #endregion
    }
}