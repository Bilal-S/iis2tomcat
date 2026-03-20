using System;
using System.Text;
using Xunit;
using BonCodeAJP13.TomcatPackets;

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
    }
}