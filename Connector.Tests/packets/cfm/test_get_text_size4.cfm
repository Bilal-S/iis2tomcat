<!---
  Test: GET returning exactly 4 bytes (body size = multiple of 4, padding = 0)
  Expected body size: 4 bytes ("ABCD")
  Padding: 0 bytes
  This directly exposes Bug 1: GetUserDataBytes will drop the last byte
  Current behavior: returns [41,42,43] instead of [41,42,43,44]
--->
<cfcontent type="text/plain; charset=utf-8">
<cfoutput>ABCD</cfoutput>