<!---
  Test: POST with inline base64-encoded image data, server decodes and returns as binary
  Expected: sends base64 in "imagedata" field, returns the decoded PNG bytes
  This tests: POST body with binary-safe content, binary response round-trip
  Also exposes Bug 1 on the response side (67-byte PNG has padding)
--->
<cfcontent type="image/png" reset="true">
<cfif StructKeyExists(form, "imagedata")>
  <cfset pngBytes = ToBinary(form.imagedata)>
  <cfoutput>#pngBytes#</cfoutput>
<cfelse>
  <cfset pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==">
  <cfoutput>ERROR: send imagedata form field with base64 PNG</cfoutput>
</cfif>