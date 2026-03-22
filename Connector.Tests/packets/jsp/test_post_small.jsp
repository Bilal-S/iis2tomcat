<%--
  Test: POST with small form data
  Expected: receives form fields "field1" and "field2", echoes them back
  Body size: small (under single packet threshold ~8186 bytes)
  Tests: ForwardRequest packet construction, POST body parsing
--%><%@ page contentType="text/plain; charset=utf-8" buffer="none" %><%
    String field1 = request.getParameter("field1");
    String field2 = request.getParameter("field2");
    String result = "field1=" + field1 + ",field2=" + field2;
    response.getOutputStream().write(result.getBytes("UTF-8"));
    response.getOutputStream().flush();
%>