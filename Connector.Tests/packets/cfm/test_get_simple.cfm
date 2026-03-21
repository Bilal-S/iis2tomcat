<!---
  Test: GET request returning simple HTML
  Expected body size: 12 bytes ("Hello World!" = 12 chars, all ASCII)
  Padding: 0 bytes (12 is multiple of 4)
  Body content: Hello World!
--->
<cfcontent type="text/html; charset=utf-8">
<cfset body = "Hello World!">
<cfoutput>#body#</cfoutput>