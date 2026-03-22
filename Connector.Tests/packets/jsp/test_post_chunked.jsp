<%--
  Test: Accepts chunked transfer encoding, echoes back content length
  Expected: returns "CONTENT_LENGTH=<n>" where n is the total bytes received
  Tests: Chunked transfer encoding handling
--%><%@ page contentType="text/plain; charset=utf-8" buffer="none" %><%
    int contentLen = request.getContentLength();
    if (contentLen < 0) {
        // Chunked: read manually
        java.io.InputStream is = request.getInputStream();
        int total = 0;
        byte[] buf = new byte[8192];
        int n;
        while ((n = is.read(buf)) != -1) {
            total += n;
        }
        response.getOutputStream().write(("CONTENT_LENGTH=" + total).getBytes("UTF-8"));
    } else {
        response.getOutputStream().write(("CONTENT_LENGTH=" + contentLen).getBytes("UTF-8"));
    }
    response.getOutputStream().flush();
%>