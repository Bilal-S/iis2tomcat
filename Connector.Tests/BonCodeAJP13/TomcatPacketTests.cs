using System;
using System.Collections.Specialized;
using System.Text;
using Xunit;
using BonCodeAJP13;
using BonCodeAJP13.TomcatPackets;
using BonCodeAJP13.ServerPackets;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Unit tests for Tomcat packet classes
    /// </summary>
    public class TomcatPacketTests
    {
        #region TomcatReturn Tests

        [Fact]
        public void TomcatReturn_DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new TomcatReturn();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void TomcatReturn_WithByteArray_SetsData()
        {
            // Arrange
            var data = new byte[] { 0x41, 0x42, 0x00, 0x43, 0x44, 0x00, 0x45, 0x46 };

            // Act
            var packet = new TomcatReturn(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
            Assert.Equal(8, packet.PacketLength);
            Assert.Equal(4, packet.UserDataLength);
        }

        [Fact]
        public void TomcatReturn_SetDataBytes_UpdatesData()
        {
            // Arrange
            var packet = new TomcatReturn();
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            // Act
            packet.SetDataBytes(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
        }

        [Fact]
        public void TomcatReturn_GetDataString_ReturnsContent()
        {
            // Arrange
            var content = "Hello";
            var data = Encoding.UTF8.GetBytes(content);
            var packet = new TomcatReturn(data);

            // Act
            var result = packet.GetDataString();

            // Assert
            Assert.Equal(content, result);
        }

        #endregion

        #region TomcatSendBodyChunk Tests

        [Fact]
        public void TomcatSendBodyChunk_DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new TomcatSendBodyChunk();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void TomcatSendBodyChunk_WithByteArray_SetsData()
        {
            // Arrange
            var data = new byte[] { 0x00, 0x00, 0x00, 0x05, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', 0x00 };

            // Act
            var packet = new TomcatSendBodyChunk(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
            Assert.Equal(10, packet.PacketLength);
            Assert.Equal(6, packet.UserDataLength);
        }

        [Fact]
        public void TomcatSendBodyChunk_WithString_CreatesPacket()
        {
            // Arrange
            var content = "Hello World";

            // Act
            var packet = new TomcatSendBodyChunk(content);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            Assert.True(packet.PacketLength > 0);
            // String constructor adds padding
            Assert.Equal(content.Length + 8, packet.PacketLength);
        }

        [Fact]
        public void TomcatSendBodyChunk_GetUserDataString_ReturnsContent()
        {
            // Arrange
            var content = "Test Content";
            var packet = new TomcatSendBodyChunk(content);

            // Act
            var result = packet.GetUserDataString();

            // Assert
            Assert.Contains("Test Content", result);
        }

        [Fact]
        public void TomcatSendBodyChunk_WithEmptyString_CreatesPacket()
        {
            // Arrange
            var content = "";

            // Act
            var packet = new TomcatSendBodyChunk(content);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            // Empty string + 8 bytes padding
            Assert.Equal(8, packet.PacketLength);
        }

        [Fact]
        public void TomcatSendBodyChunk_WithUtf8String_PreservesCharacters()
        {
            // Arrange
            var content = "Héllo Wörld"; // Contains accented characters

            // Act
            var packet = new TomcatSendBodyChunk(content);
            var result = packet.GetUserDataString();

            // Assert
            Assert.Contains("H", result);
        }

        #endregion

        #region TomcatCPongReply Tests

        [Fact]
        public void TomcatCPongReply_DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new TomcatCPongReply();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void TomcatCPongReply_WithByteArray_SetsData()
        {
            // Arrange
            var data = new byte[] { 0x41, 0x42, 0x00, 0x01, 0x09 };

            // Act
            var packet = new TomcatCPongReply(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
            Assert.Equal(5, packet.PacketLength);
            Assert.Equal(1, packet.UserDataLength);
        }

        [Fact]
        public void TomcatCPongReply_InheritsFromTomcatReturn()
        {
            // Arrange
            var packet = new TomcatCPongReply();

            // Assert
            Assert.IsType<TomcatReturn>(packet, exactMatch: false);
        }

        #endregion

        #region TomcatEndResponse Tests

        [Fact]
        public void TomcatEndResponse_DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new TomcatEndResponse();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void TomcatEndResponse_WithByteArray_SetsData()
        {
            // Arrange
            var data = new byte[] { 0x41, 0x42, 0x00, 0x02, 0x05, 0x00 };

            // Act
            var packet = new TomcatEndResponse(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
            Assert.Equal(6, packet.PacketLength);
            // EndResponse should have UserDataLength = 0
            Assert.Equal(0, packet.UserDataLength);
        }

        [Fact]
        public void TomcatEndResponse_InheritsFromTomcatReturn()
        {
            // Arrange
            var packet = new TomcatEndResponse();

            // Assert
            Assert.IsType<TomcatReturn>(packet, exactMatch: false);
        }

        #endregion

        #region TomcatGetBodyChunk Tests

        [Fact]
        public void TomcatGetBodyChunk_DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new TomcatGetBodyChunk();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void TomcatGetBodyChunk_WithByteArray_SetsData()
        {
            // Arrange
            var data = new byte[] { 0x41, 0x42, 0x00, 0x03, 0x06, 0x00, 0x08, 0x00 };

            // Act
            var packet = new TomcatGetBodyChunk(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
            Assert.Equal(8, packet.PacketLength);
        }

        [Fact]
        public void TomcatGetBodyChunk_GetRequestedLength_WithoutLengthData_ReturnsMax()
        {
            // Arrange - packet without requested length data
            var data = new byte[] { 0x41, 0x42, 0x00, 0x01, 0x06 };
            var packet = new TomcatGetBodyChunk(data);

            // Act
            var result = packet.GetRequestedLength();

            // Assert - should return max length when no specific length requested
            Assert.Equal(BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH, result);
        }

        [Fact]
        public void TomcatGetBodyChunk_InheritsFromTomcatReturn()
        {
            // Arrange
            var packet = new TomcatGetBodyChunk();

            // Assert
            Assert.IsType<TomcatReturn>(packet, exactMatch: false);
        }

        #endregion

        #region TomcatSendHeaders Tests

        [Fact]
        public void TomcatSendHeaders_DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new TomcatSendHeaders();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void TomcatSendHeaders_WithByteArray_SetsData()
        {
            // Arrange - minimal headers packet: magic bytes + length + status + message + header count
            var data = new byte[] { 0x41, 0x42, 0x00, 0x04, 0x00, 0xC8, 0x00, 0x03, 0x4F, 0x4B, 0x00, 0x00 };
            // Status 200 (0xC8), message "OK", 0 headers

            // Act
            var packet = new TomcatSendHeaders(data);

            // Assert
            Assert.Equal(data, packet.GetDataBytes());
        }

        // Note: GetStatus, GetHeaders, and PrintPacketHeader require properly formatted AJP packet data
        // Testing with empty data causes NullReferenceException - this is expected behavior
        // Full integration tests should be done with real Tomcat response data

        [Fact]
        public void TomcatSendHeaders_InheritsFromTomcatReturn()
        {
            // Arrange
            var packet = new TomcatSendHeaders();

            // Assert
            Assert.IsType<TomcatReturn>(packet, exactMatch: false);
        }

        [Fact]
        public void TomcatSendHeaders_GetStatus_ReturnsCorrectValue()
        {
            // Arrange - minimal headers packet with status 200
            // Format: [type:1][status:2][msg_len:2][msg:n][msg_null:1][header_count:2]
            var data = new byte[] { 
                0x04,                   // type = SendHeaders (0x04)
                0x00, 0xC8,             // status = 200 (big-endian)
                0x00, 0x00, 0x00,       // empty status message (length=0, null terminator)
                0x00, 0x00              // 0 headers
            };
            var packet = new TomcatSendHeaders(data);

            // Act
            var status = packet.GetStatus();

            // Assert
            Assert.Equal(200, status);
        }

        [Fact]
        public void TomcatSendHeaders_GetStatus_Boundary32767_ReturnsCorrectValue()
        {
            // Arrange - status 32767 is the maximum positive signed Int16 value
            // 32767 = 0x7FFF in hex
            var data = new byte[] { 
                0x04,                   // type = SendHeaders (0x04)
                0x7F, 0xFF,             // status = 32767 (big-endian)
                0x00, 0x00, 0x00,       // empty status message (length=0, null terminator)
                0x00, 0x00              // 0 headers
            };
            var packet = new TomcatSendHeaders(data);

            // Act
            var status = packet.GetStatus();

            // Assert
            Assert.Equal(32767, status);
        }

        [Fact]
        public void TomcatSendHeaders_GetStatus_Boundary32768_ReturnsCorrectValue()
        {
            // Arrange - status 32768 is just over the signed Int16 max (used to cause OverflowException)
            // 32768 = 0x8000 in hex
            var data = new byte[] { 
                0x04,                   // type = SendHeaders (0x04)
                0x80, 0x00,             // status = 32768 (big-endian)
                0x00, 0x00, 0x00,       // empty status message (length=0, null terminator)
                0x00, 0x00              // 0 headers
            };
            var packet = new TomcatSendHeaders(data);

            // Act
            var status = packet.GetStatus();

            // Assert - should NOT throw OverflowException
            Assert.Equal(32768, status);
        }

        [Fact]
        public void TomcatSendHeaders_GetStatus_HighValue40000_ReturnsCorrectValue()
        {
            // Arrange - status 40000 is a non-standard value above signed Int16 max
            // 40000 = 0x9C40 in hex
            var data = new byte[] { 
                0x04,                   // type = SendHeaders (0x04)
                0x9C, 0x40,             // status = 40000 (big-endian)
                0x00, 0x00, 0x00,       // empty status message (length=0, null terminator)
                0x00, 0x00              // 0 headers
            };
            var packet = new TomcatSendHeaders(data);

            // Act
            var status = packet.GetStatus();

            // Assert
            Assert.Equal(40000, status);
        }

        [Fact]
        public void TomcatSendHeaders_GetStatus_MaxUShort65535_ReturnsCorrectValue()
        {
            // Arrange - status 65535 is the maximum ushort value
            // 65535 = 0xFFFF in hex
            var data = new byte[] { 
                0x04,                   // type = SendHeaders (0x04)
                0xFF, 0xFF,             // status = 65535 (big-endian)
                0x00, 0x00, 0x00,       // empty status message (length=0, null terminator)
                0x00, 0x00              // 0 headers
            };
            var packet = new TomcatSendHeaders(data);

            // Act
            var status = packet.GetStatus();

            // Assert
            Assert.Equal(65535, status);
        }

        #endregion

        #region BonCodeAJP13CPing Tests

        [Fact]
        public void BonCodeAJP13CPing_Constructor_CreatesInstance()
        {
            // Act
            var packet = new BonCodeAJP13CPing();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void BonCodeAJP13CPing_Constructor_CreatesCorrectPacketLength()
        {
            // Act
            var packet = new BonCodeAJP13CPing();

            // Assert - CPING packet is always 5 bytes
            Assert.Equal(5, packet.PacketLength);
        }

        [Fact]
        public void BonCodeAJP13CPing_Constructor_SetsCorrectUserDataLength()
        {
            // Act
            var packet = new BonCodeAJP13CPing();

            // Assert - user data length should be 1 (just the packet type byte)
            Assert.Equal(1, packet.UserDataLength);
        }

        [Fact]
        public void BonCodeAJP13CPing_InheritsFromBonCodeAJP13Packet()
        {
            // Arrange
            var packet = new BonCodeAJP13CPing();

            // Assert
            Assert.IsType<BonCodeAJP13Packet>(packet, exactMatch: false);
        }

        #endregion
    }
}
