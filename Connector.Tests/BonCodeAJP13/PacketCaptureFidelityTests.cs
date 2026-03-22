using System;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using BonCodeAJP13;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Validates that BonCodeAJP13Logger.LogPacketBytes() produces byte-for-byte
    /// exact copies of the input data on disk. This proves that .pak capture files
    /// are wire-faithful — any patterns observed in captured files (e.g. Lucee's
    /// trail=1) reflect what the server actually sent, not a capture artifact.
    /// </summary>
    [Collection("Sequential")]
    public class PacketCaptureFidelityTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly int _originalLogLevel;
        private readonly string _originalLogDir;
        private readonly Mutex _dummyMutex;

        public PacketCaptureFidelityTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "BonCodeCaptureFidelity_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);

            _originalLogLevel = BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL;
            _originalLogDir = BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR;

            // Enable packet capture and redirect output to temp dir
            BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_PACKETS;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = _tempDir;

            _dummyMutex = new Mutex();
        }

        public void Dispose()
        {
            BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL = _originalLogLevel;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = _originalLogDir;

            _dummyMutex.Dispose();

            try
            {
                if (Directory.Exists(_tempDir))
                    Directory.Delete(_tempDir, true);
            }
            catch { /* ignore cleanup failures */ }
        }

        /// <summary>
        /// Build a synthetic AJP SendBodyChunk wire packet with spec-compliant padding.
        /// Format: [AB][payloadLen][03][chunkLen_hi][chunkLen_lo][body...][padding...]
        /// </summary>
        private static byte[] BuildSendBodyChunk(int chunkLen, byte fillByte = 0x42)
        {
            int padding = (4 - (chunkLen % 4)) % 4;
            int payloadLen = 1 + 2 + chunkLen + padding; // type + chunk_len + body + padding
            int totalLen = 4 + payloadLen;               // magic + length + payload

            byte[] packet = new byte[totalLen];

            // AJP header (Container->Server)
            packet[0] = 0x41; // 'A'
            packet[1] = 0x42; // 'B'
            packet[2] = (byte)((payloadLen >> 8) & 0xFF);
            packet[3] = (byte)(payloadLen & 0xFF);

            // SendBodyChunk payload
            packet[4] = 0x03; // type = SendBodyChunk
            packet[5] = (byte)((chunkLen >> 8) & 0xFF);
            packet[6] = (byte)(chunkLen & 0xFF);

            // Body data
            for (int i = 0; i < chunkLen; i++)
            {
                packet[7 + i] = fillByte;
            }

            // Padding bytes remain 0x00 (default for byte[])

            return packet;
        }

        /// <summary>
        /// Build a synthetic AJP SendHeaders wire packet (type 0x04).
        /// </summary>
        private static byte[] BuildSendHeaders()
        {
            byte[] payload = new byte[] {
                0x04,       // type = SendHeaders
                0x00, 0xC8, // status = 200
                0x00, 0x02, 0x4F, 0x4B, // status_msg = "OK\0" (length=2, then "OK\0")
                0x00, 0x00  // num_headers = 0
            };
            int payloadLen = payload.Length;
            byte[] packet = new byte[4 + payloadLen];
            packet[0] = 0x41;
            packet[1] = 0x42;
            packet[2] = (byte)((payloadLen >> 8) & 0xFF);
            packet[3] = (byte)(payloadLen & 0xFF);
            Array.Copy(payload, 0, packet, 4, payloadLen);
            return packet;
        }

        private void AssertPakFileMatches(string pakPath, byte[] expectedBytes, string description)
        {
            Assert.True(File.Exists(pakPath), $"{description}: .pak file should exist at {pakPath}");
            byte[] actualBytes = File.ReadAllBytes(pakPath);
            Assert.Equal(expectedBytes.Length, actualBytes.Length);
            Assert.Equal(expectedBytes, actualBytes);
        }

        // ---------------------------------------------------------------
        // Direct byte[] overload — no padding (5000 bytes)
        // Critical: if capture drops 1 byte here, the bug diagnosis
        // would be wrong (capture bug, not GetUserDataBytes)
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_DirectOverload_NoPadding_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            byte[] packet = BuildSendBodyChunk(5000, 0x42); // 5000 % 4 = 0, padding = 0, total = 5007

            logger.LogPacketBytes(packet, 99, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "5000B pad=0");

            // Verify .pak.meta companion exists
            string metaPath = pakFiles[0] + ".meta";
            Assert.True(File.Exists(metaPath), "Meta file should exist");
            string meta = File.ReadAllText(metaPath);
            Assert.Contains("PacketType: 0x", meta);
            Assert.Contains("ByteCount: 5007", meta);
        }

        // ---------------------------------------------------------------
        // 1 byte padding (7 bytes body) — the Lucee case
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_DirectOverload_Padding1_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            byte[] packet = BuildSendBodyChunk(7, 0x37); // 7 % 4 = 3, padding = 1, total = 12

            logger.LogPacketBytes(packet, 99, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "7B pad=1");
        }

        // ---------------------------------------------------------------
        // 2 bytes padding (6 bytes body)
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_DirectOverload_Padding2_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            byte[] packet = BuildSendBodyChunk(6, 0x36); // 6 % 4 = 2, padding = 2, total = 13

            logger.LogPacketBytes(packet, 99, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "6B pad=2");
        }

        // ---------------------------------------------------------------
        // 3 bytes padding (5 bytes body) — maximum padding
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_DirectOverload_Padding3_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            byte[] packet = BuildSendBodyChunk(5, 0x35); // 5 % 4 = 1, padding = 3, total = 13

            logger.LogPacketBytes(packet, 99, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "5B pad=3");
        }

        // ---------------------------------------------------------------
        // Non-SendBodyChunk packet (SendHeaders type 0x04)
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_DirectOverload_SendHeaders_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            byte[] packet = BuildSendHeaders();

            logger.LogPacketBytes(packet, 42, 0x04);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "SendHeaders");
        }

        // ---------------------------------------------------------------
        // Slice overload — simulates how AnalyzePackage calls it
        // AnalyzePackage passes (receiveBuffer, iStart, packetLength+4, ...)
        // Tests the Array.Copy path inside LogPacketBytes
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_SliceOverload_ExtractsExactBytes()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);

            // Two packets concatenated (simulates TCP read returning multiple packets)
            byte[] packet1 = BuildSendBodyChunk(8, 0x41); // 8 % 4 = 0, padding = 0, total = 15
            byte[] packet2 = BuildSendBodyChunk(3, 0x42); // 3 % 4 = 3, padding = 1, total = 11

            byte[] combined = new byte[packet1.Length + packet2.Length];
            Array.Copy(packet1, 0, combined, 0, packet1.Length);
            Array.Copy(packet2, 0, combined, packet1.Length, packet2.Length);

            // Capture packet1 via slice
            logger.LogPacketBytes(combined, 0, packet1.Length, 10, 0x03);
            // Capture packet2 via slice
            logger.LogPacketBytes(combined, packet1.Length, packet2.Length, 10, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Equal(2, pakFiles.Length);

            // Match by size + content
            byte[][] actuals = pakFiles.Select(f => File.ReadAllBytes(f)).ToArray();
            bool packet1Matched = actuals.Any(a => a.Length == packet1.Length && a.SequenceEqual(packet1));
            bool packet2Matched = actuals.Any(a => a.Length == packet2.Length && a.SequenceEqual(packet2));

            Assert.True(packet1Matched, "Slice capture of packet1 (8B body, pad=0) should match exactly");
            Assert.True(packet2Matched, "Slice capture of packet2 (3B body, pad=1) should match exactly");
        }

        // ---------------------------------------------------------------
        // Slice with non-zero offset — ensures offset doesn't shift bytes
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_SliceOverload_NonZeroOffset_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);

            byte[] prefix = new byte[] { 0xFF, 0xFF, 0xFF };
            byte[] packet = BuildSendBodyChunk(4, 0x44); // 4 % 4 = 0, padding = 0, total = 11
            byte[] suffix = new byte[] { 0xAA, 0xAA };

            byte[] buffer = new byte[prefix.Length + packet.Length + suffix.Length];
            Array.Copy(prefix, 0, buffer, 0, prefix.Length);
            Array.Copy(packet, 0, buffer, prefix.Length, packet.Length);
            Array.Copy(suffix, 0, buffer, prefix.Length + packet.Length, suffix.Length);

            logger.LogPacketBytes(buffer, prefix.Length, packet.Length, 7, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "Slice at offset=3, 4B body pad=0");
        }

        // ---------------------------------------------------------------
        // Large packet — 8188 bytes body (max single SendBodyChunk)
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_DirectOverload_LargePacket_8188Bytes_NoPadding_ExactCopy()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            byte[] packet = BuildSendBodyChunk(8188, 0xAB); // 8188 % 4 = 0, padding = 0, total = 8195

            logger.LogPacketBytes(packet, 1, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);
            AssertPakFileMatches(pakFiles[0], packet, "8188B pad=0 (near max)");
        }

        // ---------------------------------------------------------------
        // Null/empty input — should not crash or create files
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_NullInput_NoFileCreated()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            logger.LogPacketBytes(null, 1, 0x03);
            Assert.Empty(Directory.GetFiles(_tempDir, "*.pak"));
        }

        [Fact]
        public void LogPacketBytes_EmptyInput_NoFileCreated()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);
            logger.LogPacketBytes(new byte[0], 1, 0x03);
            Assert.Empty(Directory.GetFiles(_tempDir, "*.pak"));
        }

        // ---------------------------------------------------------------
        // Sequential byte pattern — proves no byte reordering
        // Uses pattern 0x01..0xFE repeating (same as CFM test pages)
        // ---------------------------------------------------------------
        [Fact]
        public void LogPacketBytes_SequentialPattern_ByteOrderPreserved()
        {
            var logger = new BonCodeAJP13Logger("test_fidelity", _dummyMutex);

            int chunkLen = 5000;
            int padding = (4 - (chunkLen % 4)) % 4;
            int payloadLen = 1 + 2 + chunkLen + padding;
            int totalLen = 4 + payloadLen;
            byte[] packet = new byte[totalLen];

            packet[0] = 0x41; packet[1] = 0x42;
            packet[2] = (byte)((payloadLen >> 8) & 0xFF);
            packet[3] = (byte)(payloadLen & 0xFF);
            packet[4] = 0x03;
            packet[5] = (byte)((chunkLen >> 8) & 0xFF);
            packet[6] = (byte)(chunkLen & 0xFF);

            for (int i = 0; i < chunkLen; i++)
            {
                packet[7 + i] = (byte)((i % 254) + 1);
            }
            // padding bytes stay 0x00

            logger.LogPacketBytes(packet, 1, 0x03);

            var pakFiles = Directory.GetFiles(_tempDir, "*.pak");
            Assert.Single(pakFiles);

            byte[] actual = File.ReadAllBytes(pakFiles[0]);
            Assert.Equal(packet.Length, actual.Length);

            // Verify every body byte is in correct order
            for (int i = 0; i < chunkLen; i++)
            {
                byte expected = (byte)((i % 254) + 1);
                Assert.Equal(expected, actual[7 + i]);
            }

            // Verify padding bytes are zero
            for (int i = 0; i < padding; i++)
            {
                Assert.Equal(0x00, actual[7 + chunkLen + i]);
            }
        }
    }
}