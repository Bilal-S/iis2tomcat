iis2tomcat
==========

-- Hands-down the easiest connector to deploy for IIS to Tomcat connectivity -- ;o)

This is the modern method of connecting IIS and Apache Tomcat. Most likely to use a Railo / JSP driven backend.
The BonCode AJP (Apache JServ Protocol version 1.3) Connector uses already existing pathways to connect to Apache Tomcat. 
In general it is a preference question how you connect IIS to tomcat, though, there are several advantages with the BonCode connector vs the old ISAPI connector:
*   no ISAPI code, IIS6 vestiges or backward compatibility elements needed
*   all managed code for IIS7 using the modern extensibility framework
*   works on IIS5.1, IIS6 and IIS7, IIS7.5, IIS8 (Windows XP through Windows 8)
*   speed, throughput, and stability improvements 
*   easier control on IIS side using IIS style menus and UI
*   no virtual directories and virtual mappings needed
*   configuration can be inherited to sub-paths and virtual sites
*   easy install/uninstall
*   support partial stream sending to browser (automatic flushing) with faster response to client
*   support both 32/64 bit of Windows with same process and files
*   transfer of all request headers
*   build-in simple-security for web-administration pages (Tomcat, Railo, OpenBD, ColdFusion)
*   IP6 support
*   Additional HTTP headers data is passed to Tomcat servlet container (previously unavailable)
*   Improved transfer of SSL data to Tomcat servlet container
*   Support improved translation of load balancer headers to determine correct client IP 
*   Support client fingerprint mechanism for use with safer sessions
*   Support for Adobe Coldfusion 10 AJP dialect
*   Support for Railo and OpenBD CFML Engines 
*   Support for alternate Path-Info header transmission via AJP

If you were using a proxy or URL rewrite engine you would also benefit from:
*   Fully integrated SSL to Servlet container
*   Tomcat threading awareness (will not overload Tomcat and drop connections unnecessarily)
*   Your servlets and scripts will receive correct HTTP header/URL/IP information for processing
*   reduced traffic and processing on both IIS and tomcat sides
*   allows you to connect to multiple tomcat instances from within one IIS site without interfering with ISAPI connector, e.g. Shibboleth and ColdFusion 10/Railo

-----------------------------------------
VERSION HISTORY:

Version 1.0.13 Updates :
* Add: secure Adobe CF10 CFIDE admin paths when setting EnableRemoteAdmin is set to False

Version 1.0.12 Updates :
* Fix: installer iteration of web sites would duplicate site names
* Add: Support Windows 8 / Windows Server 2012
* Add: Support for .net 4 / .net 4.5
* Add: installer detect if .net framework 4/4.5 is installed and skip .net 3.5
* Add: installer detect if Windows 8 / Server 12: install .net 4.5 feature/extensibility/asp.net
* Add: installer add default documents on IIS7/IIS8 for selected handlers  (index.jsp/index.cfm)
* Add: installer expanded uninstall to remove additional configuration and features

Version 1.0.11 Updates:
* Fix: IIS request timeout before tomcat timeout would leave data in tcp stream cache that would be displayed in next connection when using connection pool
* Fix: Exposing ColdFusion 10 Webservices calls (cfc remoting) caused packet order error and IIS exception
* Add: Added setting (EnableClientFingerPrint) for simple client fingerprinting calculations, result in HTTP header xajp-clientfingerprint

Version 1.0.10 Updates:
* Fix: Handle exception thrown when required protocol information is not supplied by client
* Upd: Logging format changes. Reschuffled log levels. Added HTTP Headers explicit logging
* Upd: Update to Manuals
* Add: Do not add prefix to inbound Uri if the specified prefix is already at the beginning of Uri

Version 1.0.9 Updates:
* Fix: Adobe PathRequest packets for absolute paths and relative path that are different from document path
* Upd: Update Manuals
* Upd: Remove duplicate HTTP headers
* Add: Support for Adobe ColdFusion 10 file upload data packet order
* Add: Support for AllowPartiallyTrustedCaller in the .net assembly so connector can be run with restricted permissions
* Add: Support for EMC Documentum chunked file transfers
* Add: retranslate header names and cases from IIS standard to original client supplied (RAW) format when possible

Version 1.0.8 Updates:
* Fix: More Adobe ColdFusion 10 release version adjustments, added setting to switch into Adobe mode
* Upd: Update Manuals

Version 1.0.7 Updates:
* Fix: Non default large PacketSizes (over 32KB) would cause arythmetic errors
* Fix: Adobe ColdFusion 10 release version adjustments
* Upd: Update Manuals

Version 1.0.6 Updates:
* Add: setting file location in version identification. Also identify whether defaults are used or setting file data.
* Add: auto-correction for Path-Info (xajp-path-info) to RFC3875 standard
* Upd: access to administrator when deployed as war is not allowed when disable remote access is selected

Version 1.0.5 Updates:
* Add: Installer allow unlocking of IIS sub configurations (sub path)
* Add: PacketSize option to support non-default transfer packet sizes

Version 1.0.4 Updates:
* Add: path prefix setting to allow mapping of a given IIS site root to designated tomcat application
* Fix: Installer Windows 2003 and Windows XP 64-bit asp.net references

Version 1.0.3 Updates:
* Add: Connector Version identifier through local URL parameter call (BonCodeConnectorVersion=yes)
* Add: Installer enable flush option
* Add: Installer enable client IP detection
* Add: Installer scripted support for uninstall directory

Version 1.0.2 Updates:
* Fix: reading of port from setting file
* Add: vendor (Adobe) specific AJP extensions

Version 1.0.1 Updates:
* Add: installer option for header data support 
* Add: Settings for HTTP Status Codes option, ErrorRedirectURL, TCPClientErrorMessage, TCPStreamErrorMessage
* Add: Automatic connection recovery after tomcat has been restarted while IIS is still running.
* Add: Error message displays for different connection errors occur (rather than empty screens).
* Edit: In global install mode. Change settings directory from system32 to windows.

Version 1.0 Updates:
* Add: script support for installer for automated deployements

Version 1.0rc Updates:
* Add: installer deploy in GAC mode
* Add: installer configure tomcat server.xml if on same server

Version 0.9.2.9 Updates:
* Add: setting to show suppressed headers (AllowEmptyHeaders). The connectors skips headers that do not have data to speed processing. Set to true to send empty headers as well.
* Add: setting to send path info in alternate http header (PathInfoHeader). This is to bypass tomcat bug with AJP path-info transfer.
* Fix: Remove default for ResolveRemoteAddrFrom (HTTP_X_FORWARDED_FOR). Will now need to be explicitly set to be enabled.

Version 0.9.2.8 Updates:
* Fix: extend timeout for socket so that longer timeouts in IIS Application Pool do not result in closed socket errors
* Add: automatic translation of client IPs to account for intermediaries (load balancers and proxies), e.g. HTTP_X_FORWARDED_FOR to REMOTE_ADDR automatic rewrite
* Add: Strong signing of assemblies so project can be placed in GAC (Global Assembly Cache)

Version 0.9.2.7 Updates:
* Add: Improve http flush detection, add network stream behavior in addition to timer.
* Fix: Zero content tomcat packages would cause display of error message in browser.
* Fix: ignore binary transfers that contain AJP protocol magic markers would lead to empty screen.

Version 0.9.2.6 Updates:
* Fix: correct transfer of non-standard headers without the http prefix added by IIS
* Fix: compatibility with Axis1 projects
* Add: no longer force conversion of text to UTF8. Will pass directly content as is to browser from tomcat regardless of content-type declaration.
* Add: Setting [ForceSecureSession] to force secure session via SSL. Will automatically exchange secure session cookies and force all communication over SSL to the Webserver.
* Add: Settings for timeout tcp/ip connections are exposed and can be changed by user.

Version 0.9.2.5 Updates:
* Fix: separate AJP attributes from headers. All present optional http attributes are transferred even if not processed by tomcat.
* Fix: Recognize misordered packets from Tomcats (out of order GET BODY CHUNK) and provide proper response. This would result in blank screen.
* Add: add new setting to transfer optional header (x-tomcat-docroot) note capitalization
* Add: automatically fix wrong content-length declaration if content is missing. Fill in empty characters where content is missing. This is not correct but will continue browser processing.
* Fix: Use of System Timer would leak memory when thread was destroyed
* Fix: AJP protocol header server and port designations send to servlet container were incorrect when IIS and tomcat were remoted.
* Add: HTTP header Blacklist and Whitelist options in settings file
* Fix: Correct flush protocol detection problem so HTTP flushes can be detected and spooled to browser.

Version 0.9.2.4 Updates:
* Add: Installer update. Installation of .net framework feature as option for windows 7 and windows 2008+
* Add: User friendly error messages when we cannot connect to Apache Tomcat

Version 0.9.2.3 Updates:
* Fix: JSP response.sendRedirect() call uses HTTP 302 redirect. Would lead to timeout.
* Fix: Specific issues with Jetbrains TeamCity application implementation and protocol behavior (AJAX)
* Fix: Stop waiting on tomcat when redirect directive is given
* Fix: Handle Railo vs. tomcat native variances in File upload behavior with and without redirects.

Version 0.9.2.2 Updates:
* Fix: gzip compression handling from servlets. Set correct content encoding.
* Fix: install on IIS7 did not write setting file
* Add: Updated troubleshooting information in Manuals 

Version 0.9.2.1 Updates:
* Fix: Change FlushThreshhold defaults to handle graphics pushed by script files better with IE, e.g. gif.cfm / png.cfm etc.

Version 0.9.2 Updates:
* Fix: Issues with UTF-8 conversion from double byte regions
* Fix: Forced Disconnect mode was not enabled when MaxConnections was set to zero, references to old connections would be maintained unnecessarily
* Add: Automatic release of all connection when settings file is changed 

Version 0.91 Updates:
* Added automated installer beta (all windows versions from XP on)
* Added thread throttling to not overwhelm tomcat with request. Now allows forcing reconnect for every request if so desired
* Updated documentation with trouble shooting section

END OF VERSION HISTORY
--------------------------------------------------

Manual Installation instructions are in the PDF within the project download package. 

Using automated installer contained in package is recommended though.

As usual any feedback is appreciated.
