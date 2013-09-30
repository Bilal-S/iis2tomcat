Bilal Soylu
Notes on AJP13 implementation:
Licensed under Apache License, Version 2.0


Installation:
-------------
IIS7:
-Need IIS .net extensibility feature on IIS7
-create BIN directory under web-document root e.g. c:\inetpub\wwwroot\BIN
-copy the dll files in the BonCodeIIS project into BIN directory
-register handler for extension(Request Path) using Type:
BonCodeIIS.BonCodeCallHandler


Test Remaining:
---------------
- Finalize all Unit Test Assertions


Code Conventions:
-----------------
Upper Case:
	Class Names
	Methods
	Properties

private scope (instance):
	prefixed with p_

Lower Case:
	argument names
	method variables

All Upper:
	any part of enumeration


Main Test Environment:
	Railo-Administrator
	Axis2 webservices

Version 0.9.1 Updates:
* Add: Added automated installer beta (all windows versions from XP on)
* Add: Added thread throttling to not overwhelm tomcat with request. Now allows forcing reconnect for every request if so desired
* Upd: Updated documentation with trouble shooting section

Version 0.9.2 Updates:
* Fix: Issues with UTF-8 conversion from double byte regions
* Fix: Forced Disconnect mode was not enabled when MaxConnections was set to zero, references to old connections would be maintained unnecessarily
* Add: Automatic release of all connection when settings file is changed 

Version 0.9.2.1 Updates:
* Fix: Change FlushThreshhold defaults to handle graphics pushed by script files better with IE, e.g. gif.cfm / png.cfm etc.


Version 0.9.2.2 Updates:
* Fix: gzip compression handling from servlets. Set correct content encoding.
* Fix: install on IIS7 did not write setting file
* Add: Updated troubleshooting information in Manuals 

Version 0.9.2.3 Updates:
* Fix: JSP response.sendRedirect() call uses HTTP 302 redirect. Would lead to timeout.
* Fix: Specific issues with Jetbrains TeamCity application implementation and protocol behavior (AJAX)
* Fix: Stop waiting on tomcat when redirect directive is given
* Fix: Handle Railo vs. tomcat native variances in File upload behavior with and without redirects.
* Inf: ISAPI redirector is not protocol compliant if packet size with form data is below max. bytes.
* Inf: ISAPI redirector is not protocol compliant when HTTP 302 redirect is issued and comm. is terminated by tomcat with END RESPONSE. Keeps sending data.


Version 0.9.2.4 Updates:
* Add: Installer update. Installation of .net framework feature as option for windows 7 and windows 2008+
* Add: User friendly error messages when we cannot connect to Apache Tomcat


Version 0.9.2.5 Updates:
* Fix: seperate AJP attributes from headers. All present optional http attributes are transferred even if not processed by tomcat.
* Fix: Recognize misordered packets from Tomcats (out of order GET BODY CHUNK) and provide propper response. This would result in blank screen.
* Add: add new setting to transfer optional header (x-tomcat-docroot) note capitalization
* Add: default secure connection redirect and session support via AJP (SSL); tomcat will automatically choose this mode and the connector will support it
* Add: automatically fix wrong content-length decleration if content is missing. Fill in empty characters where content is missing. This is not correct but will continue browser processing.
* Add: IP6 support
* Fix: Use of System Timer would leak memory when thread was destroyed
* Fix: AJP protocol header server and port designations send to servlet container were incorrect when IIS and tomcat were remoted.
* Add: HTTP header Blacklist and Whitelist options in settings file
* Fix: Correct flush protocol detection problem so HTTP flushes can be detected and spooled to browser.


Version 0.9.2.6 Updates:
* Fix: correct transfer of non-standard headers without the http prefix added by IIS
* Fix: compatibility with Axis1 projects
* Add: no longer force conversion of text to UTF8. Will pass directly content as is to browser from tomcat regardless of content-type declaration.
* Add: Setting [ForceSecureSession] to force secure session via SSL. Will automatically exchange secure session cookies and force all communication over SSL to the Webserver.
* Add: Settings for timeout tcp/ip connections are exposed and can be changed by user.

Version 0.9.2.7 Updates:
* Add: Improve http flush detection, add network stream behavior in addition to timer.
* Fix: Zero content tomcat packages would cause display of error message in browser.
* Fix: ignore binary transfers that contain AJP protocol magic markers would lead to empty screen.

Version 0.9.2.8 Updates :
* Fix: extend timeout for TCP socket so that longer timeouts in IIS Application Pool do not result in closed socket errors
* Add: automatic translation of client IPs to account for intermediaries (load balancers and proxies), e.g. HTTP_X_FORWARDED_FOR to REMOTE_ADDR automatic rewrite
* Add: assembly signing and strong name support so that library files can be deployed in Global Assembly Cache

Version 0.9.2.9 Updates:
* Add: setting to show suppressed headers (AllowEmptyHeaders). The connector skips headers that do not have data to speed processing. Set to true to send empty headers as well.
* Add: setting to send path info in alternate http header (PathInfoHeader). This is to bypass tomcat bug with AJP path-info transfer.
* Fix: Remove default for ResolveRemoteAddrFrom (HTTP_X_FORWARDED_FOR). Will now need to be explicitly set to be enabled.

Version 1.0 Updates:
* Add: installer deploy in GAC mode
* Add: installer accept setting file for silent deployment
* Add: installer configure tomcat server.xml if on same server

Version 1.0.1 Updates:
* Add: installer option for header data support 
* Add: Settings for HTTP Status Codes option, ErrorRedirectURL, TCPClientErrorMessage, TCPStreamErrorMessage
* Add: Automatic connection recovery after tomcat has been restarted while IIS is still running.
* Add: Error message displays for different connection errors occur (rather than empty screens).
* Edt: In global install mode. Change settings directory from system32 to windows.

Version 1.0.2 Updates:
* Fix: Port setting was not read from setting file
* Add: Adobe Extension support to AJP

Version 1.0.3 Updates:
* Add: Connector Version identifier through local URL parameter call (BonCodeConnectorVersion=yes)
* Add: Installer enable flush option
* Add: Installer enable client IP detection
* Add: Installer scripted support for uninstall directory

Version 1.0.4 Updates:
* Add: path prefix setting to allow mapping of a given IIS site root to designated tomcat application
* Fix: Installer Windows 2003 and Windows XP 64-bit asp.net references

Version 1.0.5 Updates:
* Add: Installer allow unlocking of IIS sub configurations (sub path)
* Add: packetSize option to support non-default packet sizes

Version 1.0.6 Updates:
* Add: setting file location in version identification. Also identify whether defaults are used or setting file data.
* Add: auto-correction for Path-Info (xajp-path-info) to RFC3875 standard
* Upd: Accesses to administrator when deployed as war is not allowed when “disable remote access” is selected

Version 1.0.7 Updates:
* Fix: Non default large PacketSizes (over 32KB) would cause arithmetic errors
* Fix: Adobe ColdFusion 10 release version adjustments
* Upd: Update Manuals

Version 1.0.8 Updates:
* Fix: More Adobe ColdFusion 10 release version adjustments, added setting to switch into Adobe mode
* Upd: Update Manuals

Version 1.0.9 Updates:
* Fix: Adobe PathRequest packets for absolute paths and relative path that are different from document path
* Upd: Update Manuals
* Upd: Remove duplicate HTTP headers
* Add: Support for Adobe ColdFusion 10 file upload data packet order
* Add: Support for AllowPartiallyTrustedCaller in the .net assembly so connector can be run with restricted permissions
* Add: Support for EMC Documentum chunked file transfers
* Add: retranslate header names and cases from IIS standard to original client supplied (RAW) format when possible


Version 1.0.10 Updates:
* Fix: Handle exception thrown when required protocol information is not supplied by client
* Upd: Logging format changes. Reshuffled  log levels. Added HTTP Headers explicit logging (3)
* Upd: Update to Manuals
* Add: Do not add prefix to inbound Uri if a prefix is specified to be added is already at the beginning of Uri

Version 1.0.11 Updates:
* Fix: IIS request timeout before tomcat timeout would leave data in tcp stream cache that would be displayed in next connection when using connection pool
* Fix: Exposing ColdFusion 10 Webservices calls (cfc remoting) caused packet order error and IIS exception
* Add: Added setting (EnableClientFingerPrint) for simple client fingerprinting calculations, result in HTTP header xajp-clientfingerprint

Version 1.0.12 Updates:
* Fix: installer iteration of web sites would duplicate site names
* Add: Support Windows 8 / Windows Server 2012
* Add: Support for .net 4 / .net 4.5
* Add: installer detect if .net framework 4/4.5 is installed and skip .net 3.5
* Add: installer detect if Windows 8 / Server 12: install .net 4.5 feature/extensibility/asp.net
* Add: installer add default documents on IIS7/IIS8 for selected handlers  (index.jsp/index.cfm)
* Add: installer expanded uninstall to remove additional configuration and features

Version 1.0.13 Updates:
* Add: secure Adobe CFIDE admin paths when setting EnableRemoteAdmin is set to False

Version 1.0.14 Updates:
* Add: display more error details when processing call on localhost vs remote
* Add: installer display warning if there is an installer.settings file and the setup is started interactively by user
* Fix: UTF-8 headers in URi would not transfer correctly
* Add: error handling when ColdFusion 10 tomcat instance sents zero byte packages
* Add: detect log file contention when multiple connector instances are running
* Add: Setting LogFile to specify a different log file name from default

Version 1.0.15 Updates:
* Upd: For CF10 remove packet size setting from AdobeMode operation. Use default tomcat packet size.
* Fix: SSL Key size transfer in HTTP Attributes used wrong data type
* Fix: Handle Tomcat bug that would result in packets being sent after tomcat already had declared end of transmission (EndResponse)
* Add: Workaround for Tomcat Bug that would introduce null data

Version 1.0.16 Updates:
* Add: Block access to WEB-INF and META-INF path on any site when remote access to Admin is disabled
* Add: installer block WEB-INF and META-INF access automatically using IIS facilities for all websites

