<cfsetting enablecfoutputonly="true">
<cfscript>
bytes = createObject("java", "java.io.ByteArrayOutputStream").init(9000);
for (i = 0; i < 9000; i++) {
    bytes.write(javaCast("int", (i % 254) + 1));
}
binaryData = bytes.toByteArray();
</cfscript>
<cfcontent type="application/octet-stream" reset="true" variable="#binaryData#">