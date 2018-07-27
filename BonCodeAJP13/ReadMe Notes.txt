Bilal Soylu
Notes on AJP13 implementation:
Licensed under Apache License, Version 2.0

Automatic Installation:
-----------------------
- run installer if possible (Connector_Setup.exe) to handle all the details


Manual Installation:
-------------------
IIS7 - IIS10:
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
	Tomcat with Lucee	
	Axis2 webservices

==========================================

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

Version 1.0.17 Updates:
* Fix: 404 redirect by connector could leave stream cache to be reused on the connection.
* Add: New Setting (FPHeaders) to determine HTTP headers used for client fingerprint.
* Add: Included logger changes from Igal: Log file time stamp and debug log method
* Upd: Change the default log file name and extension to BonCodeAJP13Connection[yyyyMMdd].log
* Upd: Internal class cleanup
* Fix: Handle exception when determining Physical Path in case invalid virtual path references are passed

Version 1.0.18 Updates:
* Fix: Dominic's fix for Empty (null) Headers. When headers are transferred with null values instead of empty string error would be thrown.
* Add: Set HTTP error code for Connection Errors with Tomcat included as stub in this version
* Upd: Improve error messages for connection problems on local and remotes
* Upd: Changed name reference of setting [FlushThreshold] to [FlushThresholdTicks] to clarify that we use time ticks.
* Add: Added new setting [FlushThresholdBytes]. This will start spooling buffer after the byte threshold is reached. Aid in streaming large files via Tomcat.
* Add: Allow BIN directory and Setting files to be located on UNC path so servers can share libraries and configuration
* Add: Expanded log file name generation so that a shared setting file among multiple servers does not produce write contention
* Add: Concurrent connection count estimate in log file with log level 2 (experimental)
* Upd: Added back the PacketSize default when you switch to Adobe mode that was removed in 1.0.15, it will be set to 65531 bytes
* Add: When buffering whole content switch from chunked transfer-type to fixed-length transfer for non binary data
* Upd: Remove automatic IIS side redirect for any 30x status and use Tomcat directive instead
* Upd: Installer does no longer remove IIS features automatically without confirmation or in silent mode
* Fix: Adobe specific AJP13 file path extension would fail with Unicode path name requests
* Fix: Adobe use of iso-8859-1 encoding even when UTF8 is exchanged.

Version 1.0.19 Updates:
* Fix: raw header translation when custom HTTP headers are introduced with duplicate names
* Add: Support X509 client certificates via AJP attributes in addition to HTTP headers
* Add: new setting SkipIISCustomErrors for stopping IIS from displaying error pages when servlet returns error status.
* Add: new setting LogIPFilter to selectivly log client streams, supports regex 

Version 1.0.20 Updates:
* Add: on connection error with forward URL we will forward error information to error page, when setting TomcatConnectErrorURL is populated, URL attributes : errorcode & detail will be added to target request
* Add: on connection error without forward URL we will use error code 502 for any connection errors that need to be displayed.
* Add: support for Lucee CFML engine and add Lucee administrator security


Version 1.0.21 Updates:
* Upd: only mark threads with error HTTP 400 and above to be reset. Previously reset any non HTTP 200 thread.
* Add: added new x-vdirs header that will transmit IIS virtual directory mappings when EnableHeaderDataSupport setting is turned on. Additional permissions need to be assigned to connector for this to work.
* Add: Installer can make appropriate changes to permission when needed to list virtual directories.

Version 1.0.22 Updates:
* Add: new HTTP header x-webserver-context when EnableHeaderDataSupport setting is turned on. Will contain the IIS site context. Used with mod_cfml this can assist in auto creation of Tomcat contexts.
* Upd: disable connection pool by default (MaxConnections=0). Most reports of bugs were related to this as users did not understand repercussions of this setting. 


Version 1.0.23 Updates:
* Fix: IIS InstanceId determination on certain computers would throw errors.
* Fix: Windows 2012 flexgateway receiving out of order packets
* Fix: Windows 2012 flexgateway empty packets from tomcat would cause connection abort

Version 1.0.24 Updates:
* Add: New setting RequestSecret to support Tomcat requiredSecret setup. A shared secret can be used to secure the AJP connection.
* Add: Cache of settingfile reads
* Add: New setting EnableAggressiveGC for more frequent Garbage Collection. Users transfering large number of bytes, e.g. images and file assets through the connector rather than through IIS.
* Add: Installer can now install site-specific handlers
* Upd: Changed default for HeaderBlacklist to decrease the size of initial packet. The HTTP headers URL,SERVER_SOFTWARE,SERVER_NAME, and SERVER_PROTOCOL will no longer be automatically transferred.
* Fix: Correct virtual direcotry listing when Site Ids where manually changed by users

Version 1.0.25 Updates:
* Add: New setting for ModCfmlSecret
* Add: Installer setting file can accept requestSecret and modCfmlSecret parameters

Version 1.0.26 Updates:
* Add: Flushing priority. When both time and byte flushing are specified, we will first wait for time, then use either time or byte markers
* Add: Setting DocRoot for manual HTTP x-tomcat-docroot override, in cases where Tomcat is running on Linux
* Add: Block client HTTP headers that match connector generated HTTP headers

Version 1.0.27 Updates:
* Add: optional logging of connection issues to Windows log if a Log Source is available
* Add: allways log full stack trace in log file when any type of logging is enabled
* Add: additional system error condition catches throughout comm cycle
* Add: when we encounter stream read errors and null buffers, we will retry to read stream instead of throw exception
* Add: suppress logging certain errors when Tomcat is stopped before IIS to avoid confusion
* Add: suppress client disconnect errors (thread aborted) 
* Upd: installer change information text, remove requirement to select a handler 
* Upd: Adobe Coldfusion ExpandPath() with invalid path will no longer throw exception but return invalid path
* Fix: error when IIS site instance IDs were set to a value higher than 32767, now the max value is: 4294967295 (UInt32)

Version 1.0.28 Updates:
* Add: additial stack trace to windows event log
* Upd: continue spooling data even after an unallowed header change has been attempted by servlet code

Version 1.0.29 Updates:
* Add: additial stack trace to windows event log when we cannot write to logfiles
* Add: AJP native flush detection. A correctly formated AJP flush packet will lead to flushing when any of the flush flags is enabled.

Version 1.0.30 Updates:
* Upd: reduce system flushing events and use IIS delayed buffer write to client when user has disabled flushing

Version 1.0.31 Updates:
* Fix: class type comparison failed in .net framework resulting in skipped terminator BonCodeAJP13.TomcatPackets.TomcatSendBodyChunk package. This looked like pages had stopped processing.

Version 1.0.32 Updates:
* Fix: connection pool did not reuse existing connection even when setting was set
* Fix: remove thread contention for object collections under load causing extra 502 errors

Version 1.0.33 Updates:
* Fix: IIS will throw error code is 0x800704cd when flushing to a disconnected client

Version 1.0.34 Updates:
* Fix: tomcat error redirect URL would not be used with remote connected clients.

Version 1.0.35 Updates:
* Fix: EventLog.SourceExists() requires security permissions causing exception
* Upd: Do not record system events for invalid URL to physical path conversions
* Upd: Encoding of redirected URL attributes when TomcatConnection Error
* Add: additional error details for windows system events

Version 1.0.36 Updates:
* Upd: change TCP Read/Write timeout setting behavior. Now if these timeouts are not set, they default to infinite (they do not timeout).

Version 1.0.37 Updates:
* Fix: Array out of bound issues when marker bytes where at the end of network package
* Fix: Timing issue with packet delivery. Introduce wait for variable speed networks and network latency.
* Fix: Apache Axis Soap processor embedded in Adobe ColdFusion will not instantiate when CONTENT_LENGTH is specified as zero
* Upd: Moved to minumum requirement of .net Framework 4.5
* Upd: Removed .net framework 3.5 distribution


Version 1.0.38 Updates:
* Upd: block /pms path from being used when admin access from remote is not allowed. This is for Adobe CF.
* Fix: location of global setting file was placed under GAC rather then c:\windows

--------------------------------------

Version 1.0.50 Updates Planning:
* Add: installer add /REST folder wildcard mapping for Railo when individual sites are chosen
* Add: installer detect Adobe CF10 and add screen to toggle adobe mode (bypass others)
* Add: installer add mapping for *.cfchart / *.cfres / *.cfr 

Version 2.0 Planning:
* Add: Log4Net
* Add: Automatic Log Cycle
* Add: Load Balancing 
* Upd: Install Option Selection Default. if site-by-site is selected it will remain default.
* Add: Installer unblock DLL script (needed for Windows8)
* Add: Installer check web pools and ensure that they will work with connector for given site. 
* Add: Installer option to change per site web application pool to match what connector is needing.
