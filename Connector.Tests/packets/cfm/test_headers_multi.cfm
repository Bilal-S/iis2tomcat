<!---
  Test: Returns multiple response headers including Set-Cookie headers
  Expected: 2 Set-Cookie headers, custom X-Custom-Header, Content-Type
  Tests: Header parsing in TomcatSendHeaders, specifically the |, delimiter
  Also tests multiple Set-Cookie handling (Bug area in GetHeaders)
--->
<cfheader name="Set-Cookie" value="session1=abc123; Path=/; HttpOnly">
<cfheader name="Set-Cookie" value="session2=xyz789; Path=/; HttpOnly">
<cfheader name="X-Custom-Header" value="test-value-12345">
<cfheader name="X-Request-Id" value="#CreateUUID()#">
<cfcontent type="text/plain; charset=utf-8">
<cfoutput>HEADERS_OK</cfoutput>