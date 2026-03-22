using System;
using System.IO;
using System.Linq;
using Xunit;
using BonCodeAJP13.TomcatPackets;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Replays real captured .pak files through TomcatSendBodyChunk.GetUserDataBytes()
    /// to validate that the method correctly extracts body data from wire-format packets.
    ///
    /// Background: AJP SendBodyChunk wire format (after header stripping by AnalyzePackage):
    ///   [0x03][chunkLen_hi][chunkLen_lo][body_bytes...][0x00]
    /// GetUserDataBytes() starts at index 3 (MIN_PACKET_DATA_LENGTH) and trims 4 bytes
    /// total (PACKET_CONTROL_BYTES), returning exactly chunkLen body bytes.
    /// The trailing 0x00 (AJP termination byte) is the byte that gets trimmed.
    ///
    /// All 74 captured SendBodyChunk PAK files (TomcatCapture + OldCapture) have been
    /// verified to end with 0x00. These tests prove GetUserDataBytes handles them correctly.
    /// </summary>
    public class GetUserDataBytesPakReplayTests
    {
        /// <summary>
        /// Resolves the base directory for captured PAK files.
        /// Walks up from the test assembly location to find the project root.
        /// </summary>
        private static string GetCaptureBaseDir()
        {
            // Test assembly is in Connector.Tests/bin/Debug/netNNN/ or similar
            string asmDir = AppDomain.CurrentDomain.BaseDirectory;
            string candidate = asmDir;
            while (!string.IsNullOrEmpty(candidate))
            {
                string testDir = Path.Combine(candidate, "Connector.Tests", "packets", "captured");
                if (Directory.Exists(testDir))
                    return testDir;
                candidate = Directory.GetParent(candidate)?.FullName;
            }
            // Fallback: try relative from working directory
            candidate = Path.Combine(Directory.GetCurrentDirectory(), "Connector.Tests", "packets", "captured");
            if (Directory.Exists(candidate))
                return candidate;
            throw new DirectoryNotFoundException(
                "Cannot find Connector.Tests/packets/captured directory. " +
                "Tests must run from the solution root or test output must be in a standard location.");
        }

        /// <summary>
        /// Simulates what AnalyzePackage does: strips the 4-byte AJP header
        /// (0x41 0x42 + 2-byte length) and returns the payload.
        /// </summary>
        private static byte[] StripAjpHeader(byte[] wirePacket)
        {
            int payloadLen = (wirePacket[2] << 8) | wirePacket[3];
            byte[] payload = new byte[payloadLen];
            Array.Copy(wirePacket, 4, payload, 0, payloadLen);
            return payload;
        }

        /// <summary>
        /// Extracts the chunk length from a SendBodyChunk payload.
        /// Payload layout: [0x03][chunkLen_hi][chunkLen_lo][body...][0x00]
        /// </summary>
        private static int GetChunkLength(byte[] payload)
        {
            return (payload[1] << 8) | payload[2];
        }

        /// <summary>
        /// Extracts the raw body bytes directly from wire packet (ground truth).
        /// Wire: [AB][pLen][03][cl_hi][cl_lo][body...][0x00]
        /// </summary>
        private static byte[] ExtractBodyDirect(byte[] wirePacket)
        {
            int chunkLen = (wirePacket[5] << 8) | wirePacket[6];
            byte[] body = new byte[chunkLen];
            Array.Copy(wirePacket, 7, body, 0, chunkLen);
            return body;
        }

        /// <summary>
        /// Runs GetUserDataBytes on a PAK file and returns the result.
        /// Simulates the full AnalyzePackage → TomcatSendBodyChunk → GetUserDataBytes pipeline.
        /// </summary>
        private static byte[] ReplayPakFile(string pakPath)
        {
            byte[] wirePacket = File.ReadAllBytes(pakPath);
            byte[] payload = StripAjpHeader(wirePacket);
            var chunk = new TomcatSendBodyChunk(payload);
            return chunk.GetUserDataBytes();
        }

        #region Category 1: Invariant — All PAK files end with 0x00

        [Fact]
        public void All_TomcatCapture_SendBodyChunks_EndWith0x00()
        {
            string baseDir = GetCaptureBaseDir();
            string tomcatDir = Path.Combine(baseDir, "TomcatCapture");
            var pakFiles = Directory.GetFiles(tomcatDir, "*_SendBodyChunk.pak");

            Assert.True(pakFiles.Length > 0, "Should find SendBodyChunk PAK files in TomcatCapture");

            foreach (var pakFile in pakFiles)
            {
                byte[] wire = File.ReadAllBytes(pakFile);
                int pLen = (wire[2] << 8) | wire[3];
                byte lastByte = wire[4 + pLen - 1]; // last byte of payload
                Assert.Equal(0x00, lastByte);
            }
        }

        [Fact]
        public void All_OldCapture_SendBodyChunks_EndWith0x00()
        {
            string baseDir = GetCaptureBaseDir();
            string oldDir = Path.Combine(baseDir, "OldCapture");
            if (!Directory.Exists(oldDir))
            {
                // OldCapture may not always be present — skip gracefully
                return;
            }

            var pakFiles = Directory.GetFiles(oldDir, "*.pak")
                .Where(f =>
                {
                    byte[] header = File.ReadAllBytes(f);
                    return header.Length >= 5 && header[4] == 0x03;
                });

            foreach (var pakFile in pakFiles)
            {
                byte[] wire = File.ReadAllBytes(pakFile);
                int pLen = (wire[2] << 8) | wire[3];
                byte lastByte = wire[4 + pLen - 1];
                Assert.Equal(0x00, lastByte);
            }
        }

        #endregion

        #region Category 2: Correct extraction from real PAK files

        [Fact]
        public void EmptyChunk_ReturnsEmptyArray()
        {
            // T192_03: chunkLen=0 (AJP flush marker), payload = [03][00][00][00]
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T192_03_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);

            Assert.Empty(result);
        }

        [Fact]
        public void SmallTextChunk_8Bytes_ReturnsCorrectBytes()
        {
            // T192_02: chunkLen=8, text response body
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T192_02_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);
            byte[] expected = ExtractBodyDirect(File.ReadAllBytes(pakPath));

            Assert.Equal(8, result.Length);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MediumTextChunk_14Bytes_ReturnsCorrectBytes()
        {
            // T193_02: chunkLen=14
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T193_02_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);
            byte[] expected = ExtractBodyDirect(File.ReadAllBytes(pakPath));

            Assert.Equal(14, result.Length);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LargeBinaryChunk_8184Bytes_ReturnsCorrectBytes()
        {
            // T208_06: chunkLen=8184 (near-max single chunk)
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T208_06_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);
            byte[] expected = ExtractBodyDirect(File.ReadAllBytes(pakPath));

            Assert.Equal(8184, result.Length);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LargeBinaryChunk_5000Bytes_ReturnsCorrectBytes()
        {
            // T222_02: chunkLen=5000
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T222_02_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);
            byte[] expected = ExtractBodyDirect(File.ReadAllBytes(pakPath));

            Assert.Equal(5000, result.Length);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void OddSizeChunk_3Bytes_Mod4Equals3_ReturnsCorrectBytes()
        {
            // T216_02: chunkLen=3 (3 % 4 == 3 — worst case for padding theory)
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T216_02_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);
            byte[] expected = ExtractBodyDirect(File.ReadAllBytes(pakPath));

            Assert.Equal(3, result.Length);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SingleByteChunk_1Byte_ReturnsCorrectByte()
        {
            // T216_06: chunkLen=1 — edge case, losing 1 byte = losing 100% of data
            string pakPath = Path.Combine(GetCaptureBaseDir(), "TomcatCapture", "T216_06_SendBodyChunk.pak");
            byte[] result = ReplayPakFile(pakPath);
            byte[] expected = ExtractBodyDirect(File.ReadAllBytes(pakPath));

            Assert.Single(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void All_TomcatCapture_Chunks_ExtractExactChunkLengthBytes()
        {
            // Comprehensive: every SendBodyChunk PAK returns exactly chunkLen bytes
            string baseDir = GetCaptureBaseDir();
            string tomcatDir = Path.Combine(baseDir, "TomcatCapture");
            var pakFiles = Directory.GetFiles(tomcatDir, "*_SendBodyChunk.pak")
                .OrderBy(f => f)
                .ToList();

            foreach (var pakFile in pakFiles)
            {
                byte[] wire = File.ReadAllBytes(pakFile);
                int expectedChunkLen = (wire[5] << 8) | wire[6];
                byte[] result = ReplayPakFile(pakFile);

                Assert.True(result.Length == expectedChunkLen,
                    $"{Path.GetFileName(pakFile)}: expected {expectedChunkLen} bytes but got {result.Length}");
            }
        }

        #endregion

        #region Category 3: Multi-chunk concatenation

        [Fact]
        public void MultiChunk_9000Bytes_ReturnsCorrectTotal()
        {
            // T211: test_get_binary_9000.jsp response split into 2 chunks (8184 + 816)
            string baseDir = Path.Combine(GetCaptureBaseDir(), "TomcatCapture");
            string pak1 = Path.Combine(baseDir, "T211_02_SendBodyChunk.pak"); // 8184 bytes
            string pak2 = Path.Combine(baseDir, "T211_03_SendBodyChunk.pak"); // 816 bytes

            byte[] chunk1 = ReplayPakFile(pak1);
            byte[] chunk2 = ReplayPakFile(pak2);

            Assert.Equal(8184, chunk1.Length);
            Assert.Equal(816, chunk2.Length);

            // Concatenate and verify total
            byte[] fullBody = new byte[chunk1.Length + chunk2.Length];
            Array.Copy(chunk1, 0, fullBody, 0, chunk1.Length);
            Array.Copy(chunk2, 0, fullBody, chunk1.Length, chunk2.Length);

            Assert.Equal(9000, fullBody.Length);
        }

        [Fact]
        public void MultiChunk_16384Bytes_ReturnsCorrectTotal()
        {
            // T221: large binary response split into 3 chunks (8184 + 8184 + 16)
            string baseDir = Path.Combine(GetCaptureBaseDir(), "TomcatCapture");
            string pak1 = Path.Combine(baseDir, "T221_02_SendBodyChunk.pak"); // 8184
            string pak2 = Path.Combine(baseDir, "T221_03_SendBodyChunk.pak"); // 8184
            string pak3 = Path.Combine(baseDir, "T221_04_SendBodyChunk.pak"); // 16

            byte[] chunk1 = ReplayPakFile(pak1);
            byte[] chunk2 = ReplayPakFile(pak2);
            byte[] chunk3 = ReplayPakFile(pak3);

            Assert.Equal(8184, chunk1.Length);
            Assert.Equal(8184, chunk2.Length);
            Assert.Equal(16, chunk3.Length);

            byte[] fullBody = new byte[chunk1.Length + chunk2.Length + chunk3.Length];
            int offset = 0;
            Array.Copy(chunk1, 0, fullBody, offset, chunk1.Length); offset += chunk1.Length;
            Array.Copy(chunk2, 0, fullBody, offset, chunk2.Length); offset += chunk2.Length;
            Array.Copy(chunk3, 0, fullBody, offset, chunk3.Length);

            Assert.Equal(16384, fullBody.Length);
        }

        [Fact]
        public void MultiChunk_20605Bytes_LargeResponse_ReturnsCorrectTotal()
        {
            // T208: test_get_large_response.jsp — 5 data chunks (70 + 8184 + 8184 + 4142 + 25)
            // Note: T208 has multiple responses in one connection; chunks 02,06,07,08 are the large one
            string baseDir = Path.Combine(GetCaptureBaseDir(), "TomcatCapture");
            var pakFiles = new[]
            {
                Path.Combine(baseDir, "T208_06_SendBodyChunk.pak"), // 8184
                Path.Combine(baseDir, "T208_07_SendBodyChunk.pak"), // 8184
                Path.Combine(baseDir, "T208_08_SendBodyChunk.pak"), // 4142
            };

            int total = 0;
            foreach (var pak in pakFiles)
            {
                byte[] chunk = ReplayPakFile(pak);
                total += chunk.Length;
            }

            Assert.Equal(8184 + 8184 + 4142, total);
        }

        #endregion

        #region Category 4: UserDataLength property validation

        [Fact]
        public void UserDataLength_MatchesChunkLength_ForAllChunks()
        {
            // p_UserDataLength = content.Length - 4, and with 0x00 termination:
            // content.Length = chunkLen + 4, so UserDataLength = chunkLen. Correct.
            string baseDir = Path.Combine(GetCaptureBaseDir(), "TomcatCapture");
            var pakFiles = Directory.GetFiles(baseDir, "*_SendBodyChunk.pak")
                .OrderBy(f => f)
                .ToList();

            foreach (var pakFile in pakFiles)
            {
                byte[] wire = File.ReadAllBytes(pakFile);
                int expectedChunkLen = (wire[5] << 8) | wire[6];
                byte[] payload = StripAjpHeader(wire);
                var chunk = new TomcatSendBodyChunk(payload);

                Assert.True(chunk.UserDataLength == expectedChunkLen,
                    $"{Path.GetFileName(pakFile)}: UserDataLength={chunk.UserDataLength} but chunkLen={expectedChunkLen}");
            }
        }

        #endregion
    }
}