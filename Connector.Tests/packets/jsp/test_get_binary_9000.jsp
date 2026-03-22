<%@ page contentType="application/octet-stream" buffer="none" %><%
    // 9000 bytes, sequential pattern 0x01..0xFE repeating
    // 9000 % 4 = 0 → AJP spec padding = 0 for both chunks
    // Max AJP chunk payload = 8186 bytes, so:
    //   Chunk 1: 8186 bytes body → padding = (4 - (8186 % 4)) % 4 = 2 → wire payload = 8191
    //   Chunk 2:  814 bytes body → padding = (4 - (814 % 4)) % 4 = 2 → wire payload = 819
    // With padding=2 for both: GetUserDataBytes copies Length-4, which includes 2 padding bytes
    // Bug exposure: each chunk gains 2 garbage trailing bytes → 8188+816 = 9004 bytes output
    byte[] data = new byte[9000];
    for (int i = 0; i < 9000; i++) {
        data[i] = (byte)((i % 254) + 1);
    }
    response.getOutputStream().write(data);
    response.getOutputStream().flush();
%>