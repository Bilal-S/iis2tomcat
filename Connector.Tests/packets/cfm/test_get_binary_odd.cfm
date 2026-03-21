<!---
  Test: GET request returning binary data with odd size (3 bytes)
  Expected body size: 3 bytes (0xFF 0xFE 0xFD)
  Padding: 1 byte (4 is multiple of 4)
  This exposes Bug 1: by accident works (3+4=7, 7-4=3 correct offset)
  But tests the parsing path for small binary payloads
--->
<cfcontent type="application/octet-stream" reset="true">
<cfset data = Chr(255) & Chr(254) & Chr(253)>
<cfoutput>#data#</cfoutput>