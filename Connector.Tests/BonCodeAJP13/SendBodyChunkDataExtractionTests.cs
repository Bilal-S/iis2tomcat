using System;
using System.Linq;
using Xunit;
using BonCodeAJP13.TomcatPackets;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Tests for GetUserDataBytes() in TomcatSendBodyChunk.
    ///
    /// The payload layout passed by AnalyzePackage is:
    ///   [0x03] [chunk_length_hi] [chunk_length_lo] [body_data...]
    /// GetUserDataBytes() should skip the 3-byte prefix and return all body data.
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

        [Fact]
        public void GetUserDataBytes_SmallBody5Bytes_ReturnsAll5Bytes()
        {
            var body = SequentialBytes(5);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Equal(5, result.Length);
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMultipleOf4_ReturnsAllBytes()
        {
            var body = SequentialBytes(8);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Equal(8, result.Length);
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMod4Equals1_ReturnsAllBytes()
        {
            var body = SequentialBytes(9);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Equal(9, result.Length);
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMod4Equals2_ReturnsAllBytes()
        {
            var body = SequentialBytes(10);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Equal(10, result.Length);
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_BodySizeMod4Equals3_ReturnsAllBytes()
        {
            var body = SequentialBytes(11);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Equal(11, result.Length);
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_LargeBody9000Bytes_ReturnsAll9000Bytes()
        {
            var body = SequentialBytes(9000);
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Equal(9000, result.Length);
            Assert.Equal(body, result);
        }

        [Fact]
        public void GetUserDataBytes_SingleByteBody_ReturnsThatByte()
        {
            var body = new byte[] { 0xAB };
            var userData = BuildSendBodyChunkUserData(body);
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Single(result);
            Assert.Equal(new byte[] { 0xAB }, result);
        }

        [Fact]
        public void GetUserDataBytes_EmptyChunk_ReturnsEmptyArray()
        {
            var userData = new byte[] { 0x03, 0x00, 0x00 };
            var packet = new TomcatSendBodyChunk(userData);

            var result = packet.GetUserDataBytes();

            Assert.Empty(result);
        }

        [Fact]
        public void UserDataLength_Property_ReturnsCorrectChunkLength()
        {
            var body = SequentialBytes(100);
            var userData = BuildSendBodyChunkUserData(body); // 103 bytes total
            var packet = new TomcatSendBodyChunk(userData);

            ushort actualUserDataLength = packet.UserDataLength;
            ushort expectedUserDataLength = 100;

            Assert.Equal(expectedUserDataLength, actualUserDataLength);
        }
    }
}