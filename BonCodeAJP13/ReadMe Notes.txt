Bilal Soylu
Notes on AJP13 implementation:
Licensed under Creative Commons License version 3.0


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

Version 0.9.1 Updates:
•   Added automated installer beta (all windows versions from XP on)
•   Added thread throttling to not overwhelm tomcat with request. Now allows forcing reconnect for every request if so desired
•   Updated documentation with trouble shooting section

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
