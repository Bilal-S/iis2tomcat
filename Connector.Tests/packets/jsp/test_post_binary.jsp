<%--
  Test: POST with inline base64-encoded image data, server decodes and returns as binary
  Expected: sends base64 in "imagedata" field, returns the decoded PNG bytes
--%><%@ page contentType="image/png" buffer="none" %><%
    String imagedata = request.getParameter("imagedata");
    if (imagedata != null && !imagedata.isEmpty()) {
        byte[] pngBytes = java.util.Base64.getDecoder().decode(imagedata);
        response.getOutputStream().write(pngBytes);
    } else {
        String error = "ERROR: send imagedata form field with base64 PNG";
        response.setContentType("text/plain");
        response.getOutputStream().write(error.getBytes("UTF-8"));
    }
    response.getOutputStream().flush();
%>