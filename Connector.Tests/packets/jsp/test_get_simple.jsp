<%--
  Test: GET request returning simple HTML
  Expected body size: 12 bytes ("Hello World!" = 12 chars, all ASCII)
  Padding: 0 bytes (12 is multiple of 4)
  Body content: Hello World!
  Uses getOutputStream() for exact byte control (no JSP Writer whitespace issues)
--%><%@ page contentType="text/html; charset=utf-8" buffer="none" %><%
    response.getOutputStream().write("Hello World!".getBytes("UTF-8"));
    response.getOutputStream().flush();
%>