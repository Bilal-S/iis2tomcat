using System;
using System.Linq;
using Xunit;
using BonCodeAJP13.TomcatPackets;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Tests that prove GetUserDataBytes() in BonCodeAJP13Packet drops the last byte
    /// of every Send Body Chunk, corrupting binary responses.
    ///
    /// Background: AnalyzePackage strips the 4-byte AJP header (AB + length) and
    /// passes only the payload to TomcatSendBodyChunk. The payload layout is:
    ///   [0x03] [chunk_length_hi] [chunk_length_lo] [body_data...]
    /// GetUserDataBytes() starts at index 3 (correct) but trims 4 bytes total
    /// (MIN_PACKET_DATA_LENGTH=3 start + PACKET_CONTROL_BYTES=4 end = net -1).
    /// This always drops the final byte of body data.
    /// </summary>
    public class SendBodyChunkDataExtractionTests
    {
        /// <summary>
        /// Builds the exact byte array that AnalyzePackage would pass to
        /// TomcatSendBodyChunk(byte[] content): [0x03][length_hi][length_lo][body...]
        /// </summary>
        private static byte[] BuildSendBodyChunkUserData(byte[] body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            int chunkLength = body.Length;
            var userData = new byte[3 + chunkLength];
            userData[0] = 0x03; // TOMCAT_SENDBODYCHUNK prefix code
            userData[1] = (byte)((chunkLength >> 8) & 0xFF); // length high byte (big-endian)
            userData[2] = (byte)(chunkLength & 0xFF);        // length low byte
            Array.Copy(body, 0, userData, 3, chunkLength);
            return userData;
        }

        /// <summary>
        /// Creates a sequential byte pattern of the given length: 0x01, 0x02, 0x03, ...
        /// This makes truncation clearly visible since each byte has a unique value.
        /// </summary>
        private static byte[] SequentialBytes(int length)
        {
            var bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)((i % 254) + 1); // 0x01..0xFE repeating
            }
            return bytes;
        }

        #region Bug-demonstrating tests (all should FAIL with current code)

        [Fact]
        public void GetUserDataBytes_SmallBody5Bytes_ReturnsAll5Bytes()
        {
            // Arrange — body of 5 bytes: 0x01 0x02 0x03 0x04 0x05
            var body = SequentialBytes(5);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Equal(5, result.Length);  // BUG: currently returns 4
            Assert.Equal(body, result);       // BUG: last byte 0x05 is dropped
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMultipleOf4_ReturnsAllBytes()
        {
            // Arrange — 8 bytes (8 % 4 == 0), disproves padding theory
            var body = SequentialBytes(8);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Equal(8, result.Length);  // BUG: currently returns 7
            Assert.Equal(body, result);       // BUG: last byte 0x08 is dropped
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMod4Equals1_ReturnsAllBytes()
        {
            // Arrange — 9 bytes (9 % 4 == 1)
            var body = SequentialBytes(9);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Equal(9, result.Length);  // BUG: currently returns 8
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMod4Equals2_ReturnsAllBytes()
        {
            // Arrange — 10 bytes (10 % 4 == 2)
            var body = SequentialBytes(10);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Equal(10, result.Length); // BUG: currently returns 9
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMod4Equals3_ReturnsAllBytes()
        {
            // Arrange — 11 bytes (11 % 4 == 3)
            var body = SequentialBytes(11);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Equal(11, result.Length); // BUG: currently returns 10
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_LargeBody9000Bytes_ReturnsAll9000Bytes()
        {
            // Arrange — 9000 bytes created by loop, clearly shows 1-byte truncation
            var body = SequentialBytes(9000);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Equal(9000, result.Length); // BUG: currently returns 8999
            Assert.Equal(body, result);         // BUG: final byte is dropped
        }

        [Fact]
        public void GetUserDataBytes_SingleByteBody_ReturnsThatByte()
        {
            // Arrange — 1 byte body: edge case where 100% of data is lost
            var body = new byte[] { 0xAB };
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            Assert.Single(result);    // BUG: currently returns 0 (empty!)
            Assert.Equal(new byte[] { 0xAB }, result);
        }

        #endregion

        #region Tests that should PASS with current code

        [Fact]
        public void GetUserDataBytes_EmptyChunk_ReturnsEmptyArray()
        {
            // Arrange — AJP flush marker: type=0x03, length=0, no body data
            var userData = new byte[] { 0x03, 0x00, 0x00 };
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert — p_ByteStore.Length (3) is NOT > MIN_PACKET_DATA_LENGTH (3),
            // so the method returns empty array. This case happens to be correct.
            Assert.Empty(result);
        }

        #endregion

        #region Diagnostic tests — measure exact corruption for clarity

        [Fact]
        public void GetUserDataBytes_Diagnostic_ShowsExactByteLost_For8ByteBody()
        {
            // Arrange — body: 0x01 0x02 0x03 0x04 0x05 0x06 0x07 0x08
            var body = SequentialBytes(8);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert — with current bug: result is 7 bytes, missing last byte 0x08
            // These assertions document the exact corruption. They will flip
            // from fail→pass once the bug is fixed.
            int expectedLength = 8;
            int actualLength = result.Length;
            int bytesLost = expectedLength - actualLength;

            Assert.True(bytesLost == 0,
                $"Expected {expectedLength} bytes but got {actualLength}. " +
                $"Lost {bytesLost} byte(s). " +
                $"Missing byte value: 0x{body[actualLength]:X2}. " +
                $"Actual last byte in result: {(actualLength > 0 ? "0x" + result[actualLength - 1].ToString("X2") : "N/A")}. " +
                $"Full result: {BitConverter.ToString(result)}");
        }

        [Fact]
        public void GetUserDataBytes_Diagnostic_ShowsExactByteLost_For9000ByteBody()
        {
            // Arrange — 9000 sequential bytes
            var body = SequentialBytes(9000);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            var result = packet.GetUserDataBytes();

            // Assert
            int expectedLength = 9000;
            int actualLength = result.Length;
            int bytesLost = expectedLength - actualLength;

            Assert.True(bytesLost == 0,
                $"Expected {expectedLength} bytes but got {actualLength}. " +
                $"Lost {bytesLost} byte(s). " +
                $"Missing byte value: 0x{body[actualLength]:X2} (expected index {actualLength}). " +
                $"Actual last byte: 0x{result[actualLength - 1]:X2} (should be 0x{(actualLength % 254):X2} at index {actualLength - 1}).");
        }

        [Fact]
        public void UserDataLength_Property_IsAlsoWrong_ForSendBodyChunk()
        {
            // Arrange — p_UserDataLength is set as content.Length - 4 in the constructor
            // but should be the chunk_length parsed from bytes 1-2
            var body = SequentialBytes(100);
            var userData = BuildSendBodyChunkUserData(body); // 103 bytes total
            var packet = new TomcatSendBodyChunk(userData);

            // Act
            ushort actualUserDataLength = packet.UserDataLength;
            ushort expectedUserDataLength = 100; // the actual body size from the chunk_length field

            // Assert — currently: 103 - 4 = 99 (wrong), should be 100
            Assert.Equal(expectedUserDataLength, actualUserDataLength);
        }

        #endregion
    }
}