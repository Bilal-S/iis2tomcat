<!---
  Test: GET returning a large response (~16KB) requiring multiple SendBodyChunk packets
  Expected: 16384 bytes of repeated "X" characters
  This tests: multi-chunk response assembly, padding handling across chunk boundaries
  Each chunk is ~8182 bytes of user data (packet size 8192 minus overhead)
  With 16384 bytes, expect exactly 2 SendBodyChunk packets
--->
<cfcontent type="text/plain; charset=utf-8">
<cfset body = RepeatString("X", 16384)>
<cfoutput>#body#</cfoutput>