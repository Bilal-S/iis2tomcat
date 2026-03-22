<%@ page contentType="application/octet-stream" buffer="none" %><%
    // 5000 bytes, sequential pattern 0x01..0xFE repeating
    // 5000 % 4 = 0 → AJP spec padding = 0 → wire payload = 5003 bytes (type+length+body)
    // Single chunk (under 8186 max payload)
    // With padding=0: GetUserDataBytes copies Length-4 = 5003-4 = 4999 bytes (DROPS 1 BYTE)
    // Bug exposure: last byte (0x01 at index 4999) is lost
    byte[] data = new byte[5000];
    for (int i = 0; i < 5000; i++) {
        data[i] = (byte)((i % 254) + 1);
    }
    response.getOutputStream().write(data);
    response.getOutputStream().flush();
%>