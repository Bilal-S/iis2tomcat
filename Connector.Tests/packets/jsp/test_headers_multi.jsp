<%--
  Test: Returns multiple response headers including Set-Cookie headers
  Expected: 2 Set-Cookie headers, custom X-Custom-Header, Content-Type
  Tests: Header parsing in TomcatSendHeaders, specifically the |, delimiter
  Also tests multiple Set-Cookie handling (Bug area in GetHeaders)
--%><%@ page contentType="text/plain; charset=utf-8" buffer="none" %><%
    response.addHeader("Set-Cookie", "session1=abc123; Path=/; HttpOnly");
    response.addHeader("Set-Cookie", "session2=xyz789; Path=/; HttpOnly");
    response.setHeader("X-Custom-Header", "test-value-12345");
    response.setHeader("X-Request-Id", java.util.UUID.randomUUID().toString());
    response.getOutputStream().write("HEADERS_OK".getBytes("UTF-8"));
    response.getOutputStream().flush();
%>