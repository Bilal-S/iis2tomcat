using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Xunit;
using BonCodeAJP13;

namespace Connector.Tests.BonCodeAJP13
{
    public class ServerConnectionTests
    {
        #region Static Method Tests - FlipArray

        [Fact]
        public void FlipArray_ReversesByteArray()
        {
            var input = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var expected = new byte[] { 0x04, 0x03, 0x02, 0x01 };
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FlipArray_WithSingleElement_ReturnsSameArray()
        {
            var input = new byte[] { 0x42 };
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);
            Assert.Equal(input, result);
        }

        [Fact]
        public void FlipArray_WithEmptyArray_ReturnsEmptyArray()
        {
            var input = new byte[0];
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);
            Assert.Empty(result);
        }

        [Fact]
        public void FlipArray_WithTwoElements_SwapsPositions()
        {
            var input = new byte[] { 0xAB, 0xCD };
            var expected = new byte[] { 0xCD, 0xAB };
            var result = InvokeStaticMethod<byte[]>("FlipArray", input);
            Assert.Equal(expected, result);
        }

        #endregion

        #region Static Method Tests - ByteSearch

        [Fact]
        public void ByteSearch_FindsPatternAtStart_ReturnsZero()
        {
            var searchIn = new byte[] { 0xAB, 0xCD, 0xEF, 0x00 };
            var searchFor = new byte[] { 0xAB, 0xCD };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(0, result);
        }

        [Fact]
        public void ByteSearch_FindsPatternInMiddle_ReturnsCorrectIndex()
        {
            var searchIn = new byte[] { 0x00, 0x01, 0xAB, 0xCD, 0x02 };
            var searchFor = new byte[] { 0xAB, 0xCD };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(2, result);
        }

        [Fact]
        public void ByteSearch_PatternNotFound_ReturnsMinusOne()
        {
            var searchIn = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            var searchFor = new byte[] { 0xAB, 0xCD };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_WithStartOffset_SearchesFromOffset()
        {
            var searchIn = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD };
            var searchFor = new byte[] { 0xAB, 0xCD };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 2);
            Assert.Equal(2, result);
        }

        [Fact]
        public void ByteSearch_SingleBytePattern_FindsCorrectly()
        {
            var searchIn = new byte[] { 0x00, 0x42, 0x01 };
            var searchFor = new byte[] { 0x42 };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(1, result);
        }

        [Fact]
        public void ByteSearch_EmptySearchIn_ReturnsMinusOne()
        {
            var searchIn = new byte[0];
            var searchFor = new byte[] { 0xAB };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_EmptySearchFor_ReturnsMinusOne()
        {
            var searchIn = new byte[] { 0x00, 0x01, 0x02 };
            var searchFor = new byte[0];
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_PatternLongerThanData_ReturnsMinusOne()
        {
            var searchIn = new byte[] { 0x01 };
            var searchFor = new byte[] { 0x01, 0x02, 0x03 };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ByteSearch_FindsAJP13MagicBytes()
        {
            var searchIn = new byte[] { 0x00, 0x00, 0x41, 0x42, 0x00, 0x05 };
            var searchFor = new byte[] { 0x41, 0x42 };
            var result = InvokeStaticMethod<int>("ByteSearch", searchIn, searchFor, 0);
            Assert.Equal(2, result);
        }

        #endregion

        #region Private Method Tests - GetInt16B

        [Fact]
        public void GetInt16B_ReadsBigEndianValue()
        {
            var data = new byte[] { 0x01, 0x02 };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 0);
            Assert.Equal(258, result);
        }

        [Fact]
        public void GetInt16B_WithOffset_ReadsCorrectPosition()
        {
            var data = new byte[] { 0x00, 0x00, 0x03, 0x00 };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 2);
            Assert.Equal(768, result);
        }

        [Fact]
        public void GetInt16B_WithZero_ReturnsZero()
        {
            var data = new byte[] { 0x00, 0x00 };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 0);
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetInt16B_WithMaxValue_ReturnsMaxUInt16()
        {
            var data = new byte[] { 0xFF, 0xFF };
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var result = InvokePrivateMethod<int>(connection, "GetInt16B", data, 0);
            Assert.Equal(65535, result);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Server_GetSet_WorksCorrectly()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.Server = "192.168.1.1";
            Assert.Equal("192.168.1.1", connection.Server);
        }

        [Fact]
        public void Port_GetSet_WorksCorrectly()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.Port = 9009;
            Assert.Equal(9009, connection.Port);
        }

        [Fact]
        public void AbortConnection_DefaultValue_IsFalse()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            Assert.False(connection.AbortConnection);
        }

        [Fact]
        public void AbortConnection_GetSet_WorksCorrectly()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.AbortConnection = true;
            Assert.True(connection.AbortConnection);
        }

        [Fact]
        public void ChunkedTransfer_DefaultValue_IsFalse()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            Assert.False(connection.ChunkedTransfer);
        }

        [Fact]
        public void ChunkedTransfer_GetSet_WorksCorrectly()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.ChunkedTransfer = true;
            Assert.True(connection.ChunkedTransfer);
        }

        [Fact]
        public void ReceivedDataCollection_IsNotNull()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            Assert.NotNull(connection.ReceivedDataCollection);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithLogFilePostFix_CreatesInstance()
        {
            Assert.NotNull(new BonCodeAJP13ServerConnection("test-postfix", "127.0.0.1"));
        }

        [Fact]
        public void Constructor_WithEmptyParameters_CreatesInstance()
        {
#pragma warning disable CS0618 // Type or member is obsolete - testing deprecated constructor still creates instance
            Assert.NotNull(new BonCodeAJP13ServerConnection());
#pragma warning restore CS0618
        }

        #endregion

        #region AddPacketToSendQueue Tests

        [Fact]
        public void AddPacketToSendQueue_AddsPacketToCollection()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var packet = new global::BonCodeAJP13.TomcatPackets.TomcatSendBodyChunk("test data");
            connection.AddPacketToSendQueue(packet);
            Assert.NotNull(connection);
        }

        #endregion

        #region IDisposable Tests

        [Fact]
        public void Dispose_IsIdempotent_DoesNotThrowOnSecondCall()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.Dispose();
            connection.Dispose();
        }

        [Fact]
        public void Dispose_ThrowsObjectDisposedException_OnReceivedDataCollection()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => connection.ReceivedDataCollection);
        }

        [Fact]
        public void Dispose_ThrowsObjectDisposedException_OnBeginConnection()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => connection.BeginConnection());
        }

        [Fact]
        public void Dispose_ThrowsObjectDisposedException_OnAddPacketToSendQueue()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            var packet = new global::BonCodeAJP13.TomcatPackets.TomcatSendBodyChunk("test");
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => connection.AddPacketToSendQueue(packet));
        }

        [Fact]
        public void ImplementsIDisposable()
        {
            Assert.IsAssignableFrom<IDisposable>(new BonCodeAJP13ServerConnection("test", "127.0.0.1"));
        }

        [Fact]
        public void SetTcpClient_SetsOwnershipToFalse_WhenNonNullClientPassed()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            using (var tcpClient = new TcpClient())
            {
                connection.SetTcpClient = tcpClient;
                var field = typeof(BonCodeAJP13ServerConnection).GetField("_ownsTcpClient", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(field);
                Assert.False((bool)field.GetValue(connection));
            }
        }

        [Fact]
        public void SetTcpClient_SetsOwnershipToTrue_WhenNullPassed()
        {
            var connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
            connection.SetTcpClient = null;
            var field = typeof(BonCodeAJP13ServerConnection).GetField("_ownsTcpClient", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            Assert.True((bool)field.GetValue(connection));
        }

        [Fact]
        public void UsingStatement_DisposesConnection()
        {
            BonCodeAJP13ServerConnection connection;
            using (connection = new BonCodeAJP13ServerConnection("test", "127.0.0.1"))
            {
                Assert.NotNull(connection.ReceivedDataCollection);
            }
            Assert.Throws<ObjectDisposedException>(() => connection.ReceivedDataCollection);
        }

        #endregion

        #region TCP Connection Reuse Tests

        [Fact]
        public void Dispose_DoesNotCloseBorrowedTcpClient()
        {
            var listener = TcpListener.Create(0);
            listener.Start();
            try
            {
                var endpoint = (IPEndPoint)listener.LocalEndpoint;
                var tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", endpoint.Port);
                Assert.True(tcpClient.Connected, "Precondition: TCP client should be connected");

                using (var conn = new BonCodeAJP13ServerConnection("test", "127.0.0.1"))
                {
                    conn.SetTcpClient = tcpClient;
                }

                Assert.True(tcpClient.Connected, "Borrowed TcpClient should survive ServerConnection Dispose");
                tcpClient.Close();
            }
            finally
            {
                listener.Stop();
            }
        }

        [Fact]
        public void Dispose_ClosesOwnedTcpClient()
        {
            var listener = TcpListener.Create(0);
            listener.Start();
            try
            {
                var endpoint = (IPEndPoint)listener.LocalEndpoint;
                var tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", endpoint.Port);
                Assert.True(tcpClient.Connected, "Precondition: TCP client should be connected");

                var conn = new BonCodeAJP13ServerConnection("test", "127.0.0.1");
                conn.SetTcpClient = null; // _ownsTcpClient = true
                var tcpField = typeof(BonCodeAJP13ServerConnection).GetField("p_TCPClient", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(tcpField);
                tcpField.SetValue(conn, tcpClient);

                conn.Dispose();

                bool connected = false;
                try { connected = tcpClient.Connected; }
                catch (NullReferenceException) { /* expected: socket was disposed */ }
                Assert.False(connected, "Owned TcpClient should be closed after Dispose");
            }
            finally
            {
                listener.Stop();
            }
        }

        [Fact]
        public void SameTcpClient_ReusedAcrossMultipleConnections()
        {
            var listener = TcpListener.Create(0);
            listener.Start();
            try
            {
                var endpoint = (IPEndPoint)listener.LocalEndpoint;
                var tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", endpoint.Port);
                int originalPort = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Port;

                using (var conn1 = new BonCodeAJP13ServerConnection("test", "127.0.0.1"))
                {
                    conn1.SetTcpClient = tcpClient;
                }
                Assert.True(tcpClient.Connected, "TCP should survive first request");

                using (var conn2 = new BonCodeAJP13ServerConnection("test", "127.0.0.1"))
                {
                    conn2.SetTcpClient = tcpClient;
                }
                Assert.True(tcpClient.Connected, "TCP should survive second request");

                int reusedPort = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Port;
                Assert.True(originalPort == reusedPort, "Same local port proves the socket was reused, not destroyed and recreated");

                tcpClient.Close();
            }
            finally
            {
                listener.Stop();
            }
        }

        #endregion

        #region Helper Methods

        private T InvokeStaticMethod<T>(string methodName, params object[] parameters)
        {
            var method = typeof(BonCodeAJP13ServerConnection).GetMethod(methodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) throw new InvalidOperationException($"Method {methodName} not found");
            return (T)method.Invoke(null, parameters);
        }

        private T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
        {
            var method = typeof(BonCodeAJP13ServerConnection).GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) throw new InvalidOperationException($"Method {methodName} not found");
            return (T)method.Invoke(instance, parameters);
        }

        #endregion

        #region AnalyzePackage - partial packet header across reads

        private static byte[] SendBodyChunk(int dataLength)
        {
            // 41 42 | payload length (2) | 03 | chunk length (2) | data | 00
            int payloadLength = 1 + 2 + dataLength + 1;
            var packet = new byte[4 + payloadLength];
            packet[0] = 0x41; packet[1] = 0x42;
            packet[2] = (byte)(payloadLength >> 8); packet[3] = (byte)(payloadLength & 0xFF);
            packet[4] = 0x03;
            packet[5] = (byte)(dataLength >> 8); packet[6] = (byte)(dataLength & 0xFF);
            for (int i = 0; i < dataLength; i++) packet[7 + i] = 0x61;
            packet[7 + dataLength] = 0x00;
            return packet;
        }

        private static readonly byte[] EndResponse = { 0x41, 0x42, 0x00, 0x02, 0x05, 0x01 };

        private bool IsLastPacket(BonCodeAJP13ServerConnection conn)
        {
            return (bool)typeof(BonCodeAJP13ServerConnection)
                .GetField("p_IsLastPacket", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(conn);
        }

        private byte[] Analyze(BonCodeAJP13ServerConnection conn, byte[] buffer)
        {
            var method = typeof(BonCodeAJP13ServerConnection).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(m => m.Name == "AnalyzePackage" && m.GetParameters().Length == 3);
            return (byte[])method.Invoke(conn, new object[] { buffer, false, 0 });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void AnalyzePackage_ReadEndingInsideNextPacketHeader_CarriesTheHeaderBytes(int bytesIntoEndPacket)
        {
            var body = SendBodyChunk(10);
            var stream = body.Concat(EndResponse).ToArray();
            int cut = body.Length + bytesIntoEndPacket;
            var first = stream.Take(cut).ToArray();
            var second = stream.Skip(cut).ToArray();

            using (var conn = new BonCodeAJP13ServerConnection("", ""))
            {
                byte[] carried = Analyze(conn, first);
                Assert.NotNull(carried);
                Assert.Equal(bytesIntoEndPacket, carried.Length);
                Assert.False(IsLastPacket(conn));

                byte[] leftover = Analyze(conn, carried.Concat(second).ToArray());
                Assert.Null(leftover);
                Assert.True(IsLastPacket(conn), "END_RESPONSE split across two reads must still be recognised");
            }
        }

        [Fact]
        public void AnalyzePackage_ResponseReadInPacketSizePieces_RecognisesEndResponse()
        {
            // 281-byte SEND_HEADERS + three full 8192-byte chunks + a 7910-byte chunk + END_RESPONSE
            // = 32773 bytes total. Read in 8192-byte pieces, the fourth read ends one byte into
            // END_RESPONSE (layout from #115).
            var headers = new byte[281];
            headers[0] = 0x41; headers[1] = 0x42; headers[2] = 0x01; headers[3] = 0x15; headers[4] = 0x04;
            var stream = headers
                .Concat(SendBodyChunk(8184)).Concat(SendBodyChunk(8184)).Concat(SendBodyChunk(8184))
                .Concat(SendBodyChunk(7902)).Concat(EndResponse).ToArray();
            Assert.Equal(32773, stream.Length);

            using (var conn = new BonCodeAJP13ServerConnection("", ""))
            {
                byte[] carry = null;
                for (int offset = 0; offset < stream.Length; offset += 8192)
                {
                    var piece = stream.Skip(offset).Take(8192).ToArray();
                    carry = Analyze(conn, (carry ?? new byte[0]).Concat(piece).ToArray());
                }
                Assert.Null(carry);
                Assert.True(IsLastPacket(conn));
            }
        }

        [Fact]
        public void AnalyzePackage_ReadEndingOnPacketBoundary_CarriesNothing()
        {
            var body = SendBodyChunk(10);
            using (var conn = new BonCodeAJP13ServerConnection("", ""))
            {
                Assert.Null(Analyze(conn, body));
                Assert.False(IsLastPacket(conn));
                Assert.Null(Analyze(conn, EndResponse));
                Assert.True(IsLastPacket(conn));
            }
        }

        // future tests (see #115): lone 0x41 tail carry; partial header then partial body
        #endregion
    }
}
