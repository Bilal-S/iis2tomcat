<!---
  Test: Accepts chunked transfer encoding, echoes back content length
  Expected: returns "CONTENT_LENGTH=<n>" where n is the total bytes received
  Tests: Chunked transfer encoding handling
--->
<cfcontent type="text/plain; charset=utf-8">
<cfset contentLen = GetHttpRequestData().content.len()>
<cfoutput>CONTENT_LENGTH=#contentLen#</cfoutput>