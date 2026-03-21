<!---
  Test: GET request returning a 1x1 transparent PNG (67 bytes)
  Expected body size: 67 bytes
  Padding: 1 byte (68 is multiple of 4)
  This exposes Bug 1: GetUserDataBytes will drop 1 byte of actual image data
  PNG is a 1x1 transparent pixel: iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==
--->
<cfcontent type="image/png" reset="true">
<cfset pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==">
<cfset pngBytes = ToBinary(pngBase64)>
<cfoutput>#pngBytes#</cfoutput>