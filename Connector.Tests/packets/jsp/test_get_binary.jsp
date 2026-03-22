<%--
  Test: GET request returning a 1x1 transparent PNG (67 bytes)
  Expected body size: 67 bytes
--%><%@ page contentType="image/png" buffer="none" %><%
    String pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
    byte[] pngBytes = java.util.Base64.getDecoder().decode(pngBase64);
    response.getOutputStream().write(pngBytes);
    response.getOutputStream().flush();
%>