<%--
  Test: GET returning a 20,510-byte real PNG (playstore-icon.png)
  Expected body size: 20,510 bytes
  Produces 3 SendBodyChunk packets (~8186 + ~8186 + ~4138 bytes).
  Each chunk may drop or add bytes via GetUserDataBytes bug.
  PNG corruption is verifiable by comparing response against original file.
  Writes entire file at once to let Tomcat AJP connector handle chunking.
--%><%@ page contentType="image/png" buffer="none" %><%
    String path = application.getRealPath("/playstore-icon.png");
    java.io.File file = new java.io.File(path);
    byte[] allBytes = java.nio.file.Files.readAllBytes(file.toPath());
    response.getOutputStream().write(allBytes);
    response.getOutputStream().flush();
%>