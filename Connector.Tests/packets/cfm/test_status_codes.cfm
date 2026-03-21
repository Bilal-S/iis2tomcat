<!---
  Test: Returns various HTTP status codes based on "code" URL parameter
  Usage: test_status_codes.cfm?code=200 (default), 301, 404, 500, etc.
  Expected: sets the status code and returns body text indicating the code
  Tests: TomcatSendHeaders status parsing (GetStatus method)
  Note: 301 includes a Location header for redirect testing
--->
<cfparam name="url.code" default="200">
<cfset code = Val(url.code)>
<cfif code EQ 301>
  <cfheader name="Location" value="/test_get_simple.cfm">
</cfif>
<cfheader statuscode="#code#" statustext="#GetHttpStatusString(code)#">
<cfcontent type="text/plain; charset=utf-8">
<cfoutput>STATUS_#code#</cfoutput>

<cffunction name="GetHttpStatusString" output="false" returntype="string">
  <cfargument name="code" type="numeric" required="true">
  <cfswitch expression="#arguments.code#">
    <cfcase value="200">OK</cfcase>
    <cfcase value="301">Moved Permanently</cfcase>
    <cfcase value="302">Found</cfcase>
    <cfcase value="304">Not Modified</cfcase>
    <cfcase value="404">Not Found</cfcase>
    <cfcase value="500">Internal Server Error</cfcase>
    <cfcase value="403">Forbidden</cfcase>
    <cfdefaultcase>Unknown</cfdefaultcase>
  </cfswitch>
</cffunction>