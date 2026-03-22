<cfsetting enablecfoutputonly="true">
<!---
  Test: GET returning a 20,510-byte real PNG (playstore-icon.png)
  Expected body size: 20,510 bytes
  Produces 3 SendBodyChunk packets (~8186 + ~8186 + ~4138 bytes).
  Each chunk drops 1 byte via GetUserDataBytes bug = 3 bytes lost total.
  PNG corruption is verifiable by comparing response against original file.
  Uses cfcontent file= for zero-copy streaming from disk.
--->
<cfcontent type="image/png" file="#ExpandPath('./playstore-icon.png')#" reset="true">