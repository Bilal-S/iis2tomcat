<%--
  Test: POST with ~20KB of form data (multi-packet scenario)
  Expected: receives a large "data" field, returns byte count
  This exposes Bug 2: integer division in numOfPackets calculation
  Tests: Multi-packet POST body segmentation, TomcatGetBodyChunk flow
--%><%@ page contentType="text/plain; charset=utf-8" buffer="none" %><%
    String data = request.getParameter("data");
    int receivedLen = (data != null) ? data.length() : 0;
    response.getOutputStream().write(("RECEIVED_LENGTH=" + receivedLen).getBytes("UTF-8"));
    response.getOutputStream().flush();
%>