<%--
  Test: GET request returning text with body size NOT multiple of 4
  Expected body size: 5 bytes ("ABCDE")
  Padding: 3 bytes (to reach multiple of 4)
  This exposes Bug 1: GetUserDataBytes will include padding bytes in output
--%><%@ page contentType="text/plain; charset=utf-8" buffer="none" %><%
    response.getOutputStream().write("ABCDE".getBytes("UTF-8"));
    response.getOutputStream().flush();
%>