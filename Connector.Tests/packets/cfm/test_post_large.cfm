<!---
  Test: POST with ~20KB of form data (multi-packet scenario)
  Expected: receives a large "data" field, returns byte count
  This exposes Bug 2: integer division in numOfPackets calculation
  Tests: Multi-packet POST body segmentation, TomcatGetBodyChunk flow
--->
<cfcontent type="text/plain; charset=utf-8">
<cfset receivedLen = Len(form.data)>
<cfoutput>RECEIVED_LENGTH=#receivedLen#</cfoutput>