<%--
  Test: Returns various HTTP status codes based on "code" URL parameter
  Usage: test_status_codes.jsp?code=200 (default), 301, 404, 500, etc.
  Expected: sets the status code and returns body text indicating the code
  Tests: TomcatSendHeaders status parsing (GetStatus method)
  Note: 301 includes a Location header for redirect testing
--%><%@ page contentType="text/plain; charset=utf-8" buffer="none" %><%
    String codeParam = request.getParameter("code");
    int code = 200;
    try {
        if (codeParam != null) code = Integer.parseInt(codeParam);
    } catch (NumberFormatException e) {}

    if (code == 301) {
        response.setHeader("Location", "/test_get_simple.jsp");
    }
    response.setStatus(code);

    String statusText;
    switch (code) {
        case 200: statusText = "OK"; break;
        case 301: statusText = "Moved Permanently"; break;
        case 302: statusText = "Found"; break;
        case 304: statusText = "Not Modified"; break;
        case 403: statusText = "Forbidden"; break;
        case 404: statusText = "Not Found"; break;
        case 500: statusText = "Internal Server Error"; break;
        default: statusText = "Unknown"; break;
    }

    response.getOutputStream().write(("STATUS_" + code).getBytes("UTF-8"));
    response.getOutputStream().flush();
%>