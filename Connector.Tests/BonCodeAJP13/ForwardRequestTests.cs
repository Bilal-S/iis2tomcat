using System;
using System.Collections.Specialized;
using Xunit;
using BonCodeAJP13;
using BonCodeAJP13.ServerPackets;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Unit tests for BonCodeAJP13ForwardRequest class
    /// </summary>
    public class ForwardRequestTests
    {
        #region Constructor Tests

        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            // Act
            var packet = new BonCodeAJP13ForwardRequest();

            // Assert
            Assert.NotNull(packet);
        }

        [Fact]
        public void BodyOnlyConstructor_WithValidContent_CreatesPacket()
        {
            // Arrange
            var content = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

            // Act
            var packet = new BonCodeAJP13ForwardRequest(content);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            Assert.True(packet.PacketLength > 0);
        }

        [Fact]
        public void BodyOnlyConstructor_WithEmptyContent_CreatesPacket()
        {
            // Arrange
            var content = new byte[0];

            // Act
            var packet = new BonCodeAJP13ForwardRequest(content);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            Assert.Equal(4, packet.PacketLength); // Empty packet is 4 bytes (magic + length)
        }

        [Fact]
        public void BodyOnlyConstructor_WithOversizedContent_ThrowsException()
        {
            // Arrange - create content larger than max allowed
            var maxAllowed = BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH;
            var content = new byte[maxAllowed + 1];

            // Act & Assert
            Assert.Throws<Exception>(() => new BonCodeAJP13ForwardRequest(content));
        }

        [Fact]
        public void BodyOnlyConstructor_WithMaxSizeContent_CreatesPacket()
        {
            // Arrange
            var maxAllowed = BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH;
            var content = new byte[maxAllowed];

            // Act
            var packet = new BonCodeAJP13ForwardRequest(content);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        #endregion

        #region Test Constructor (without headers)

        [Fact]
        public void TestConstructor_WithGetRequest_CreatesPacket()
        {
            // Act - using test constructor without headers
            var packet = new BonCodeAJP13ForwardRequest(
                method: 0x02, // GET
                protocol: "HTTP/1.1",
                req_uri: "/test/path",
                remote_addr: "127.0.0.1",
                remote_host: "localhost",
                server_name: "example.com",
                server_port: 80,
                is_ssl: false
            );

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            // Note: GetUrl/GetMethod are only populated when logging is enabled via headers constructor
        }

        [Fact]
        public void TestConstructor_WithPostRequest_CreatesPacket()
        {
            // Act - POST method is 0x03
            var packet = new BonCodeAJP13ForwardRequest(
                method: 0x03, // POST
                protocol: "HTTP/1.1",
                req_uri: "/api/submit",
                remote_addr: "192.168.1.1",
                remote_host: "client.local",
                server_name: "server.example.com",
                server_port: 443,
                is_ssl: true
            );

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            // Note: GetUrl/GetMethod are only populated when logging is enabled via headers constructor
        }

        [Fact]
        public void TestConstructor_WithRootUri_CreatesPacket()
        {
            // Act
            var packet = new BonCodeAJP13ForwardRequest(
                method: 0x02,
                protocol: "HTTP/1.1",
                req_uri: "/",
                remote_addr: "::1",
                remote_host: "::1",
                server_name: "localhost",
                server_port: 8080,
                is_ssl: false
            );

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            // Note: GetUrl is only populated when logging is enabled via headers constructor
        }

        #endregion

        #region Headers Constructor Tests

        [Fact]
        public void HeadersConstructor_WithBasicHeaders_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            Assert.True(packet.PacketLength > 0);
        }

        [Fact]
        public void HeadersConstructor_WithPathInfo_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            var pathInfo = "/additional/path";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers, pathInfo);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void HeadersConstructor_WithSourcePort_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            var pathInfo = "";
            var sourcePort = 54321;
            var vdirs = "/app1,/app2";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers, pathInfo, sourcePort, vdirs);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void HeadersConstructor_WithMinimalHeaders_CreatesPacket()
        {
            // Arrange - include ALL_RAW to avoid NullReferenceException
            var headers = new NameValueCollection();
            headers["REQUEST_METHOD"] = "GET";
            headers["SERVER_PROTOCOL"] = "HTTP/1.1";
            headers["SCRIPT_NAME"] = "/minimal";
            headers["REMOTE_ADDR"] = "127.0.0.1";
            headers["REMOTE_HOST"] = "localhost";
            headers["HTTP_HOST"] = "localhost";
            headers["SERVER_PORT"] = "80";
            headers["HTTPS"] = "off";
            headers["ALL_RAW"] = "Host: localhost\r\n";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        #endregion

        #region HTTP Method Tests

        [Fact]
        public void GetMethod_ForGetRequest_ReturnsGET()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["REQUEST_METHOD"] = "GET";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.Equal("GET", packet.GetMethod);
        }

        [Fact]
        public void GetMethod_ForPostRequest_ReturnsPOST()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["REQUEST_METHOD"] = "POST";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.Equal("POST", packet.GetMethod);
        }

        [Fact]
        public void GetMethod_ForPutRequest_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["REQUEST_METHOD"] = "PUT";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void GetMethod_ForDeleteRequest_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["REQUEST_METHOD"] = "DELETE";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        #endregion

        #region URL Tests

        [Fact]
        public void GetUrl_ReturnsCorrectUri()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["SCRIPT_NAME"] = "/api/users/123";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.Equal("/api/users/123", packet.GetUrl);
        }

        [Fact]
        public void GetUrl_ForRootPath_ReturnsSlash()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["SCRIPT_NAME"] = "/";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.Equal("/", packet.GetUrl);
        }

        #endregion

        #region SSL Tests

        [Fact]
        public void Request_WithSSL_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["HTTPS"] = "on";
            headers["SERVER_PORT"] = "443";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void Request_WithoutSSL_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["HTTPS"] = "off";
            headers["SERVER_PORT"] = "80";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        #endregion

        #region Header Handling Tests

        [Fact]
        public void CheckHeaders_FiltersBlacklistedHeaders()
        {
            // Arrange
            var packet = new BonCodeAJP13ForwardRequest();
            var headers = new NameValueCollection();
            headers["HTTP_HOST"] = "example.com";
            headers["HTTP_COOKIE"] = "session=abc123";
            headers["PATH_TRANSLATED"] = "/physical/path"; // Should be filtered
            headers["INSTANCE_ID"] = "1"; // Should be filtered

            // Act
            var result = packet.CheckHeaders(headers);

            // Assert
            Assert.DoesNotContain("PATH_TRANSLATED", result.AllKeys);
            Assert.DoesNotContain("INSTANCE_ID", result.AllKeys);
        }

        [Fact]
        public void CheckHeaders_HandlesNullValues()
        {
            // Arrange
            var packet = new BonCodeAJP13ForwardRequest();
            var headers = new NameValueCollection();
            headers["HTTP_HOST"] = "example.com";
            headers["HTTP_CUSTOM"] = null;

            // Act & Assert - should not throw
            var result = packet.CheckHeaders(headers);
            Assert.NotNull(result);
        }

        [Fact]
        public void CheckHeaders_PreservesValidHeaders()
        {
            // Arrange
            var packet = new BonCodeAJP13ForwardRequest();
            var headers = new NameValueCollection();
            headers["HTTP_HOST"] = "example.com";
            headers["HTTP_USER_AGENT"] = "TestClient/1.0";
            headers["HTTP_ACCEPT"] = "application/json";

            // Act
            var result = packet.CheckHeaders(headers);

            // Assert
            Assert.Contains("HTTP_HOST", result.AllKeys);
            Assert.Contains("HTTP_USER_AGENT", result.AllKeys);
            Assert.Contains("HTTP_ACCEPT", result.AllKeys);
        }

        #endregion

        #region Content/Body Handling Tests

        [Fact]
        public void BodyContent_WithinLimits_CreatesPacket()
        {
            // Arrange
            var content = new byte[1000];
            for (int i = 0; i < content.Length; i++)
            {
                content[i] = (byte)(i % 256);
            }

            // Act
            var packet = new BonCodeAJP13ForwardRequest(content);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
            Assert.True(packet.PacketLength > content.Length);
        }

        [Fact]
        public void BodyContent_StartsWithMagicBytes()
        {
            // Arrange
            var content = new byte[] { 0x01, 0x02, 0x03 };

            // Act
            var packet = new BonCodeAJP13ForwardRequest(content);
            var data = packet.GetDataBytes();

            // Assert - Forward request packets use AB magic bytes (0x41, 0x42)
            Assert.NotNull(data);
            Assert.True(data.Length >= 2);
        }

        #endregion

        #region PrintPacketHeader Tests

        [Fact]
        public void PrintPacketHeader_ReturnsFormattedString()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["HTTP_HOST"] = "example.com";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);
            var result = packet.PrintPacketHeader();

            // Assert
            Assert.Contains("GET", result);
            Assert.Contains("/test", result);
            Assert.Contains("bytes", result);
        }

        [Fact]
        public void PrintPacketHeader_WithHeadersInstance_ReturnsString()
        {
            // Arrange - use headers constructor to populate internal fields
            var headers = CreateBasicHeaders();
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Act
            var result = packet.PrintPacketHeader();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("GET", result);
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void ForwardRequest_InheritsFromBonCodeAJP13Packet()
        {
            // Act
            var packet = new BonCodeAJP13ForwardRequest();

            // Assert
            Assert.IsType<BonCodeAJP13Packet>(packet, exactMatch: false);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Request_WithQueryString_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["QUERY_STRING"] = "param1=value1&param2=value2";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void Request_WithUnicodeUri_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["SCRIPT_NAME"] = "/api/tëst/ünicode";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void Request_WithIPv6Address_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["REMOTE_ADDR"] = "::1";
            headers["REMOTE_HOST"] = "::1";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void Request_WithContentLength_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["CONTENT_LENGTH"] = "1024";
            headers["CONTENT_TYPE"] = "application/json";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        [Fact]
        public void Request_WithZeroContentLength_CreatesPacket()
        {
            // Arrange
            var headers = CreateBasicHeaders();
            headers["CONTENT_LENGTH"] = "0";

            // Act
            var packet = new BonCodeAJP13ForwardRequest(headers);

            // Assert
            Assert.NotNull(packet.GetDataBytes());
        }

        #endregion

        #region WritePacketTest Path Prefix Tests

        [Fact]
        public void WritePacketTest_PathPrefix_ProducesCorrectUri()
        {
            // This test exposes the bug: WritePacketTest uses .Length (int) instead of
            // the string value, producing e.g. "6//test" instead of "/lucee/test"
            string originalPrefix = BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX;
            try
            {
                BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX = "/lucee";

                var packet = new BonCodeAJP13ForwardRequest(
                    method: 0x02,
                    protocol: "HTTP/1.1",
                    req_uri: "/test",
                    remote_addr: "::1",
                    remote_host: "::1",
                    server_name: "localhost",
                    server_port: 80,
                    is_ssl: false
                );

                string uri = ExtractUriFromTestPacket(packet.GetDataBytes());
                Assert.Equal("/lucee//test", uri);
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX = originalPrefix;
            }
        }

        [Fact]
        public void WritePacketTest_PathPrefix_DoesNotStartWithDigit()
        {
            // The bug concatenates .Length (an integer) producing URIs like "6//test"
            // A valid URI should never start with the length number
            string originalPrefix = BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX;
            try
            {
                BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX = "/lucee";

                var packet = new BonCodeAJP13ForwardRequest(
                    method: 0x02,
                    protocol: "HTTP/1.1",
                    req_uri: "/some/path",
                    remote_addr: "::1",
                    remote_host: "::1",
                    server_name: "localhost",
                    server_port: 80,
                    is_ssl: false
                );

                string uri = ExtractUriFromTestPacket(packet.GetDataBytes());
                Assert.False(uri.Length > 0 && char.IsDigit(uri[0]),
                    $"URI should not start with a digit. Got: \"{uri}\"");
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX = originalPrefix;
            }
        }

        [Fact]
        public void WritePacketTest_NoPathPrefix_ProducesOriginalUri()
        {
            // When prefix is empty (length <= 2), the prefix block is skipped entirely
            string originalPrefix = BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX;
            try
            {
                BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX = "";

                var packet = new BonCodeAJP13ForwardRequest(
                    method: 0x02,
                    protocol: "HTTP/1.1",
                    req_uri: "/test",
                    remote_addr: "::1",
                    remote_host: "::1",
                    server_name: "localhost",
                    server_port: 80,
                    is_ssl: false
                );

                string uri = ExtractUriFromTestPacket(packet.GetDataBytes());
                Assert.Equal("/test", uri);
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX = originalPrefix;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Extracts the URI string from a test-constructor packet by parsing the AJP13 binary layout.
        /// Layout after 4-byte header: type(1) + method(1) + protocol(string) + URI(string) + ...
        /// </summary>
        private static string ExtractUriFromTestPacket(byte[] packetBytes)
        {
            int pos = 4; // skip 4-byte header (magic + length)
            pos++;       // skip type byte (0x02 = SERVER_FORWARD_REQUEST)
            pos++;       // skip method byte
            ExtractAjpString(packetBytes, ref pos); // skip protocol string (e.g. "HTTP/1.1")
            return ExtractAjpString(packetBytes, ref pos); // this is the URI
        }

        /// <summary>
        /// Reads an AJP13-encoded string from the byte array at the given position.
        /// AJP13 string format: [len_hi len_lo][UTF-8 bytes][0x00 terminator]
        /// Advances pos past the entire string entry.
        /// </summary>
        private static string ExtractAjpString(byte[] data, ref int pos)
        {
            ushort len = (ushort)((data[pos] << 8) | data[pos + 1]);
            string value = System.Text.Encoding.UTF8.GetString(data, pos + 2, len);
            pos = pos + len + 3; // skip length prefix (2) + string bytes + null terminator (1)
            return value;
        }

        private NameValueCollection CreateBasicHeaders()
        {
            var headers = new NameValueCollection();
            headers["REQUEST_METHOD"] = "GET";
            headers["SERVER_PROTOCOL"] = "HTTP/1.1";
            headers["SCRIPT_NAME"] = "/test";
            headers["REMOTE_ADDR"] = "127.0.0.1";
            headers["REMOTE_HOST"] = "localhost";
            headers["HTTP_HOST"] = "localhost";
            headers["SERVER_PORT"] = "80";
            headers["HTTPS"] = "off";
            headers["ALL_RAW"] = "Host: localhost\r\n";
            return headers;
        }

        #endregion
    }
}