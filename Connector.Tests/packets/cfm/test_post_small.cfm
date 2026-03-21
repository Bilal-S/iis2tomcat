<!---
  Test: POST with small form data
  Expected: receives form fields "field1" and "field2", echoes them back
  Body size: small (under single packet threshold ~8186 bytes)
  Tests: ForwardRequest packet construction, POST body parsing
--->
<cfcontent type="text/plain; charset=utf-8">
<cfset result = "field1=" & form.field1 & ",field2=" & form.field2>
<cfoutput>#result#</cfoutput>